using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChalkGoal : MonoBehaviour
{
    public float delayBeforeNextLevel = 1.2f;
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;

        ChalkPlayer player = collision.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            isTriggered = true;
            StartCoroutine(TransitionToNextLevel(player));
        }
    }

    private IEnumerator TransitionToNextLevel(ChalkPlayer player)
    {
        player.enabled = false;
        Debug.Log("Chalk goal reached! Level complete.");

        // Save level progress!
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex; // Assuming Level 1 = Index 1, Level 2 = Index 2...
        int savedLevelReached = PlayerPrefs.GetInt("HighestLevelReached", 1);

        if (currentSceneIndex + 1 > savedLevelReached)
        {
            PlayerPrefs.SetInt("HighestLevelReached", currentSceneIndex + 1);
            PlayerPrefs.Save(); // Persist to disk
        }

        yield return new WaitForSeconds(delayBeforeNextLevel);

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
