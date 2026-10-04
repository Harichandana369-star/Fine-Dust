using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Reference")]
    public GameObject pauseMenuPanel;

    [Header("HUD Elements to Hide While Paused")]
    public GameObject[] gameplayHUD; // Assign PlayerPortrait, LineUI, etc. here

    private bool isPaused = false;

    private void Awake()
    {
        // Bind instance to current scene's PauseManager
        Instance = this;
    }

    private void Start()
    {
        // Guarantee time is running and menu is hidden when level starts
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Toggle pause when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        // Hide gameplay HUD elements (Portrait, Chalk Mass Bar, etc.)
        if (gameplayHUD != null)
        {
            foreach (GameObject hud in gameplayHUD)
            {
                if (hud != null) hud.SetActive(false);
            }
        }

        // Show Pause Panel
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        Time.timeScale = 0f; // Freeze game physics, movement, and animations
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Hide Pause Panel
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Restore gameplay HUD elements
        if (gameplayHUD != null)
        {
            foreach (GameObject hud in gameplayHUD)
            {
                if (hud != null) hud.SetActive(true);
            }
        }

        Time.timeScale = 1f; // Resume gameplay
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Always reset timeScale before changing scenes!
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Always reset timeScale before returning to menu!

        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.LoadSceneByName("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}