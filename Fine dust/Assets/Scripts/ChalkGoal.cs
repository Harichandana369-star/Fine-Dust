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
        player.enabled = false; // Freeze player controls
        Debug.Log("Chalk goal reached! Level complete.");

        // Play level complete SFX if assigned
        if (AudioManager.Instance != null && AudioManager.Instance.plusEnemySFX != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.plusEnemySFX);
        }

        yield return new WaitForSeconds(delayBeforeNextLevel);

        // Smoothly fade out and load the next scene
        if (LevelTransitionManager.Instance != null)
        {
            LevelTransitionManager.Instance.LoadNextLevel();
        }
        else
        {
            // Fallback direct load if transition manager isn't present
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(nextSceneIndex);
            }
            else
            {
                SceneManager.LoadScene(0); // Reset to Main Menu / Level 1
            }
        }
    }
}
