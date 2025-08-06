using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Play butonuna ba�l�
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Options panelini a��p kapatmak i�in (opsiyonel)
    public GameObject optionsPanel;
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    // Quit butonuna ba�l�
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OpenMarket()
    {
        SceneManager.LoadScene("LevelMarket");
    }

    // iste�e ba�l�: ana men�ye geri d�nen fonksiyon
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // Sahnenin ad�n� kendi ana men� sahnenin ad�yla de�i�tir
    }
}
