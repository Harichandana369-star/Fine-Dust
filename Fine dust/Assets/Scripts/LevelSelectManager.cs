using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [System.Serializable]
    public struct LevelButton
    {
        public Button button;
        public GameObject lockIcon;
        public string sceneName; // e.g., "Level1", "Level2"
    }

    [Header("Level Buttons Setup")]
    public LevelButton[] levelButtons;

    [Header("UI Panels")]
    public GameObject mainButtonsPanel; // Panel containing Start, Levels, Rules, Quit buttons
    public GameObject levelSelectPanel; // Panel containing Level 1-6 buttons

    private void OnEnable()
    {
        RefreshLevelButtons();
    }

    public void RefreshLevelButtons()
    {
        // Level 1 is always unlocked (Level 1 reached = 1)
        int highestLevelReached = PlayerPrefs.GetInt("HighestLevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1; // Level numbers starting at 1

            if (levelIndex <= highestLevelReached)
            {
                // Level is unlocked
                levelButtons[i].button.interactable = true;
                if (levelButtons[i].lockIcon != null)
                    levelButtons[i].lockIcon.SetActive(false);
            }
            else
            {
                // Level is locked
                levelButtons[i].button.interactable = false;
                if (levelButtons[i].lockIcon != null)
                    levelButtons[i].lockIcon.SetActive(true);
            }
        }
    }

    // Called when clicking any unlocked Level button
    public void SelectLevel(int levelNumber)
    {
        string levelSceneName = "Level" + levelNumber;

        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.LoadSceneByName(levelSceneName);
        }
        else
        {
            SceneManager.LoadScene(levelSceneName);
        }
    }

    // Switch between Main Menu panel and Level Select panel
    public void OpenLevelSelect()
    {
        mainButtonsPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        levelSelectPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }
}
