using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeDamage : MonoBehaviour
{
    [Tooltip("Bir temasta verilecek hasar miktarý")]
    public float damage = 1f;

    // Eðer oyuncu ile temas edince belirli bir süre sonra yeniden hasar vermek istersen:
    [Tooltip("Tekrar hasar verebilmesi için bekleme süresi (saniye)")]
    public float cooldown = 0.5f;

    private float lastHitTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // Eðer cooldown süresi içindeyse tekrar vurma
        if (Time.time - lastHitTime >= cooldown)
            TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        // PlayerController varsa hasar ver
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
            lastHitTime = Time.time;
        }
    }
}
