using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class WormEnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    [Tooltip("Sol limit noktası")]
    public Transform pointA;
    [Tooltip("Sağ limit noktası")]
    public Transform pointB;
    [Tooltip("Hedefe ne kadar yakınsam flip yapayım")]
    public float flipThreshold = 0.05f;

    [Header("Combat")]
    public float damage = 1f;
    public float bounceForce = 10f;

    [Header("Audio")]
    [Tooltip("Ölme sesi için AudioSource")]
    public AudioSource audioSource;
    [Tooltip("Ölme sesi clip’i")]
    public AudioClip deathSound;

    [Header("Death")]
    [Tooltip("Ölü sprite’ı")]
    public Sprite deathSprite;
    [Tooltip("Ölümden sonra ne kadar beklesin")]
    public float deathDelay = 1f;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Collider2D bodyCollider;
    Collider2D headTrigger;
    bool movingToB = true;
    bool isDying = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.freezeRotation = true;

        sr = GetComponent<SpriteRenderer>();

        // Collider’ları ayıkla: trigger olan head, diğeri body
        foreach (var c in GetComponents<Collider2D>())
        {
            if (c.isTrigger) headTrigger = c;
            else bodyCollider = c;
        }
    }

    void FixedUpdate()
    {
        if (isDying) return;

        // Hedef pozisyonu seç
        Vector2 target = movingToB
            ? (Vector2)pointB.position
            : (Vector2)pointA.position;
        Vector2 current = rb.position;

        // Bir sonraki pozisyonu hesapla
        float step = speed * Time.fixedDeltaTime;
        Vector2 next = Vector2.MoveTowards(current, target, step);

        // Taşı
        rb.MovePosition(next);

        // Hedefe ulaşınca yön değiştir
        if (Vector2.Distance(next, target) < flipThreshold)
            movingToB = !movingToB;

        // Sprite + collider flip
        var ls = transform.localScale;
        ls.x = movingToB
            ? Mathf.Abs(ls.x)
            : -Mathf.Abs(ls.x);
        transform.localScale = ls;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDying) return;
        if (col.collider.CompareTag("Player"))
        {
            col.collider.GetComponent<PlayerController>()?.TakeDamage(damage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDying || !other.CompareTag("Player")) return;

        // Player’ı zıplat
        var prb = other.attachedRigidbody;
        if (prb != null)
            prb.velocity = new Vector2(prb.velocity.x, bounceForce);

        // Ölüm sürecini başlat
        Die();
    }

    /// <summary>
    /// Dışarıdan da çağrılabilir olacak Die metodu
    /// </summary>
    public void Die()
    {
        if (!isDying)
            StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        isDying = true;
        rb.velocity = Vector2.zero;
        bodyCollider.enabled = false;
        if (headTrigger != null) headTrigger.enabled = false;

        // Ölme sesi
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        // Ölüm sprite’ı
        if (deathSprite != null)
            sr.sprite = deathSprite;

        // Bir süre bekle, sonra nesneyi sil
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }
}
