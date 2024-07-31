using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public Material lineMaterial; // Material for the LineRenderer

    public float radius = 5f; // The radius of the circle
    public int segments = 100; // The number of segments to approximate the circle
    public float speed = 1f; // Speed for moving object
    public LineRenderer lineRenderer;
    public float theta = 0f; // Initial angle

    SaccadeController saccadeController;

    public static CircleDrawer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void OnEnable()
    {
        saccadeController = new SaccadeController();

        if (lineMaterial == null)
        {
            Debug.LogError("Line Material is not assigned.");
            return;
        }

        if (lineRenderer == null)
        {
            Debug.LogError("Line Renderer is not assigned.");
            return;
        }
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = lineMaterial; // Set the material for the LineRenderer
        lineRenderer.widthMultiplier = 0.05f;
        //DrawCircle(radius);
        CreatePoints();
    }
    void CreatePoints()
    {
        float angleStep = 2 * Mathf.PI / segments;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = radius * Mathf.Cos(i * angleStep);
            float y = radius * Mathf.Sin(i * angleStep);

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
        }
    }
    public Vector3[] GetCirclePoints()
    {
        Vector3[] points = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(points);
        return points;
    }
    public Vector3[] GetKeyPoints(float radius)
    {
        Vector3 top = new Vector3(0, radius, 0);
        Vector3 bottom = new Vector3(0, -radius, 0);
        Vector3 left = new Vector3(-radius, 0, 0);
        Vector3 right = new Vector3(radius, 0, 0);

        return new Vector3[] { top, bottom, left, right };
    }
}