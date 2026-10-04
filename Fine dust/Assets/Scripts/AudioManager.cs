using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Chalk & Movement SFX")]
    public AudioClip drawLineSFX;
    public AudioClip jumpSFX;

    [Header("Math Enemy SFX")]
    public AudioClip minusEnemySFX;
    public AudioClip plusEnemySFX;
    public AudioClip variableEnemySFX;

    [Header("Game State SFX")]
    public AudioClip respawnSFX;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Subscribe to scene load event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from scene load event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        PlayBackgroundMusicFromStart();
    }

    // Called automatically every time ANY scene loads
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBackgroundMusicFromStart();
    }

    public void PlayBackgroundMusicFromStart()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.time = 0f; // Reset track time to 0 seconds
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void PlayLoopingSFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            if (sfxSource.clip != clip || !sfxSource.isPlaying)
            {
                sfxSource.clip = clip;
                sfxSource.loop = true;
                sfxSource.Play();
            }
        }
    }

    public void StopLoopingSFX()
    {
        if (sfxSource != null && sfxSource.loop)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
        }
    }
}