using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;      // OptionsPanel objesi
    public Slider volumeSlider;
    public Toggle muteToggle;

    [Header("Scene Names")]
    public string mainMenuSceneName = "MainMenu";

    float _savedVolume;

    void Awake()
    {
        // Panel kapalý baþlasýn
        panel.SetActive(false);

        // Önceki ayarlarý yükle
        float vol = PlayerPrefs.GetFloat("MasterVolume", 1f);
        bool muted = PlayerPrefs.GetInt("Muted", 0) == 1;

        volumeSlider.value = vol;
        muteToggle.isOn = muted;
        ApplyVolume(vol, muted);

        // Dinleyicileri ekle
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggled);
    }

    void OnVolumeChanged(float value)
    {
        if (!muteToggle.isOn)
            ApplyVolume(value, false);
    }

    void OnMuteToggled(bool isMuted)
    {
        ApplyVolume(volumeSlider.value, isMuted);
    }

    void ApplyVolume(float vol, bool isMuted)
    {
        AudioListener.volume = isMuted ? 0f : vol;
        PlayerPrefs.SetFloat("MasterVolume", vol);
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Açma/kapama metotlarýný koroya alýyoruz:

    public void OpenOptions() => StartCoroutine(EnableNextFrame(true));
    public void CloseOptions() => StartCoroutine(EnableNextFrame(false));

    private IEnumerator EnableNextFrame(bool open)
    {
        // Mevcut týklama olayýný bitir:
        yield return new WaitForEndOfFrame();
        panel.SetActive(open);
    }

    // Ana menüye dön:
    public void BackToMainMenu()
    {
        CloseOptions();
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void OpenLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
    public void OpenMarket()
    {
        SceneManager.LoadScene("LevelMarket");
    }
}
