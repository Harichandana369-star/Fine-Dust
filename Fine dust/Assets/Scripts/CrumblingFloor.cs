using System.Collections;
using UnityEngine;

public class CrumblingFloor : MonoBehaviour
{
    [Header("Erosion Settings")]
    public float maxDurability = 100f;
    public float currentDurability = 100f;
    public float erosionPerSecond = 50f; // Speed at which player standing erodes floor
    public float destroyDelay = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Collider2D floorCollider;
    private Color originalColor;
    private bool isDestroyed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        floorCollider = GetComponent<Collider2D>();

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDestroyed) return;

        ChalkPlayer player = collision.gameObject.GetComponent<ChalkPlayer>();
        if (player != null)
        {
            // Floor erodes when player stands or walks on it
            currentDurability -= erosionPerSecond * Time.deltaTime;

            // Fade opacity to show visual erosion
            if (spriteRenderer != null)
            {
                float alpha = Mathf.Clamp01(currentDurability / maxDurability);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            if (currentDurability <= 0f)
            {
                StartCoroutine(CrumbleAndDestroy());
            }
        }
    }

    private IEnumerator CrumbleAndDestroy()
    {
        isDestroyed = true;

        if (floorCollider != null)
        {
            floorCollider.enabled = false;
        }

        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
