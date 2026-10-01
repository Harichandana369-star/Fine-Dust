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
        player.enabled = false; // Freeze controls
        Debug.Log("Chalk goal reached! Level complete.");

        yield return new WaitForSeconds(delayBeforeNextLevel);

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Reset to Level 1 if no more scenes in build
            SceneManager.LoadScene(0);
        }
    }
}
