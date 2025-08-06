using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StompTrigger : MonoBehaviour
{
    private Enemy_Walker enemy;

    void Awake()
    {
        // Parent�taki Enemy_Walker script�ini bul
        enemy = GetComponentInParent<Enemy_Walker>();
        if (enemy == null)
            Debug.LogError("StompTrigger parent'�nda Enemy_Walker bulunamad�!", this);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Sadece Player tag�li objeyle ilgilen
        if (!col.CompareTag("Player")) return;

        // D��man zaten �l�me gitti mi?
        if (enemy.IsDying) return;

        // Stomp: player�� z�plat ve d��man� �ld�r
        Rigidbody2D playerRb = col.attachedRigidbody;
        enemy.HandleStomp(playerRb);
    }
}
