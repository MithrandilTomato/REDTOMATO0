using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class BreakableTerrain : MonoBehaviour
{
    [Header("Break Settings")]
    [Tooltip("Kırılmış hali için kullanılacak sprite")]
    public Sprite crackedSprite;

    [Tooltip("Kırılma anında oynatılacak partikül efekt prefab'ı")]
    public GameObject effectPrefab;

    [Tooltip("Bu sahne örneği değil, prefab asset’i; opsiyonel bırakabilirsiniz.")]
    public GameObject terrainPrefabAsset;

    [Tooltip("Zeminin tamamen yok olması için bekleme süresi (saniye)")]
    public float destroyDelay = 0.5f;

    private SpriteRenderer sr;
    private Collider2D col2d;
    private bool hasBroken;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col2d = GetComponent<Collider2D>();

        if (terrainPrefabAsset == null)
        {
            // Resources/BreakableTerrains klasöründen adla yükle
            terrainPrefabAsset = Resources.Load<GameObject>(
                $"BreakableTerrains/{gameObject.name}"
            );
            if (terrainPrefabAsset == null)
                Debug.LogError($"Prefab atanmamış ve Resources’ta bulunamadı: {gameObject.name}");
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (hasBroken) return;
        if (col.collider.CompareTag("Player"))
            Break();
    }

    public void Break()
    {
        if (hasBroken) return;
        hasBroken = true;

        if (crackedSprite != null)
            sr.sprite = crackedSprite;

        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);

        // Efekt
        if (effectPrefab != null)
        {
            var fx = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            var ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Destroy(fx, 2f);
        }

        // Prefab asset referansını hazırla:
        GameObject prefabToRegister = terrainPrefabAsset;

        // Eğer inspector’da atanmamışsa Resources’tan dene:
        if (prefabToRegister == null)
        {
            // Prefab dosyanızın adı, bu GameObject’in adıyla aynı olmalı
            string prefabName = gameObject.name;
            prefabToRegister = Resources.Load<GameObject>(
                $"BreakableTerrains/{prefabName}"
            );
            if (prefabToRegister == null)
                Debug.LogWarning($"BreakableTerrain: Resources/BreakableTerrains/{prefabName} yüklenemedi.");
        }

        // Kayıt
        if (prefabToRegister != null && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterBrokenTerrain(
                prefabToRegister,
                transform.position,
                transform.rotation
            );
        }

        // Obje yok olsun
        Destroy(gameObject);
    }
}
