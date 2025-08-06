using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [Header("Default Music (t�m seviyeler)")]
    public AudioClip defaultClip;

    [Header("Seviye Bazl� �zel M�zikler")]
    public List<LevelMusic> levelMusics;

    AudioSource _source;
    string _currentScene;

    [System.Serializable]
    public struct LevelMusic
    {
        public string sceneName;  // �rn. "Level7"
        public AudioClip clip;      // o seviye i�in �al�nacak m�zik
    }

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;

        // Aktif MusicPlayer instance�lar�n� bulup, ikincisini yok et
        var players = Object.FindObjectsByType<MusicPlayer>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );
        if (players.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Sahne y�klendi�inde OnSceneLoaded tetiklesin
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;

        // E�er levelMusics listesinde bu sahne i�in bir tan�m varsa onu �al, yoksa default
        var lm = levelMusics.Find(x => x.sceneName == _currentScene);
        PlayMusic(lm.clip != null ? lm.clip : defaultClip);
    }

    void PlayMusic(AudioClip clip)
    {
        if (_source.clip == clip) return;  // zaten o diyorsa ba�lamas�n
        _source.clip = clip;
        _source.Play();
    }

    void OnDestroy()
    {
        // Dinleyiciyi temizleyelim
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
