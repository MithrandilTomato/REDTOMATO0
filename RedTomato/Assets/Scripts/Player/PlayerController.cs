using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Skins")]
    [Tooltip("Inspector’dan atayacağınız skin sprite dizisi")]
    public Sprite[] skinSprites;

    [Header("Movement")]
    [Tooltip("Yatay hareket hızı")]
    public float moveSpeed = 5f;
    [Tooltip("Zıplama kuvveti")]
    public float jumpForce = 10f;

    [Header("UI Controls")]
    [Tooltip("Dokununca zıplatacak UI buton")]
    public Button jumpButton;
    [Tooltip("Joystick arka plan prefab’ı")]
    public RectTransform joystickBgPrefab;
    [Tooltip("Joystick saplama (handle) prefab’ı")]
    public RectTransform joystickHandlePrefab;
    [Tooltip("Joystick yarıçapı (pixel)")]
    public float joystickRadius = 100f;
    [Tooltip("Joystick’in görüneceği Canvas (Screen Space–Overlay)")]
    public Canvas joystickCanvas;

    [Header("Audio")]
    [Tooltip("Oyuncu hareket seslerini çalacak AudioSource")]
    public AudioSource audioSource;
    [Tooltip("Zıplama sesi clip’i")]
    public AudioClip jumpSound;
    [Tooltip("Hasar alma sesi clip’i")]
    public AudioClip damageSound;
    [Tooltip("Ölüm sesi clip’i")]
    public AudioClip deathSound;

    [Header("Physics")]
    [Tooltip("Oyuncu collider'ına uygulanacak sürtünme değeri")]
    public float friction = 0f;

    [Header("Slide Effect")]
    public ParticleSystem slipDustEffect;
    public AudioSource slipSoundEffect;
    public float slideThreshold = 0.1f;

    [Header("Health & Invincibility")]
    [Tooltip("Başlangıç can sayısı")]
    public int maxHealth = 3;
    [Tooltip("Hasar sonrası kaç saniye boyunca koruma olsun")]
    public float invincibilityDuration = 1f;

    [Header("Moving Platform Support")]
    [Tooltip("Hareket eden platformlar bu tag’e sahip olmalı")]
    public string movingPlatformTag = "MovingPlatform";

    // ————— Runtime —————
    Rigidbody2D rb;
    Collider2D col2d;
    SpriteRenderer sr;
    PhysicsMaterial2D physMat;

    int currentHealth;
    bool isInvincible, isDead;
    bool canJump, jumpRequest;
    bool isSliding;
    float slideSpeed;

    // Input
    float hInput;

    // Dynamic joystick
    RectTransform joystickBG, joystickHandle;
    int joystickTouchId = -1;   // -1 = none, -2 = mouse
    Vector2 joystickStartPos;
    float dynamicH;

    // Platformun Rigidbody’si (varsa)
    Rigidbody2D currentPlatformRb;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        // Fizik titreşimini azaltmak için interpolasyon
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.freezeRotation = true;

        // Sürtünme için material
        physMat = new PhysicsMaterial2D($"{name}_Mat")
        {
            friction = friction,
            bounciness = 0f
        };
        col2d.sharedMaterial = physMat;

        currentHealth = maxHealth;
    }

    void Start()
    {
        // Jump butonuna PointerDown ekle
        if (jumpButton != null)
        {
            var trigger = jumpButton.gameObject.AddComponent<EventTrigger>();
            var entry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            entry.callback.AddListener(_ => jumpRequest = true);
            trigger.triggers.Add(entry);
        }

        ApplySkin(PlayerPrefs.GetInt("CurrentSkinID", 0));
    }

    void Update()
    {
        if (isDead) return;

        HandleDynamicJoystick();

        // Yatay girdi oku
        hInput = (joystickBG != null) ? dynamicH : Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))
            jumpRequest = true;
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // — 1) Yatay hareket + platform hızı
        Vector2 vel = rb.velocity;
        vel.x = hInput * moveSpeed;
        if (currentPlatformRb != null)
            vel.x += currentPlatformRb.velocity.x;
        rb.velocity = vel;

        // — 2) Sliding
        if (isSliding && Mathf.Abs(rb.velocity.x) > slideThreshold)
        {
            float baseVel = slideSpeed;
            if (currentPlatformRb != null)
                baseVel += currentPlatformRb.velocity.x;
            rb.velocity = new Vector2(baseVel, rb.velocity.y);
        }

        // — 3) Zıplama + zıplama sesi
        if (jumpRequest && canJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false;

            if (audioSource != null && jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
        }
        jumpRequest = false;
    }

    void LateUpdate()
    {
        if (isDead) return;

        // Flip & tilt
        if (Mathf.Abs(hInput) > 0.01f)
        {
            var ls = transform.localScale;
            ls.x = hInput > 0 ? Mathf.Abs(ls.x) : -Mathf.Abs(ls.x);
            transform.localScale = ls;
        }
        float tiltAngle = 15f;
        float tgtZ = -hInput * tiltAngle;
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0, 0, tgtZ),
            Time.deltaTime * 10f
        );
    }

    #region Dynamic Joystick
    void HandleDynamicJoystick()
    {
#if UNITY_EDITOR
        if (joystickTouchId == -1
            && Input.GetMouseButtonDown(0)
            && Input.mousePosition.x < Screen.width / 2
            && !EventSystem.current.IsPointerOverGameObject())
        {
            joystickTouchId = -2;
            SpawnJoystick(Input.mousePosition);
        }
        else if (joystickTouchId == -2)
        {
            if (Input.GetMouseButton(0) && Input.mousePosition.x < Screen.width / 2)
                UpdateJoystick(Input.mousePosition);
            else
                ResetJoystick();
        }
#endif
        foreach (var t in Input.touches)
        {
            if (joystickTouchId == -1
                && t.phase == TouchPhase.Began
                && t.position.x < Screen.width / 2
                && !EventSystem.current.IsPointerOverGameObject(t.fingerId))
            {
                joystickTouchId = t.fingerId;
                SpawnJoystick(t.position);
            }
            else if (t.fingerId == joystickTouchId)
            {
                if ((t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
                    && t.position.x < Screen.width / 2)
                    UpdateJoystick(t.position);
                else
                    ResetJoystick();
            }
        }
    }

    void SpawnJoystick(Vector2 screenPos)
    {
        var canvasRect = joystickCanvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 lp);
        joystickStartPos = lp;

        joystickBG = Instantiate(joystickBgPrefab, joystickCanvas.transform, false);
        joystickBG.anchorMin = joystickBG.anchorMax = Vector2.one * .5f;
        joystickBG.pivot = Vector2.one * .5f;
        joystickBG.anchoredPosition = lp;

        joystickHandle = Instantiate(joystickHandlePrefab, joystickCanvas.transform, false);
        joystickHandle.anchorMin = joystickHandle.anchorMax = Vector2.one * .5f;
        joystickHandle.pivot = Vector2.one * .5f;
        joystickHandle.anchoredPosition = lp;

        dynamicH = 0f;
    }

    void UpdateJoystick(Vector2 screenPos)
    {
        var canvasRect = joystickCanvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPos, null, out Vector2 lp);
        Vector2 delta = lp - joystickStartPos;
        if (delta.sqrMagnitude > joystickRadius * joystickRadius)
            delta = delta.normalized * joystickRadius;
        joystickHandle.anchoredPosition = joystickStartPos + delta;
        dynamicH = delta.x / joystickRadius;
    }

    void ResetJoystick()
    {
        if (joystickBG) Destroy(joystickBG.gameObject);
        if (joystickHandle) Destroy(joystickHandle.gameObject);
        joystickBG = joystickHandle = null;
        joystickTouchId = -1;
        dynamicH = 0f;
    }
    #endregion

    void OnCollisionEnter2D(Collision2D col)
    {
        // Zıplama hakkı & hareketli platform tespiti
        bool landed = false;
        foreach (var c in col.contacts)
            if (c.normal.y > 0.5f) landed = true;
        if (landed)
        {
            canJump = true;
            if (col.collider.CompareTag(movingPlatformTag))
                currentPlatformRb = col.collider.attachedRigidbody;
        }

        // Slippery
        if (col.collider.CompareTag("Slippery"))
        {
            isSliding = true;
            slideSpeed = rb.velocity.x;
            slipDustEffect?.Play();
            slipSoundEffect?.Play();
        }

        // Kırılabilir zemin
        col.collider.GetComponent<BreakableTerrain>()?.Break();
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.collider.CompareTag(movingPlatformTag)
            && col.collider.attachedRigidbody == currentPlatformRb)
            currentPlatformRb = null;

        if (col.collider.CompareTag("Slippery"))
        {
            isSliding = false;
            slipDustEffect?.Stop();
        }
    }

    public void TakeDamage(float dmg)
    {
        if (isInvincible || isDead) return;
        isInvincible = true;
        currentHealth = Mathf.Max(currentHealth - Mathf.CeilToInt(dmg), 0);
        audioSource?.PlayOneShot(damageSound);
        CameraShake.Instance?.Shake(3f, 0.2f);
        StartCoroutine(DamageRoutine());
    }

    IEnumerator DamageRoutine()
    {
        var orig = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        if (currentHealth <= 0)
        {
            isDead = true;
            audioSource?.PlayOneShot(deathSound);
            GameManager.Instance.GameOver();
            yield break;
        }
        GameManager.Instance.RespawnPlayer();
        sr.color = new Color(orig.r, orig.g, orig.b, 0.5f);
        yield return new WaitForSeconds(invincibilityDuration);
        sr.color = orig;
        isInvincible = false;
        physMat.friction = friction;
    }

    public void OnRespawn()
    {
        rb.velocity = Vector2.zero;
        isDead = isInvincible = false;
        sr.color = Color.white;
        physMat.friction = friction;
    }

    public void Bounce(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    void ApplySkin(int skinID)
    {
        if (skinSprites == null || skinSprites.Length == 0) return;
        if (skinID < 0 || skinID >= skinSprites.Length) skinID = 0;
        sr.sprite = skinSprites[skinID];
    }
}
