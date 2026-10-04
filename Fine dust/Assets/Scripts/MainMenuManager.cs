using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Canvas Panels")]
    public GameObject mainButtonsPanel;
    public GameObject infoPanel;
    public GameObject levelSelectPanel;

    [Header("Info Sub-Pages (Tutorial Steps)")]
    public GameObject[] infoSubPages; // Sub-page 0: Line Drawing | Sub-page 1: Math Enemies
    public GameObject prevButton;
    public GameObject nextButton;

    private int currentInfoPageIndex = 0;
    private bool isStartingGameFromRules = false; // Tracks if rules were opened by clicking Start

    void Start()
    {
        // Always start on the main menu buttons panel
        ShowMainMenu();
    }

    // --- Main Menu Navigation ---

    // Called when clicking the START button
    public void StartGame()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        // Check if player is launching the game for the first time
        if (PlayerPrefs.GetInt("HasSeenRules", 0) == 0)
        {
            isStartingGameFromRules = true; // Mark that closing rules should load Level 1
            ShowRulesForFirstTime();
        }
        else
        {
            LaunchLevel1();
        }
    }

    private void ShowRulesForFirstTime()
    {
        OpenInfoPanelUI();

        // Mark rules as seen so Start goes straight to Level 1 on future runs
        PlayerPrefs.SetInt("HasSeenRules", 1);
        PlayerPrefs.Save();
    }

    public void LaunchLevel1()
    {
        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.LoadSceneByName("Level1");
        }
        else
        {
            SceneManager.LoadScene("Level1");
        }
    }

    // Called when player manually clicks "Rules" button on Main Menu
    public void OpenInfo()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        isStartingGameFromRules = false; // Player clicked Rules manually, so close will return to Main Menu
        OpenInfoPanelUI();
    }

    private void OpenInfoPanelUI()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(true);

        currentInfoPageIndex = 0;
        UpdateInfoPageVisibility();
    }

    // LINK THIS FUNCTION TO THE "CLOSE / BACK" BUTTON INSIDE INFOPANEL
    public void CloseInfoPanel()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        if (isStartingGameFromRules)
        {
            // If opened via Start button -> Launch Level 1 immediately!
            LaunchLevel1();
        }
        else
        {
            // If opened via Rules button -> Go back to Main Menu
            ShowMainMenu();
        }
    }

    public void OpenLevelSelect()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.jumpSFX);
        }

        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
        if (infoPanel != null) infoPanel.SetActive(false);
        if (levelSelectPanel != null) levelSelectPanel.SetActive(false);
    }

    public void RestartProgress()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.respawnSFX);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        ShowMainMenu();
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
        for (int i = 0; i < infoSubPages.Length; i++)
        {
            if (infoSubPages[i] != null)
            {
                infoSubPages[i].SetActive(i == currentInfoPageIndex);
            }
        }

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
