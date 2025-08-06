using UnityEngine;

public class CheckpointFlag : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            GameManager.Instance.SetCheckpoint(transform.position);
            // dilersen burada bir animasyon veya ses çalabilirsin
        }
    }
}
