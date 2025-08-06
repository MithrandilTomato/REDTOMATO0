using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level Transition")]
    [Tooltip("Ge�i� yapmak istedi�in sahnenin ad� (Build Settings'teki ad�yla birebir olmal�).")]
    public string nextSceneName = "Level2";

    [Header("Unlock Settings")]
    [Tooltip("Bu trigger tetiklendi�inde a��lacak level numaras� (1-based).")]
    public int nextLevelNumber = 2;

    [Header("Player Tag")]
    [Tooltip("Player GameObject'i �zerine atanm�� Tag.")]
    public string playerTag = "Player";

    private void Reset()
    {
        // E�er Collider2D yoksa ekle, varsa da trigger yap
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // --- 1) PlayerPrefs'te saklanan en y�ksek a��lan level'i g�ncelle ---
        // (Varsay�lan 1; e�er nextLevelNumber daha b�y�kse onu kaydet)
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (nextLevelNumber > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevelNumber);
            PlayerPrefs.Save();
            Debug.Log($"LevelEndTrigger: UnlockedLevel = {nextLevelNumber}");
        }

        // --- 2) Bir sonraki sahneyi y�kle ---
        Debug.Log($"LevelEndTrigger: Loading scene '{nextSceneName}'");
        SceneManager.LoadScene(nextSceneName);
    }
}
