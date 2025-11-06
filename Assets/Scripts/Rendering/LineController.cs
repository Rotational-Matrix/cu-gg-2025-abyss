using UnityEngine;

public class LineController : MonoBehaviour
{
    [Header("Objects to Connect")]
    public Transform pointA;  // sariel
    public Transform pointB;  // eve

    [Header("Offsets")]
    public Vector3 offsetA = Vector3.zero;  // local offset for pointA
    public Vector3 offsetB = Vector3.zero;  // local offset for pointB

    [Header("Line Flow Settings")]
    public float scrollSpeed = 1f;  // units per second
    private Material lineMaterial;  // cached material instance

    [Header("Line Width Settings")]
    public float minWidth = 0.05f; 
    public float maxWidth = 0.2f; 
    public float maxDistance = 10f;  // distance at which line reaches minWidth

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
            Debug.LogError("No LineRenderer found");

        lineMaterial = lineRenderer.material;
    }

    private void Start()
    {
        ShowLine();  // Show on start
    }

    private void Update()
    {
        if (lineRenderer.enabled && pointA != null && pointB != null)
        {
            Vector3 posA = pointA.position + offsetA;
            Vector3 posB = pointB.position + offsetB;

            lineRenderer.SetPosition(0, posA);
            lineRenderer.SetPosition(1, posB);

            // Adjust width based on distance
            float distance = Vector3.Distance(posA, posB);
            float t = Mathf.Clamp01(distance / maxDistance); // 0 if close, 1 if at maxDistance
            float width = Mathf.Lerp(maxWidth, minWidth, t);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        // Flow the texture
        if (lineMaterial != null)
        {
            Vector2 offset = lineMaterial.mainTextureOffset;
            offset.x += scrollSpeed * Time.deltaTime;
            lineMaterial.mainTextureOffset = offset;
        }
    }

    public void ShowLine()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = true;
    }

    public void HideLine()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;
    }
}
