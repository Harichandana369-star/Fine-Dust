using System.Collections.Generic;
using UnityEngine;

public class ChalkDrawer : MonoBehaviour
{
    [Header("References")]
    public GameObject linePrefab;
    public ChalkPlayer player;

    [Header("Collision & Obstacles")]
    public LayerMask obstacleLayers;            // Assign Ground/Platform layers in Inspector

    [Header("Proximity & Limits")]
    public float maxDrawRadiusFromPlayer = 3.0f; // Distance limit around player stick
    public float maxTotalLineLength = 25f;       // Total length limit allowed on board
    public int maxLineSegments = 5;              // Maximum number of lines allowed

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

        bool canDraw = (player != null && player.isGrounded && player.currentMass > player.minMass + 1f);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 cursorPoint = new Vector2(worldPos.x, worldPos.y);

        float distanceToPlayer = player != null ? Vector2.Distance(player.transform.position, cursorPoint) : 999f;

        // 1. Begin drawing on mouse click
        if (canDraw && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            if (distanceToPlayer <= maxDrawRadiusFromPlayer && activeLines.Count < maxLineSegments && currentTotalLength < maxTotalLineLength)
            {
                CreateNewLine();
            }
        }

        // 2. Add points while dragging (Blocked if drawing through solid platforms)
        if (canDraw && (Input.GetMouseButton(0) || Input.GetMouseButton(1)) && currentLine != null)
        {
            if (distanceToPlayer <= maxDrawRadiusFromPlayer)
            {
                if (linePoints.Count == 0)
                {
                    AddPoint(cursorPoint);
                }
                else
                {
                    Vector2 lastPoint = linePoints[linePoints.Count - 1];
                    float stepDistance = Vector2.Distance(lastPoint, cursorPoint);

                    if (stepDistance >= minPointDistance)
                    {
                        // Check for solid platforms/walls in the path of the line segment
                        Vector2 direction = (cursorPoint - lastPoint).normalized;
                        RaycastHit2D hit = Physics2D.Raycast(lastPoint, direction, stepDistance, obstacleLayers);

                        if (hit.collider != null)
                        {
                            // Hit an obstacle: snap point to impact location and stop line
                            float hitDistance = Vector2.Distance(lastPoint, hit.point);

                            if (currentTotalLength + hitDistance <= maxTotalLineLength)
                            {
                                currentTotalLength += hitDistance;
                                player.ConsumeMass(drawMassCostPerSecond * Time.deltaTime);
                                AddPoint(hit.point);
                            }

                            FinishLine();
                        }
                        else
                        {
                            // Clear path: add point normally
                            if (currentTotalLength + stepDistance <= maxTotalLineLength)
                            {
                                currentTotalLength += stepDistance;
                                player.ConsumeMass(drawMassCostPerSecond * Time.deltaTime);
                                AddPoint(cursorPoint);
                            }
                        }
                    }
                }
            }
        }

        // 3. Stop drawing on mouse release or if grounded state breaks
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || !canDraw) && currentLine != null)
        {
            FinishLine();
        }
    }

    private void CreateNewLine()
    {
        if (linePrefab == null) return;

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

    public void ClearAllDrawnLines()
    {
        foreach (GameObject line in activeLines)
        {
            if (line != null) Destroy(line);
        }
        activeLines.Clear();
        currentTotalLength = 0f;
    }

    public float GetCurrentTotalLength()
    {
        return currentTotalLength;
    }
}
