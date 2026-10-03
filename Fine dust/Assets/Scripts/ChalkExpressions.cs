using UnityEngine;

public class ChalkExpressions : MonoBehaviour
{
    [Header("Face Sprite Renderer Reference")]
    public SpriteRenderer faceSpriteRenderer;

    [Header("Expression Sprites")]
    public Sprite neutralFace;   // Default idle on ground
    public Sprite walkingFace;   // Moving left/right
    public Sprite jumpFace;      // Mid-air / Jump expression
    public Sprite drawingFace;   // Focused while drawing
    public Sprite lowMassFace;   // Worried when mass is low

    private ChalkPlayer playerScript;
    private Rigidbody2D rb;

    // Stored offsets from where you arranged the face in Scene view
    private Vector3 initialFaceLocalPosition;
    private Vector3 initialFaceLocalScale;

    void Start()
    {
        playerScript = GetComponent<ChalkPlayer>();
        rb = GetComponent<Rigidbody2D>();

        // Store the exact position & scale you manually arranged in the editor
        if (faceSpriteRenderer != null)
        {
            initialFaceLocalPosition = faceSpriteRenderer.transform.localPosition;
            initialFaceLocalScale = faceSpriteRenderer.transform.localScale;
        }

        SetExpression(neutralFace);
    }

    void Update()
    {
        if (faceSpriteRenderer == null) return;

        // 1. Keep the face at your custom arrangement while player scales
        AdjustFaceTransform();

        // 2. Low mass expression check
        if (playerScript != null && playerScript.currentMass < 30f)
        {
            SetExpression(lowMassFace);
            return;
        }

        // 3. Air / Jump expression check
        if (rb != null && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            SetExpression(jumpFace);
            return;
        }

        // 4. Drawing expression check
        if (Input.GetMouseButton(0))
        {
            SetExpression(drawingFace);
            return;
        }

        // 5. Walking vs Idle
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            SetExpression(walkingFace);
        }
        else
        {
            SetExpression(neutralFace);
        }
    }

    private void AdjustFaceTransform()
    {
        if (playerScript == null || faceSpriteRenderer == null) return;

        // Calculate remaining mass ratio (1.0 = full size, 0.0 = completely empty)
        float massRatio = Mathf.Clamp01(playerScript.currentMass / playerScript.maxMass);

        // Counter-scale parent player scaling so face sprite stays at your manual arranged size
        Vector3 playerScale = transform.localScale;

        if (playerScale.x != 0 && playerScale.y != 0)
        {
            faceSpriteRenderer.transform.localScale = new Vector3(
                (1f / playerScale.x) * initialFaceLocalScale.x,
                (1f / playerScale.y) * initialFaceLocalScale.y,
                initialFaceLocalScale.z
            );
        }

        // Keep your exact X and Z offsets from Scene view, scaling Y smoothly down as chalk shrinks
        faceSpriteRenderer.transform.localPosition = new Vector3(
            initialFaceLocalPosition.x,
            initialFaceLocalPosition.y * massRatio,
            initialFaceLocalPosition.z
        );
    }

    public void SetExpression(Sprite newSprite)
    {
        if (faceSpriteRenderer != null && newSprite != null)
        {
            faceSpriteRenderer.sprite = newSprite;
        }
    }
}
