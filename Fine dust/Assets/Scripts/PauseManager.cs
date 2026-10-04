using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI Reference")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private void Awake()
    {
        // Re-bind Instance to current scene's PauseManager
        Instance = this;
    }

    private void Start()
    {
        // Guarantee the game is unpaused when any scene starts
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
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        Time.timeScale = 0f; // Freezes gameplay time, physics, and movement
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        Time.timeScale = 1f; // Resumes gameplay time
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; // Reset timeScale before reloading
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Reset timeScale before returning to menu

        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.LoadSceneByName("MainMenu");
        }
        else
        {
            SceneManager.LoadScene(0); // Reset to Scene 0 if transition manager is absent
        }
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}