using System.Collections;
using UnityEngine;
using TMPro;

public class StarsUI : MonoBehaviour
{
    public static StarsUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private TextMeshProUGUI starText; // Atamak için Inspector'da sürükle-bırak yapın

    void Awake()
    {
        // Singleton kurulum
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Eğer Inspector'dan atamadıysanız, child objelerde ara
        if (starText == null)
        {
            starText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (starText == null)
        {
            Debug.LogError("StarsUI: starText atanmamış! Inspector'da veya child objede TMP Text atayın.");
        }
    }

    /// <summary>
    /// Yıldız sayısı değiştiğinde UI'ı günceller. 0.5 saniyelik gecikme ile.
    /// </summary>
    public void UpdateStarCount(int count)
    {
        if (starText == null)
        {
            Debug.LogWarning("StarsUI: UpdateStarCount çağrıldı ama starText atanmadı.");
            return;
        }

        // 0.5 saniye gecikmeli güncelleme
        StartCoroutine(DelayedUpdate(count));
    }

    private IEnumerator DelayedUpdate(int count)
    {
        yield return new WaitForSeconds(0.5f);
        starText.text = count.ToString();
    }
}
