using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class ChalkPlayer : MonoBehaviour
{
    [Header("Mass Settings")]
    public float currentMass = 100f;
    public float maxMass = 100f;
    public float minMass = 5f;

    [Header("Movement Settings")]
    public float heavySpeed = 4f;
    public float lightSpeed = 9f;
    public float heavyJump = 7f;
    public float lightJump = 13f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Erosion Rates")]
    public float walkErosionRate = 0.8f; // Mass lost per unit moved
    public float dashMassCost = 15f;

    private Rigidbody2D rb;
    private Vector3 initialScale;
    private Vector3 lastPos;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Lock Z-rotation so the chalk stick stays strictly upright
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        initialScale = transform.localScale;
        lastPos = transform.position;
    }

    void Update()
    {
        // Ground Check via overlap circle
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.15f, groundLayer);
        }

        // Movement & Distance-Based Mass Erosion
        float moveInput = Input.GetAxisRaw("Horizontal");
        float distanceMoved = Vector2.Distance(transform.position, lastPos);

        if (distanceMoved > 0.01f && Mathf.Abs(rb.linearVelocity.x) > 0.1f)
        {
            ConsumeMass(distanceMoved * walkErosionRate);
            lastPos = transform.position;
        }

        // Apply Size Paradox Physics & Scale Visuals
        UpdateFormAndPhysics(moveInput);

        // Jump Input
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            float massRatio = currentMass / maxMass;
            float currentJump = Mathf.Lerp(lightJump, heavyJump, massRatio);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, currentJump);
        }

        // Dash Input
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ExecuteDash();
        }
    }

    public void ConsumeMass(float amount)
    {
        currentMass = Mathf.Clamp(currentMass - amount, minMass, maxMass);
        if (currentMass <= minMass)
        {
            OnChalkErased();
        }
    }

    public void GainMass(float amount)
    {
        currentMass = Mathf.Clamp(currentMass + amount, minMass, maxMass);
    }

    private void UpdateFormAndPhysics(float moveInput)
    {
        float massRatio = currentMass / maxMass;

        // Visual Scale: Y-scale shrinks/grows directly based on current mass
        transform.localScale = new Vector3(
            initialScale.x * Mathf.Lerp(0.5f, 1.2f, massRatio),
            initialScale.y * massRatio,
            initialScale.z
        );

        // Movement Speed Scaling: Light = fast, Heavy = slow
        float currentSpeed = Mathf.Lerp(lightSpeed, heavySpeed, massRatio);
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void ExecuteDash()
    {
        if (currentMass > dashMassCost + minMass)
        {
            ConsumeMass(dashMassCost);
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.AddForce(new Vector2(direction * 12f, 2f), ForceMode2D.Impulse);
        }
    }

    private void OnChalkErased()
    {
        // Reload current scene when completely eroded
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
