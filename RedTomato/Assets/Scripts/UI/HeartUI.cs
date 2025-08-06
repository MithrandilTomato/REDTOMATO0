using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Kalplerin yerleþtirileceði boþ panel (örneðin Horizontal Layout Group altýnda)")]
    public Transform healthPanel;
    [Tooltip("Kalp prefab’ý (Image komponenti olmalý)")]
    public GameObject heartPrefab;
    [Tooltip("Dolu kalp sprite’ý")]
    public Sprite fullHeartSprite;
    [Tooltip("Boþ kalp sprite’ý")]
    public Sprite emptyHeartSprite;

    private PlayerController playerController;
    private List<Image> hearts = new List<Image>();

    void Start()
    {
        // referans kontrolü
        if (healthPanel == null)
        {
            Debug.LogError("HeartUI: healthPanel atanmamýþ!", this);
            enabled = false;
            return;
        }
        if (heartPrefab == null)
        {
            Debug.LogError("HeartUI: heartPrefab atanmamýþ!", this);
            enabled = false;
            return;
        }
        if (fullHeartSprite == null || emptyHeartSprite == null)
        {
            Debug.LogError("HeartUI: fullHeartSprite veya emptyHeartSprite atanmamýþ!", this);
            enabled = false;
            return;
        }

        // PlayerController bul
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null)
        {
            Debug.LogError("HeartUI: Tag'i 'Player' olan obje bulunamadý!", this);
            enabled = false;
            return;
        }
        playerController = go.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("HeartUI: PlayerController component’i bulunamadý!", go);
            enabled = false;
            return;
        }

        // UI panel altýndakileri temizle ve kalp ikonlarýný yarat
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        // Önce varsa eski ikonlarý temizle
        for (int i = healthPanel.childCount - 1; i >= 0; i--)
            Destroy(healthPanel.GetChild(i).gameObject);

        hearts.Clear();

        // maxHealth kadar prefab instantiate et
        for (int i = 0; i < playerController.MaxHealth; i++)
        {
            var heartGO = Instantiate(heartPrefab, healthPanel);
            var img = heartGO.GetComponent<Image>();
            if (img == null)
            {
                Debug.LogError("HeartUI: heartPrefab içinde Image komponenti yok!", heartGO);
                continue;
            }
            hearts.Add(img);
        }
    }

    void Update()
    {
        // her frame can sayýsýna göre doldur veya boþalt
        int current = playerController.CurrentHealth;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = (i < current) ? fullHeartSprite : emptyHeartSprite;
        }
    }
}
