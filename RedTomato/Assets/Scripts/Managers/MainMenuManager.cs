using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Play butonuna baðlý
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    // Options panelini açýp kapatmak için (opsiyonel)
    public GameObject optionsPanel;
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    // Quit butonuna baðlý
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

    // isteðe baðlý: ana menüye geri dönen fonksiyon
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        // Sahnenin adýný kendi ana menü sahnenin adýyla deðiþtir
    }
}
