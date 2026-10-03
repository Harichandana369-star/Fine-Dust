using UnityEngine;
using UnityEngine.SceneManagement; // Required for Scene reloading

public class PlayerRespawn : MonoBehaviour
{
    [Header("Tag Reference")]
    public string killZoneTag = "KillZone";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(killZoneTag))
        {
            Respawn();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(killZoneTag))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // Reloads the currently active level, resetting ALL scene objects and variables to their original state
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}