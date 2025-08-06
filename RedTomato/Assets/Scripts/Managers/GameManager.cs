using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    [Tooltip("Game Over Panel (UI Panel GameObject under GameManager)")]
    public GameObject gameOverPanel;

    // Player��n ba�lang�� ve en son checkpoint pozisyonlar�
    private Vector3 startingPosition;
    private Vector3 lastCheckpoint;
    private bool isGameOver;

    // Toplanan y�ld�z say�s�
    private int totalStars;
    public int TotalStars => totalStars;

    // K�r�lan zeminin prefab�� ve konum bilgisi
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
                Debug.LogError("GameManager: Game Over Panel referans� atanmam��!");
            else
                gameOverPanel.SetActive(false);

            // Player��n ba�lang�� pozunu kaydet
            CachePlayerSpawn();

            // Sahne y�klendi�inde reset i�lemlerini yap
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Kay�tl� y�ld�z say�s�n� y�kle ve UI�� g�ncelle
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

        // 2) �nceki k�r�lan zemin verisini temizle
        brokenSinceCheckpoint.Clear();

        // 3) Player��n o sahnedeki pozunu cache�le
        CachePlayerSpawn();

        // 4) GameOver flag�ini s�f�rla
        isGameOver = false;

        // 5) Y�ld�z UI��n� g�ncelle
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
    /// BreakableTerrain taraf�ndan, zemin k�r�ld���nda �a�r�l�r.
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
    /// Checkpoint�e dokunuldu�unda �a�r�l�r.
    /// </summary>
    public void SetCheckpoint(Vector3 pos)
    {
        lastCheckpoint = pos;
        brokenSinceCheckpoint.Clear();
        Debug.Log($"Checkpoint set at {pos}");
    }

    /// <summary>
    /// Oyuncuyu en son checkpoint�e d�nd�r�r ve k�r�lan zemini yeniden olu�turur.
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
                Debug.LogWarning("GameManager: brokenSinceCheckpoint i�indeki prefab null, atlan�yor.");
        }
        brokenSinceCheckpoint.Clear();
    }

    /// <summary>
    /// Oyunu Game Over durumuna ge�irir.
    /// </summary>
    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Time.timeScale = 0f;
        gameOverPanel?.SetActive(true);
    }

    /// <summary>
    /// Level�� yeniden ba�lat�r ve checkpoint�i ba�lang�ca d�nd�r�r.
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
    /// Uygulamay� kapat�r.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    // ------ Y�ld�z Y�netimi ------

    /// <summary>
    /// Y�ld�z ekler, kaydeder ve UI�� g�nceller.
    /// </summary>
    public void AddStars(int amount)
    {
        totalStars += amount;
        PlayerPrefs.SetInt("TotalStars", totalStars);
        PlayerPrefs.Save();
        StarsUI.Instance?.UpdateStarCount(totalStars);
    }

    /// <summary>
    /// Y�ld�z harcamay� dener; ba�ar�l�ysa true d�ner.
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
