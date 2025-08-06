using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    public Button[] levelButtons;        // Inspector’da 10 button referansý
    public string levelSceneName = "Level"; // "Level1","Level2",...

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNo = i + 1;
            var btn = levelButtons[i];

            // kilidi kapat/aç
            bool open = levelNo <= unlocked;
            btn.interactable = open;
            var lockIcon = btn.transform.Find("LockIcon");
            if (lockIcon) lockIcon.gameObject.SetActive(!open);

            // onClick olayýný ata
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => LoadLevel(levelNo));
        }
    }

    public void LoadLevel(int levelNo)
    {
        SceneManager.LoadScene(levelSceneName + levelNo);
    }
}
