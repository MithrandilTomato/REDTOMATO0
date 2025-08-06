using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinButtonController : MonoBehaviour
{
    [Header("Skin Data")]
    public int skinID;
    public int price;

    [Header("UI References")]
    public Image previewImage;
    public TMP_Text priceText;     // ← TextMeshPro bileşeni
    public Button buyButton;
    public Button applyButton;     // ← yeni eklenen apply butonu
    public GameObject lockedOverlay;

    void Start()
    {
        // Fiyatı yaz
        priceText.text = price.ToString();

        // Sahibi mi kontrol et
        bool owned = PlayerPrefs.GetInt($"Skin_{skinID}", 0) == 1;

        // Kilit ikonu göster/gizle
        lockedOverlay.SetActive(!owned);
        // Buy ve Apply butonlarını show/hide et
        buyButton.gameObject.SetActive(!owned);
        applyButton.gameObject.SetActive(owned);

        // Listener’ları ekle
        buyButton.onClick.AddListener(OnBuy);
        applyButton.onClick.AddListener(OnApply);
    }

    void OnBuy()
    {
        // Yeterli yıldız var mı?
        if (!GameManager.Instance.SpendStars(price))
            return;

        // Satın alma bilgisi kaydet
        PlayerPrefs.SetInt($"Skin_{skinID}", 1);
        PlayerPrefs.Save();

        // Kilidi aç, butonları güncelle
        lockedOverlay.SetActive(false);
        buyButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(true);
    }

    void OnApply()
    {
        // Seçili skini uygula
        PlayerPrefs.SetInt("CurrentSkinID", skinID);
        PlayerPrefs.Save();
    }
}
