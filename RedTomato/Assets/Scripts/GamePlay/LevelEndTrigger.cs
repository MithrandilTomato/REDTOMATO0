using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class LevelEndTrigger : MonoBehaviour
{
    [Header("Level Transition")]
    [Tooltip("Geçiþ yapmak istediðin sahnenin adý (Build Settings'teki adýyla birebir olmalý).")]
    public string nextSceneName = "Level2";

    [Header("Unlock Settings")]
    [Tooltip("Bu trigger tetiklendiðinde açýlacak level numarasý (1-based).")]
    public int nextLevelNumber = 2;

    [Header("Player Tag")]
    [Tooltip("Player GameObject'i üzerine atanmýþ Tag.")]
    public string playerTag = "Player";

    private void Reset()
    {
        // Eðer Collider2D yoksa ekle, varsa da trigger yap
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        // --- 1) PlayerPrefs'te saklanan en yüksek açýlan level'i güncelle ---
        // (Varsayýlan 1; eðer nextLevelNumber daha büyükse onu kaydet)
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (nextLevelNumber > unlocked)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevelNumber);
            PlayerPrefs.Save();
            Debug.Log($"LevelEndTrigger: UnlockedLevel = {nextLevelNumber}");
        }

        // --- 2) Bir sonraki sahneyi yükle ---
        Debug.Log($"LevelEndTrigger: Loading scene '{nextSceneName}'");
        SceneManager.LoadScene(nextSceneName);
    }
}
