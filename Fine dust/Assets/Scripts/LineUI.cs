using UnityEngine;
using UnityEngine.UI;

public class LineUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider lineLengthSlider;
    public Image fillImage;
    public ChalkDrawer chalkDrawer;

    [Header("Color Settings")]
    public Color normalLineColor = Color.white;
    public Color lowLineColor = Color.red;

    void Update()
    {
        if (chalkDrawer == null) chalkDrawer = FindAnyObjectByType<ChalkDrawer>();
        if (chalkDrawer == null || lineLengthSlider == null) return;

        // Calculate remaining ratio of line length available
        float remainingLength = chalkDrawer.maxTotalLineLength - chalkDrawer.GetCurrentTotalLength();
        float ratio = Mathf.Clamp01(remainingLength / chalkDrawer.maxTotalLineLength);

        lineLengthSlider.value = ratio;

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowLineColor, normalLineColor, ratio);
        }
    }
}
