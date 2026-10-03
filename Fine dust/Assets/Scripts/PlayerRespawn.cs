using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.respawnSFX);
        }

        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.buildIndex);
    }
}