using UnityEngine;

public class ChalkPlayer : MonoBehaviour
{
    [Header("Movement & Physics")]
    public float lightSpeed = 8f;
    public float heavySpeed = 3f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Mass Mechanics")]
    public float maxMass = 100f;
    public float minMass = 10f;
    public float currentMass = 100f;

    [Header("Grounding State")]
    public bool isGrounded;

    private Rigidbody2D rb;
    private Vector3 initialScale;
    private PlayerRespawn respawnScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        respawnScript = GetComponent<PlayerRespawn>();
        initialScale = transform.localScale;
        currentMass = maxMass;
        UpdateVisualScale();
    }

    void Update()
    {
        // 1. Check Ground Status
        // NEW
        if (groundCheck != null)
        {
            // 1. Slightly increase radius to 0.3f so feet reliably register on thin line edges
            Collider2D hitCollider = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);

            // 2. Fallback check: If the player stands on a drawn line collider directly
            if (hitCollider == null)
            {
                Collider2D lineHit = Physics2D.OverlapCircle(groundCheck.position, 0.3f);
                isGrounded = (lineHit != null && lineHit.GetComponent<EdgeCollider2D>() != null);
            }
            else
            {
                isGrounded = true;
            }
        }

        // 2. Horizontal Movement & Jump
        float moveInput = Input.GetAxisRaw("Horizontal");
        float massRatio = Mathf.Clamp01(currentMass / maxMass);

        float currentSpeed = Mathf.Lerp(lightSpeed, heavySpeed, massRatio);
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
#else
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
#endif

        // Jump logic
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
#else
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
#endif
            isGrounded = false;
        }

        // 3. Auto-respawn if mass drops below minMass threshold
        if (currentMass <= minMass && respawnScript != null)
        {
            respawnScript.Respawn();
        }
    }

    public void ConsumeMass(float amount)
    {
        currentMass = Mathf.Max(minMass, currentMass - amount);
        UpdateVisualScale();
    }

    public void GainMass(float amount)
    {
        currentMass = Mathf.Min(maxMass, currentMass + amount);
        UpdateVisualScale();
    }

    private void UpdateVisualScale()
    {
        float massRatio = Mathf.Clamp01(currentMass / maxMass);
        transform.localScale = new Vector3(
            initialScale.x * Mathf.Lerp(0.5f, 1.2f, massRatio),
            initialScale.y * Mathf.Clamp(massRatio, 0.2f, 1f),
            initialScale.z
        );
    }

    // Called when player respawns / dies
    public void ResetPlayerStateAndLines()
    {
        currentMass = maxMass;
        UpdateVisualScale();

        ChalkDrawer drawer = FindAnyObjectByType<ChalkDrawer>();
        if (drawer != null)
        {
            drawer.ClearAllDrawnLines();
        }
    }
}