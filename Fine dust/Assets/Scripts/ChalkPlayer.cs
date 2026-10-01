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
    public float autoDecayRate = 2f; // Passive chalk decay over time

    [Header("Grounding State")]
    public bool isGrounded; // PUBLIC so ChalkDrawer can read it directly!

    private Rigidbody2D rb;
    private Vector3 initialScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialScale = transform.localScale;
    }

    void Update()
    {
        // 1. Check Ground Status
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        // 2. Passive Mass Decay
        if (currentMass > minMass)
        {
            ConsumeMass(autoDecayRate * Time.deltaTime);
        }

        // 3. Horizontal Movement & Jump
        float moveInput = Input.GetAxisRaw("Horizontal");
        float massRatio = Mathf.Clamp01(currentMass / maxMass);

        // Movement speed adjusts dynamically with player mass
        float currentSpeed = Mathf.Lerp(lightSpeed, heavySpeed, massRatio);
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);

        // Jump logic
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 4. Update Visual Size Based on Mass
        transform.localScale = new Vector3(
            initialScale.x * Mathf.Lerp(0.5f, 1.2f, massRatio),
            initialScale.y * massRatio,
            initialScale.z
        );
    }

    public void ConsumeMass(float amount)
    {
        currentMass = Mathf.Max(minMass, currentMass - amount);
    }

    public void GainMass(float amount)
    {
        currentMass = Mathf.Min(maxMass, currentMass + amount);
    }
}
