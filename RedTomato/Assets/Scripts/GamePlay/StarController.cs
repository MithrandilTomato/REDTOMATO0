using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class StarController : MonoBehaviour
{
    [Header("UI Uçuş Ayarları")]
    public Canvas uiCanvas;                   // Screen Space–Overlay Canvas
    public GameObject flyingUIPrefab;         // UI Image prefab (RectTransform + Image)
    public RectTransform counterTarget;       // Sayaç ikonunun RectTransform’u
    public float flyDuration = 0.5f;          // Animasyon süresi

    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;
        collected = true;

        // 1) Dünya objesini ANINDA gizle/kazılmaz yap
        var sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;
        GetComponent<Collider2D>().enabled = false;

        // 2) Sayaç artışı
        GameManager.Instance.AddStars(1);

        // 3) UI ikonu spawn et
        Vector2 screenStart = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 screenTarget = counterTarget.position; // Overlay Canvas’da doğrudan ekran pozisyonu

        GameObject icon = Instantiate(flyingUIPrefab, uiCanvas.transform, false);
        var rt = icon.GetComponent<RectTransform>();
        rt.position = screenStart;

        // 4) Coroutine ile uçuş animasyonunu başlat
        StartCoroutine(AnimateFlyAndCleanup(rt, screenStart, screenTarget));
    }

    private IEnumerator AnimateFlyAndCleanup(RectTransform rt, Vector2 from, Vector2 to)
    {
        float elapsed = 0f;
        while (elapsed < flyDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / flyDuration);
            rt.position = Vector2.Lerp(from, to, t);
            yield return null;
        }

        // Animasyon bitince önce UI ikonunu, sonra da dünya objesini sil
        Destroy(rt.gameObject);
        Destroy(gameObject);
    }
}
