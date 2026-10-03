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
    public float autoDecayRate = 2f; // Chalk slowly wears away over time!

    [Header("Grounding State")]
    public bool isGrounded;

    [Header("Visual Mass Feedback")]
    public SpriteRenderer spriteRenderer;
    public Color fullMassColor = Color.white;
    public Color lowMassColor = new Color(1f, 0.3f, 0.3f, 0.5f);

    private Rigidbody2D rb;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        initialScale = transform.localScale;
        currentMass = maxMass;
        UpdateVisuals();
    }

    void Update()
    {
        // 1. Check Ground Status
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.3f, groundLayer);
        }

        // 2. PASSIVE CHALK DECAY (Shrinks over time / moving / idling)
        if (currentMass > minMass)
        {
            ConsumeMass(autoDecayRate * Time.deltaTime);
        }

        // 3. Movement & Jump
        float moveInput = Input.GetAxisRaw("Horizontal");
        float massRatio = Mathf.Clamp01(currentMass / maxMass);
        float currentSpeed = Mathf.Lerp(lightSpeed, heavySpeed, massRatio);

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
#else
        rb.velocity = new Vector2(moveInput * currentSpeed, rb.velocity.y);
#endif

        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
#else
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
#endif
            isGrounded = false;
        }

        // 4. Respawn check
        if (currentMass <= minMass)
        {
            PlayerRespawn respawn = GetComponent<PlayerRespawn>();
            if (respawn != null) respawn.Respawn();
        }
    }

    public void ConsumeMass(float amount)
    {
        currentMass = Mathf.Max(minMass, currentMass - amount);
        UpdateVisuals();
    }

    public void GainMass(float amount)
    {
        currentMass = Mathf.Min(150f, currentMass + amount);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        float massRatio = Mathf.Clamp01((currentMass - minMass) / (maxMass - minMass));

        // Scale shrinks smoothly
        float scaleFactor = Mathf.Clamp(currentMass / 100f, 0.4f, 1.5f);
        transform.localScale = initialScale * scaleFactor;

        // Color shifts from white to red/transparent as mass drains
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.Lerp(lowMassColor, fullMassColor, massRatio);
        }
    }
}