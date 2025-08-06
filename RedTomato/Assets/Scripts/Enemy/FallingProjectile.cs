using UnityEngine;

public class FallingProjectile : MonoBehaviour
{
    [Tooltip("Düþme hýzý")]
    public float fallSpeed = 5f;
    [Tooltip("Temasta verilecek hasar")]
    public float damage = 1f;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Eðer çarpýlan nesne bir Enemy ise görmezden gel
        if (other.CompareTag("Enemy"))
            return;

        // 2) Player ile temas ettiyse hasar ver
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(damage);
        }

        // 3) Enemy deðilse (terrain, duvar, player vs.) kendini yok et
        Destroy(gameObject);
    }

    void Start()
    {
        Destroy(gameObject, 10f);
    }
}
