using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpikeDamage : MonoBehaviour
{
    [Tooltip("Bir temasta verilecek hasar miktar�")]
    public float damage = 1f;

    // E�er oyuncu ile temas edince belirli bir s�re sonra yeniden hasar vermek istersen:
    [Tooltip("Tekrar hasar verebilmesi i�in bekleme s�resi (saniye)")]
    public float cooldown = 0.5f;

    private float lastHitTime;

    void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // E�er cooldown s�resi i�indeyse tekrar vurma
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
