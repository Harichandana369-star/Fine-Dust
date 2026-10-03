using UnityEngine;
using UnityEngine.UI;

public class UIExpressionHUD : MonoBehaviour
{
    [Header("UI Reference")]
    public Image portraitImage;

    [Header("Player & Physics References")]
    public ChalkPlayer playerScript;
    public Rigidbody2D playerRb;

    [Header("Expression Sprites")]
    public Sprite neutralFace;   // Default state
    public Sprite walkingFace;   // Moving horizontal
    public Sprite jumpFace;      // Mid-air
    public Sprite drawingFace;   // Drawing with left-click
    public Sprite lowMassFace;   // Worried expression when chalk is low

    void Update()
    {
        if (portraitImage == null || playerScript == null) return;

        // 1. Low mass warning takes priority
        if (playerScript.currentMass < 30f)
        {
            SetPortrait(lowMassFace);
            return;
        }

        // 2. Air / Jump state
        if (playerRb != null && Mathf.Abs(playerRb.linearVelocity.y) > 0.1f)
        {
            SetPortrait(jumpFace);
            return;
        }

        // 3. Drawing state
        if (Input.GetMouseButton(0))
        {
            SetPortrait(drawingFace);
            return;
        }

        // 4. Horizontal movement vs Idle
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(moveInput) > 0.1f)
        {
            SetPortrait(walkingFace);
        }
        else
        {
            SetPortrait(neutralFace);
        }
    }

    private void SetPortrait(Sprite newSprite)
    {
        if (portraitImage != null && newSprite != null)
        {
            portraitImage.sprite = newSprite;
        }
    }
}
