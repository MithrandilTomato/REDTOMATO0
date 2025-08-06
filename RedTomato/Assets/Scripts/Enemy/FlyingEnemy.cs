using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float speed = 2f;
    private int currentWP = 0;

    [Header("Drop Projectile")]
    public GameObject projectilePrefab;
    public float dropInterval = 2f;
    private float dropTimer;

    void Start()
    {
        dropTimer = dropInterval;
    }

    void Update()
    {
        Patrol();
        TryDrop();
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        // Hedef waypoint'e do�ru hareket et
        Transform target = waypoints[currentWP];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        // Hedefe ula�t�ysak s�radaki waypoint
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
            currentWP = (currentWP + 1) % waypoints.Length;
    }

    private void TryDrop()
    {
        dropTimer -= Time.deltaTime;
        if (dropTimer <= 0f)
        {
            Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity
            );
            dropTimer = dropInterval;
        }
    }

    // Opsiyonel g�rsel flip
    void LateUpdate()
    {
        if (waypoints.Length < 2) return;
        // hareket y�n�ne g�re d�n
        float dir = waypoints[currentWP].position.x - transform.position.x;
        GetComponent<SpriteRenderer>().flipX = dir < 0;
    }
}
