using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public float radius; // The radius of the circle
    public Material lineMaterial; // Material for the LineRenderer
    public LineRenderer lineRenderer;

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

        DrawCircle(radius);
        TargetsPlacement3D.instance.circlePoints = GetCirclePoints();
        TargetsPlacement3D.instance.saccadePoints = GetKeyPoints(radius);
    }
    void DrawCircle(float radius)
    {
        int segments = 360;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.material = lineMaterial; // Set the material for the LineRenderer

        Vector3[] positions = new Vector3[segments + 1];
        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            positions[i] = new Vector3(x, y, 0);
        }
        positions[segments] = positions[0]; // Loop back to the start
        lineRenderer.SetPositions(positions);
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