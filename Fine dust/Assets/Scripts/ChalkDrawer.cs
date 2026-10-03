using System.Collections.Generic;
using UnityEngine;

public class ChalkDrawer : MonoBehaviour
{
    [Header("References")]
    public GameObject linePrefab;
    public ChalkPlayer player;

    [Header("Drawing Limits")]
    public float maxTotalLineLength = 25f; // Maximum total length allowed for drawn lines
    public int maxLineSegments = 5;        // Maximum number of lines allowed on board

    [Header("Drawing Settings")]
    public float drawMassCostPerSecond = 18f;
    public float minPointDistance = 0.1f;

    private LineRenderer currentLine;
    private EdgeCollider2D currentCollider;
    private readonly List<Vector2> linePoints = new List<Vector2>();
    private readonly List<GameObject> activeLines = new List<GameObject>();
    private Camera mainCamera;
    private float currentTotalLength = 0f;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (player == null) player = FindFirstObjectByType<ChalkPlayer>();

        // Player must exist and be grounded to draw
        bool canDraw = (player != null && player.isGrounded);

        // 1. Begin drawing on mouse click (Check max line segment limit)
        if (canDraw && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            if (activeLines.Count < maxLineSegments && currentTotalLength < maxTotalLineLength)
            {
                CreateNewLine();
            }
        }

        // 2. Add path points while dragging (Check total length limit)
        if (canDraw && (Input.GetMouseButton(0) || Input.GetMouseButton(1)) && currentLine != null)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            Vector2 point = new Vector2(worldPos.x, worldPos.y);

            if (linePoints.Count == 0 || Vector2.Distance(linePoints[linePoints.Count - 1], point) > minPointDistance)
            {
                float addedDistance = linePoints.Count > 0 ? Vector2.Distance(linePoints[linePoints.Count - 1], point) : 0f;

                // Stop drawing further if maximum total length limit is reached or player is out of mass
                if (currentTotalLength + addedDistance <= maxTotalLineLength && player.currentMass > player.minMass + 1f)
                {
                    currentTotalLength += addedDistance;
                    player.ConsumeMass(drawMassCostPerSecond * Time.deltaTime);
                    AddPoint(point);
                }
            }
        }

        // 3. Stop drawing on mouse release or if player leaves the ground
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || !canDraw) && currentLine != null)
        {
            FinishLine();
        }
    }

    private void CreateNewLine()
    {
        if (linePrefab == null)
        {
            Debug.LogError("ChalkDrawer: Missing Line Prefab reference!");
            return;
        }

        GameObject newPath = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        activeLines.Add(newPath);

        currentLine = newPath.GetComponent<LineRenderer>();
        currentCollider = newPath.GetComponent<EdgeCollider2D>();

        linePoints.Clear();
    }

    private void AddPoint(Vector2 point)
    {
        linePoints.Add(point);

        currentLine.positionCount = linePoints.Count;
        currentLine.SetPosition(linePoints.Count - 1, new Vector3(point.x, point.y, 0f));

        if (currentCollider != null && linePoints.Count > 1)
        {
            currentCollider.SetPoints(linePoints);
        }
    }

    private void FinishLine()
    {
        currentLine = null;
        currentCollider = null;
    }

    // Call this on player respawn/death to clear lines
    public void ClearAllDrawnLines()
    {
        foreach (GameObject line in activeLines)
        {
            if (line != null) Destroy(line);
        }
        activeLines.Clear();
        currentTotalLength = 0f;
    }
}
