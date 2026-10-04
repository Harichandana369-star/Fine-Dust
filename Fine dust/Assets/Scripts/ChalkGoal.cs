using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChalkGoal : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        ChalkPlayer player = collision.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            isTriggered = true;
            StartCoroutine(CompleteLevel(player));
        }
    }

    private IEnumerator CompleteLevel(ChalkPlayer player)
    {
        // Disable player movement script upon reaching goal
        player.enabled = false;
        Debug.Log("Chalk goal reached! Level complete.");

        // Save level progress to PlayerPrefs
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int savedLevelReached = PlayerPrefs.GetInt("HighestLevelReached", 1);

        if (currentSceneIndex + 1 > savedLevelReached)
        {
            PlayerPrefs.SetInt("HighestLevelReached", currentSceneIndex + 1);
            PlayerPrefs.Save(); // Persist saved level unlock to disk
        }

        yield return new WaitForSeconds(0.2f);

        // Show the Victory UI Popup
        if (VictoryManager.Instance != null)
        {
            VictoryManager.Instance.ShowVictory();
        }
        else
        {
            // Fallback in case VictoryManager is missing from scene
            if (LevelTransitionManager.Instance != null)
            {
                LevelTransitionManager.Instance.LoadNextLevel();
            }
            else
            {
                int nextSceneIndex = currentSceneIndex + 1;
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
            }
        }
    }
}
