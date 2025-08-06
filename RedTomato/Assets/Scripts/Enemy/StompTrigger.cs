using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StompTrigger : MonoBehaviour
{
    private Enemy_Walker enemy;

    void Awake()
    {
        // Parent’taki Enemy_Walker script’ini bul
        enemy = GetComponentInParent<Enemy_Walker>();
        if (enemy == null)
            Debug.LogError("StompTrigger parent'ýnda Enemy_Walker bulunamadý!", this);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Sadece Player tag’li objeyle ilgilen
        if (!col.CompareTag("Player")) return;

        // Düþman zaten ölüme gitti mi?
        if (enemy.IsDying) return;

        // Stomp: player’ý zýplat ve düþmaný öldür
        Rigidbody2D playerRb = col.attachedRigidbody;
        enemy.HandleStomp(playerRb);
    }
}
