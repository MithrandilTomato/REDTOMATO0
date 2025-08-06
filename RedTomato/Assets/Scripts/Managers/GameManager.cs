using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Game Over Panel (UI Panel GameObject under GameManager)")]
    public GameObject gameOverPanel;

    // Player’ýn baþlangýç ve en son checkpoint pozisyonlarý
    private Vector3 startingPosition;
    private Vector3 lastCheckpoint;
    private bool isGameOver;

    // Toplanan yýldýz sayýsý
    private int totalStars;
    public int TotalStars => totalStars;

    // Kýrýlan zeminin prefab’ý ve konum bilgisi
    private struct BrokenData
    {
        public GameObject prefab;
        public Vector3 position;
        public Quaternion rotation;
    }
    private List<BrokenData> brokenSinceCheckpoint = new List<BrokenData>();

    private void Awake()
    {
        // Singleton + persist
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (gameOverPanel == null)
                Debug.LogError("GameManager: Game Over Panel referansý atanmamýþ!");
            else
                gameOverPanel.SetActive(false);

            // Player’ýn baþlangýç pozunu kaydet
            CachePlayerSpawn();

            // Sahne yüklendiðinde reset iþlemlerini yap
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Kayýtlý yýldýz sayýsýný yükle ve UI’ý güncelle
            totalStars = PlayerPrefs.GetInt("TotalStars", 0);
            StarsUI.Instance?.UpdateStarCount(totalStars);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1) Game Over panelini kapat
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // 2) Önceki kýrýlan zemin verisini temizle
        brokenSinceCheckpoint.Clear();

        // 3) Player’ýn o sahnedeki pozunu cache’le
        CachePlayerSpawn();

        // 4) GameOver flag’ini sýfýrla
        isGameOver = false;

        // 5) Yýldýz UI’ýný güncelle
        StarsUI.Instance?.UpdateStarCount(totalStars);
    }

    private void CachePlayerSpawn()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            startingPosition = player.transform.position;
            lastCheckpoint = startingPosition;
        }
    }

    /// <summary>
    /// BreakableTerrain tarafýndan, zemin kýrýldýðýnda çaðrýlýr.
    /// </summary>
    public void RegisterBrokenTerrain(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        brokenSinceCheckpoint.Add(new BrokenData
        {
            prefab = prefab,
            position = pos,
            rotation = rot
        });
    }
    public void BackToMainMenu()
    {
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Checkpoint’e dokunulduðunda çaðrýlýr.
    /// </summary>
    public void SetCheckpoint(Vector3 pos)
    {
        lastCheckpoint = pos;
        brokenSinceCheckpoint.Clear();
        Debug.Log($"Checkpoint set at {pos}");
    }

    /// <summary>
    /// Oyuncuyu en son checkpoint’e döndürür ve kýrýlan zemini yeniden oluþturur.
    /// </summary>
    public void RespawnPlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = lastCheckpoint;
            player.GetComponent<PlayerController>()?.OnRespawn();
        }

        foreach (var data in brokenSinceCheckpoint)
        {
            if (data.prefab != null)
                Instantiate(data.prefab, data.position, data.rotation);
            else
                Debug.LogWarning("GameManager: brokenSinceCheckpoint içindeki prefab null, atlanýyor.");
        }
        brokenSinceCheckpoint.Clear();
    }

    /// <summary>
    /// Oyunu Game Over durumuna geçirir.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);
    }

    /// <summary>
    /// Level’ý yeniden baþlatýr ve checkpoint’i baþlangýca döndürür.
    /// </summary>
    public void RestartLevel()
    {
        isGameOver = false;
        lastCheckpoint = startingPosition;
        brokenSinceCheckpoint.Clear();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Uygulamayý kapatýr.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    // ------ Yýldýz Yönetimi ------

    /// <summary>
    /// Yýldýz ekler, kaydeder ve UI’ý günceller.
    /// </summary>
    public void AddStars(int amount)
    {
        totalStars += amount;
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();
        StarsUI.Instance?.UpdateStarCount(totalStars);
    }

    /// <summary>
    /// Yýldýz harcamayý dener; baþarýlýysa true döner.
    /// </summary>
    public bool SpendStars(int amount)
    {
        if (totalStars < amount) return false;
        totalStars -= amount;
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();
        StarsUI.Instance?.UpdateStarCount(totalStars);
        return true;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
