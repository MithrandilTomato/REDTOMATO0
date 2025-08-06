using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [Header("Default Music (tüm seviyeler)")]
    public AudioClip defaultClip;

    [Header("Seviye Bazlý Özel Müzikler")]
    public List<LevelMusic> levelMusics;

    AudioSource _source;
    string _currentScene;

    [System.Serializable]
    public struct LevelMusic
    {
        public string sceneName;  // örn. "Level7"
        public AudioClip clip;      // o seviye için çalýnacak müzik
    }

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        _source.playOnAwake = false;

        // Aktif MusicPlayer instance’larýný bulup, ikincisini yok et
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

        // Sahne yüklendiðinde OnSceneLoaded tetiklesin
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _currentScene = scene.name;

        // Eðer levelMusics listesinde bu sahne için bir taným varsa onu çal, yoksa default
        var lm = levelMusics.Find(x => x.sceneName == _currentScene);
        PlayMusic(lm.clip != null ? lm.clip : defaultClip);
    }

    void PlayMusic(AudioClip clip)
    {
        if (_source.clip == clip) return;  // zaten o diyorsa baþlamasýn
        _source.clip = clip;
        _source.Play();
    }

    void OnDestroy()
    {
        // Dinleyiciyi temizleyelim
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
