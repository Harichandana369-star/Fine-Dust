using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Canvas Panels")]
    public GameObject mainButtonsPanel;
    public GameObject infoPanel;

    [Header("Info Sub-Pages (Tutorial Steps)")]
    public GameObject[] infoSubPages; // Sub-page 0: Line Drawing | Sub-page 1: Math Enemies
    public GameObject prevButton;
    public GameObject nextButton;

    private int currentInfoPageIndex = 0;

    void Start()
    {
        // Default UI state on scene load
        ShowMainMenu();
    }

    // --- Main Menu Navigation ---

    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        // Loads the first playable level (Index 1 in Build Settings)
        SceneManager.LoadScene(1);
    }

    public void OpenInfo()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(true);

        currentInfoPageIndex = 0;
        UpdateInfoPageVisibility();
    }

    public void ShowMainMenu()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    public void RestartProgress()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.respawnSFX);
        }

        // Clears any saved level progress or high scores if saved
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reloads the first level from fresh start
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.respawnSFX);
        }

        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    // --- Info / Tutorial Sub-Page Navigation ---

    public void NextInfoPage()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        if (currentInfoPageIndex < infoSubPages.Length - 1)
        {
            currentInfoPageIndex++;
            UpdateInfoPageVisibility();
        }
    }

    public void PreviousInfoPage()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        if (currentInfoPageIndex > 0)
        {
            currentInfoPageIndex--;
            UpdateInfoPageVisibility();
        }
    }

    private void UpdateInfoPageVisibility()
    {
        // Enable only the active sub-page
        for (int i = 0; i < infoSubPages.Length; i++)
        {
            if (infoSubPages[i] != null)
            {
                infoSubPages[i].SetActive(i == currentInfoPageIndex);
            }
        }

        // Control visibility of Next / Prev buttons based on page index
        if (prevButton != null)
        {
            prevButton.SetActive(currentInfoPageIndex > 0);
        }

        if (nextButton != null)
        {
            nextButton.SetActive(currentInfoPageIndex < infoSubPages.Length - 1);
        }
    }
}
