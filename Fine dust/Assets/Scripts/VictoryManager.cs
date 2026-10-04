using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("UI Reference")]
    public GameObject victoryPanel;

    [Header("HUD Elements to Hide on Victory")]
    public GameObject[] gameplayHUD; // Assign PlayerPortrait, LineUI, etc.

    [Header("Audio")]
    public AudioClip victorySFX;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        // 1. Stop background music
        StopBackgroundMusic();

        // 2. Play Victory SFX
        PlayVictorySound();

        // 3. Hide all gameplay HUD elements (Portrait, Chalk Mass Bar, etc.)
        if (gameplayHUD != null)
        {
            foreach (GameObject hudElement in gameplayHUD)
            {
                if (hudElement != null)
                    hudElement.SetActive(false);
            }
        }

        // 4. Show Victory Panel
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // 5. Freeze gameplay physics
        Time.timeScale = 0f;
    }

    private void StopBackgroundMusic()
    {
        if (AudioManager.Instance != null)
        {
            // If your AudioManager has a musicSource, stop or pause it
            if (AudioManager.Instance.musicSource != null)
            {
                AudioManager.Instance.musicSource.Stop();
            }
            else
            {
                // Fallback: Stop any AudioSource playing on the AudioManager object
                AudioSource musicSource = AudioManager.Instance.GetComponent<AudioSource>();
                if (musicSource != null)
                {
                    musicSource.Stop();
                }
            }
        }
    }

    private void PlayVictorySound()
    {
        if (AudioManager.Instance != null)
        {
            if (victorySFX != null)
            {
                AudioManager.Instance.PlaySFX(victorySFX);
            }
            else if (AudioManager.Instance.jumpSFX != null)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
            }
        }
        else if (victorySFX != null)
        {
            AudioSource.PlayClipAtPoint(victorySFX, Camera.main.transform.position);
        }
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
