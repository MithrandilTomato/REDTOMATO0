using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Kalplerin yerle�tirilece�i bo� panel (�rne�in Horizontal Layout Group alt�nda)")]
    public Transform healthPanel;
    [Tooltip("Kalp prefab�� (Image komponenti olmal�)")]
    public GameObject heartPrefab;
    [Tooltip("Dolu kalp sprite��")]
    public Sprite fullHeartSprite;
    [Tooltip("Bo� kalp sprite��")]
    public Sprite emptyHeartSprite;

    private PlayerController playerController;
    private List<Image> hearts = new List<Image>();

    void Start()
    {
        // referans kontrol�
        if (healthPanel == null)
        {
            Debug.LogError("HeartUI: healthPanel atanmam��!", this);
            enabled = false;
            return;
        }
        if (heartPrefab == null)
        {
            Debug.LogError("HeartUI: heartPrefab atanmam��!", this);
            enabled = false;
            return;
        }
        if (fullHeartSprite == null || emptyHeartSprite == null)
        {
            Debug.LogError("HeartUI: fullHeartSprite veya emptyHeartSprite atanmam��!", this);
            enabled = false;
            return;
        }

        // PlayerController bul
        var go = GameObject.FindGameObjectWithTag("Player");
        if (go == null)
        {
            Debug.LogError("HeartUI: Tag'i 'Player' olan obje bulunamad�!", this);
            enabled = false;
            return;
        }
        playerController = go.GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("HeartUI: PlayerController component�i bulunamad�!", go);
            enabled = false;
            return;
        }

        // UI panel alt�ndakileri temizle ve kalp ikonlar�n� yarat
        InitializeHearts();
    }

    private void InitializeHearts()
    {
        // �nce varsa eski ikonlar� temizle
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
                Debug.LogError("HeartUI: heartPrefab i�inde Image komponenti yok!", heartGO);
                continue;
            }
            hearts.Add(img);
        }
    }

    void Update()
    {
        // her frame can say�s�na g�re doldur veya bo�alt
        int current = playerController.CurrentHealth;

        for (int i = 0; i < hearts.Count; i++)
        {
            if (hearts[i] != null)
                hearts[i].sprite = (i < current) ? fullHeartSprite : emptyHeartSprite;
        }
    }
}
