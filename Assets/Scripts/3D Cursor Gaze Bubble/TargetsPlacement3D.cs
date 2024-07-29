using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetsPlacement3D : MonoBehaviour
{
    public Camera cam; // Reference to the camera
    public GameObject spherePrefabCursorGaze; // Prefab of the 3D sphere
    public GameObject spherePrefabSaccade; // Prefab of the 3D sphere
    public Transform planeTransform; // Reference to the 3D plane
    public GameObject targetsParent;
    public int numberOfSpheres = 10;

    public float speed = 1.0f; // Speed of movement
    private List<Transform[]> circlePoints;
    private int currentCircleIndex = 0;
    private int currentPointIndex = 0;

    public Vector3 upperRightBound, lowerLeftBound;

    public Action randomPrimaryTargetCursorGaze, randomPrimaryTargetSacccade;

    private List<Transform> placedSpheres = new List<Transform>();

    public static TargetsPlacement3D instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        if (MenuController.cursorGaze)
        {
            randomPrimaryTargetCursorGaze += RandomPrimaryTarget;
        }
        else if (MenuController.saccade || MenuController.smoothPursuit)
        {
            PlaceSphereInCircle();
            randomPrimaryTargetSacccade += RandomPrimaryTargetSaccade;
            if (MenuController.smoothPursuit)
            {
                // Get the CircleDrawer instance and its points
                CircleDrawer drawer = FindObjectOfType<CircleDrawer>();
                if (drawer != null)
                {
                    circlePoints = drawer.GetCirclePoints();
                }

                if (circlePoints == null || circlePoints.Count == 0)
                {
                    Debug.LogError("No circle points found!");
                    return;
                }

                // Start the movement coroutine
                StartCoroutine(MoveAlongPoints());
            }
        }
    }
    IEnumerator MoveAlongPoints()
    {
        while (true)
        {
            if (circlePoints == null || circlePoints.Count == 0)
            {
                yield break; // Exit the coroutine if there are no points
            }

            Transform[] currentCircle = circlePoints[currentCircleIndex];

            if (currentCircle == null || currentCircle.Length == 0)
            {
                yield break; // Exit the coroutine if the current circle has no points
            }

            Transform targetPoint = currentCircle[currentPointIndex];
            Vector3 targetPosition = targetPoint.position;

            // Move towards the target point
            while (Vector3.Distance(placedSpheres[0].transform.position, targetPosition) > 0.01f)
            {
                placedSpheres[0].transform.position = Vector3.MoveTowards(placedSpheres[0].transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
            }

            // Move to the next point
            currentPointIndex++;
            if (currentPointIndex >= currentCircle.Length)
            {
                currentPointIndex = 0;
                currentCircleIndex++;
                if (currentCircleIndex >= circlePoints.Count)
                {
                    currentCircleIndex = 0; // Loop back to the first circle
                }
            }

            yield return null;
        }
    }
    public void PlaceSphereInCircle()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSaccade).transform;
        newSphereTransform.SetParent(targetsParent.transform);
        int x = UnityEngine.Random.Range(0, 2);
        int y = UnityEngine.Random.Range(0, CircleDrawer.instance.circlePoints[x].Length - 1);
        newSphereTransform.transform.position = CircleDrawer.instance.circlePoints[x][y].position;
        placedSpheres.Add(newSphereTransform);
    }
    public void PlaceSpheresRandomly(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool positionFound = false;
            Transform newSphereTransform = Instantiate(spherePrefabCursorGaze).transform;

            float radius = UnityEngine.Random.Range(0.1f, 0.5f); // Random radius between 0.5 and 2 units
            newSphereTransform.localScale = Vector3.one * (radius * 2); // Set the scale of the sphere

            int attempts = 0; // To prevent infinite loop
            while (!positionFound && attempts < 100)
            {
                Vector3 randomPosition = GetRandomPositionWithinBounds();

                newSphereTransform.position = randomPosition;

                if (!IsOverlapping(newSphereTransform, radius))
                {
                    placedSpheres.Add(newSphereTransform);
                    CursorGazeBubble3D.instance.dummySpheres.Add(newSphereTransform);

                    positionFound = true;
                }
                attempts++;
            }

            if (!positionFound)
            {
                Debug.LogWarning("Could not find a suitable position for a sphere after 100 attempts.");
                Destroy(newSphereTransform.gameObject);
            }
            else
            {
                newSphereTransform.SetParent(targetsParent.transform);
            }
        }
        randomPrimaryTargetCursorGaze();
    }
    public Vector3 GetRandomPositionWithinBounds()
    {
        float randomX = UnityEngine.Random.Range(lowerLeftBound.x, upperRightBound.x);
        float randomY = UnityEngine.Random.Range(lowerLeftBound.y, upperRightBound.y);
        float randomZ = UnityEngine.Random.Range(lowerLeftBound.z, upperRightBound.z);

        return new Vector3(randomX, randomY, randomZ);
    }
    bool IsOverlapping(Transform newSphere, float radius)
    {
        foreach (Transform sphere in placedSpheres)
        {
            if (SphereOverlaps(newSphere.position, radius, sphere.position, sphere.localScale.x / 2))
            {
                return true;
            }
        }
        return false;
    }

    bool SphereOverlaps(Vector3 center1, float radius1, Vector3 center2, float radius2)
    {
        float distance = Vector3.Distance(center1, center2);
        return distance < (radius1 + radius2);
    }

    void RandomPrimaryTarget()
    {
        placedSpheres[UnityEngine.Random.Range(0, placedSpheres.Count - 1)].GetComponent<TargetsFunctionalities>().isPrimary = true;
    }
    void RandomPrimaryTargetSaccade()
    {
        int x = UnityEngine.Random.Range(0, 2);
        int y = UnityEngine.Random.Range(0, CircleDrawer.instance.circlePoints[x].Length - 1);
        placedSpheres[0].transform.position = CircleDrawer.instance.circlePoints[x][y].position;
    }
}
