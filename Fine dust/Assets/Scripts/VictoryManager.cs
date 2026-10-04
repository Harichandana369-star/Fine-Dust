using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Required for TextMeshPro

public class VictoryManager : MonoBehaviour
{
    public static VictoryManager Instance;

    [Header("UI Reference")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText; // Assign your "LEVEL COMPLETED" TMP text here

    [Header("Color Changing Settings")]
    public Color[] rainbowColors = new Color[]
    {
        Color.red,
        Color.yellow,
        Color.green,
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f) // Orange
    };
    public float colorChangeInterval = 0.1f; // Changes color every 0.1 seconds

    [Header("HUD Elements to Hide on Victory")]
    public GameObject[] gameplayHUD;

    [Header("Audio")]
    public AudioClip victorySFX;

    private Coroutine colorCoroutine;

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
        // 1. Stop background music & play victory SFX
        StopBackgroundMusic();
        PlayVictorySound();

        // 2. Hide all gameplay HUD elements
        if (gameplayHUD != null)
        {
            foreach (GameObject hudElement in gameplayHUD)
            {
                if (hudElement != null)
                    hudElement.SetActive(false);
            }
        }

        // 3. Show Victory Panel
        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        // 4. Start colorful text animation (Works even when Time.timeScale = 0!)
        if (victoryText != null && rainbowColors.Length > 0)
        {
            if (colorCoroutine != null) StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(CycleTextColor());
        }

        // 5. Freeze gameplay physics
        Time.timeScale = 0f;
    }

    private IEnumerator CycleTextColor()
    {
        int colorIndex = 0;

        while (true)
        {
            victoryText.color = rainbowColors[colorIndex];
            colorIndex = (colorIndex + 1) % rainbowColors.Length;

            // Use WaitForSecondsRealtime so it continues animating when game is paused (Time.timeScale = 0)
            yield return new WaitForSecondsRealtime(colorChangeInterval);
        }
    }

    private void StopBackgroundMusic()
    {
        if (AudioManager.Instance != null)
        {
            if (AudioManager.Instance.musicSource != null)
            {
                AudioManager.Instance.musicSource.Stop();
            }
            else
            {
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
