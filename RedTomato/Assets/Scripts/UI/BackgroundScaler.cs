using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundScaler : MonoBehaviour
{
    void Start()
    {
        // 1) SpriteRenderer elde et
        var sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        // 2) Kamera dünya boyutunu hesapla
        float worldHeight = Camera.main.orthographicSize * 2f;
        float worldWidth = worldHeight * Camera.main.aspect;

        // 3) Sprite'ýn birim boyu (bounds) al
        Vector2 spriteSize = sr.sprite.bounds.size;

        // 4) Scale’i ayarla
        transform.localScale = new Vector3(
            worldWidth / spriteSize.x,
            worldHeight / spriteSize.y,
            1f
        );

        // ---- YENÝ EKLENDÝ ----
        // 5) Pozisyonu (0,0,0) yap
        transform.position = new Vector3(
            Camera.main.transform.position.x,
            Camera.main.transform.position.y,
            transform.position.z
        );
    }
}
