using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class Enemy_Walker : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Collision")]
    public float damage = 1f;
    public float bounceForce = 10f;

    Rigidbody2D rb;
    Animator animator;
    Collider2D col2d;
    SpriteRenderer sr;

    bool movingRight = true;
    bool isDying = false;

    public bool IsDying => isDying;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col2d = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDying) return;

        // Ping-pong hareket
        if (movingRight)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            if (transform.position.x >= rightPoint.position.x)
                Flip();
        }
        else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            if (transform.position.x <= leftPoint.position.x)
                Flip();
        }
    }

    /// <summary>
    /// Yalnızca sprite’ı yatay çevirir, gözlerin de yön değiştirir.
    /// </summary>
    void Flip()
    {
        movingRight = !movingRight;
        // Eğer sağa gidiyorsa flipX = false, sola gidiyorsa flipX = true
        sr.flipX = !movingRight;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (isDying) return;
        if (!col.collider.CompareTag("Player")) return;

        // Yan temasta hasar
        var pc = col.collider.GetComponentInParent<PlayerController>();
        pc?.TakeDamage(damage);
    }

    public void HandleStomp(Rigidbody2D playerRb)
    {
        if (isDying) return;
        isDying = true;

        // 1) player zıplat
        if (playerRb != null)
            playerRb.velocity = new Vector2(playerRb.velocity.x, bounceForce);

        // 2) hareket ve collider kapat
        rb.velocity = Vector2.zero;
        col2d.enabled = false;

        // 3) ölüm animasyonu
        animator.SetTrigger("Die");

        // 4) kısa süre bekle ve yok et
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}
