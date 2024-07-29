using System.Collections.Generic;
using UnityEngine;

public class CircleDrawer : MonoBehaviour
{
    public int numberOfCircles = 2;
    public float radiusIncrement = 1.0f;
    public GameObject pointPrefab; // Prefab to visualize the points
    public Material lineMaterial; // Material for the LineRenderer
    public LineRenderer lineRenderer;

    public List<Transform[]> circlePoints;

    public static CircleDrawer instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        if (pointPrefab == null)
        {
            Debug.LogError("Point Prefab is not assigned.");
            return;
        }

        circlePoints = new List<Transform[]>();
        GenerateCircles();
    }
    public List<Transform[]> GetCirclePoints()
    {
        return circlePoints;
    }
    void GenerateCircles()
    {
        for (int i = 0; i < numberOfCircles; i++)
        {
            float radius = (i + 1) * radiusIncrement;
            Transform[] points = GenerateCirclePoints(radius);
            if (points != null)
            {
                circlePoints.Add(points);
                DrawCircle(points, radius);
            }
        }
    }

    Transform[] GenerateCirclePoints(float radius)
    {
        Transform[] points = new Transform[360];
        for (int i = 0; i < 360; i++)
        {
            float angle = i * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            Vector3 pointPosition = new Vector3(x, y, 0);

            // Instantiate the point prefab to visualize the point
            GameObject pointObject = Instantiate(pointPrefab, pointPosition, Quaternion.identity, transform);
            if (pointObject != null)
            {
                points[i] = pointObject.transform;
            }
            else
            {
                Debug.LogError("Failed to instantiate point prefab at index " + i);
                return null;
            }
        }
        return points;
    }

    void DrawCircle(Transform[] points, float radius)
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogError("Points array is null or empty.");
            return;
        }

        lineRenderer.positionCount = points.Length + 1;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.material = lineMaterial; // Set the material for the LineRenderer

        Vector3[] positions = new Vector3[points.Length + 1];
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] != null)
            {
                positions[i] = points[i].position;
            }
            else
            {
                Debug.LogError("Point at index " + i + " is null.");
                return;
            }
        }
        positions[positions.Length - 1] = points[0].position; // Loop back to the start
        lineRenderer.SetPositions(positions);
    }
}