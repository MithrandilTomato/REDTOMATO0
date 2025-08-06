using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider2D))]
public class KillZone : MonoBehaviour
{
    [Header("Damage Settings")]
    [Tooltip("Player bu alana girdiğinde ne kadar can eksilsin?")]
    public float damage = 1f;
    [Tooltip("Hasar verildikten sonra aynı oyuncuya tekrar hasar verilmeyecek süre (saniye)")]
    public float damageCooldown = 1f;

    // Player instanceID → en son hasar zamanı
    private Dictionary<int, float> _lastDamageTime = new Dictionary<int, float>();

    void Awake()
    {
        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        int id = other.gameObject.GetInstanceID();
        float now = Time.time;

        // Eğer cooldown içinde isek çık
        if (_lastDamageTime.TryGetValue(id, out float lastTime)
            && now - lastTime < damageCooldown)
        {
            return;
        }

        // Hasar uygulama
        var pc = other.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(damage);
            // timestamp güncelle
            _lastDamageTime[id] = now;
        }
    }
}
