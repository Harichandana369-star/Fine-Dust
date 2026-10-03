using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Tag Reference")]
    [Tooltip("Tag assigned to the trigger collider at the bottom of the screen.")]
    public string killZoneTag = "KillZone";

    private Vector3 spawnPosition;
    private Rigidbody2D rb;
    private ChalkPlayer chalkPlayer;

    void Start()
    {
        // 1. Store initial spawn position placed in the Scene Editor
        spawnPosition = transform.position;

        // 2. Fetch required component references
        rb = GetComponent<Rigidbody2D>();
        chalkPlayer = GetComponent<ChalkPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect collision with any trigger tagged as KillZone
        if (collision.CompareTag(killZoneTag))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        // 1. Teleport player back to the starting position
        transform.position = spawnPosition;

        // 2. Cancel out all existing physics velocity/momentum
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            rb.angularVelocity = 0f;
        }

        // 3. Reset chalk mass if ChalkPlayer component exists
        if (chalkPlayer != null)
        {
            chalkPlayer.currentMass = chalkPlayer.maxMass;

            // If ChalkPlayer uses a dedicated scale/mass refresh method, trigger it here:
            // chalkPlayer.ResetChalk(); 
        }
    }

    // Optional: Call this function if you add checkpoints later in a level
    public void SetNewCheckpoint(Vector3 newCheckpointPosition)
    {
        spawnPosition = newCheckpointPosition;
    }
}