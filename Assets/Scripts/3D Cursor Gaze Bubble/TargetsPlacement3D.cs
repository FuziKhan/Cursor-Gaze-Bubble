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

    public Vector3[] circlePoints;
    public Vector3[] saccadePoints;

    public bool horizontalSaccade = false;
    public bool verticalSaccade = false;

    public Vector3 upperRightBound, lowerLeftBound;

    public Action randomPrimaryTargetCursorGaze, randomPrimaryTargetSacccade;

    public float timer = 30f;

    private int currentPointIndex = 0;

    private List<Transform> placedSpheres = new List<Transform>();

    public static TargetsPlacement3D instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        if (MenuController.cursorGaze)
        {
            randomPrimaryTargetCursorGaze += RandomPrimaryTarget;
        }
        else
        {
            PlaceSphereInCircle();

            if (MenuController.smoothPursuit)
            {
                if (circlePoints == null || circlePoints.Length == 0)
                {
                    Debug.LogError("No circle points found!");
                    return;
                }

                // Start the movement coroutine
                StartCoroutine(MoveAlongPoints());
            }
            StartCoroutine(TimeLapsedChecker());
        }
    }
    IEnumerator MoveAlongPoints()
    {
        while (true && placedSpheres.Count > 0)
        {
            if (circlePoints == null || circlePoints.Length == 0)
            {
                yield break; // Exit the coroutine if there are no points
            }

            Vector3 targetPosition = circlePoints[currentPointIndex];

            // Move towards the target point
            if (placedSpheres[0] != null)
            {
                while (Vector3.Distance(placedSpheres[0].transform.position, targetPosition) > 0.01f)
                {
                    placedSpheres[0].transform.position = Vector3.MoveTowards(placedSpheres[0].transform.position, targetPosition, speed * Time.deltaTime);
                    yield return null;
                }
            }
            // Move to the next point
            currentPointIndex++;
            if (currentPointIndex >= circlePoints.Length)
            {
                currentPointIndex = 0; // Loop back to the first point
            }
            yield return null;
        }
    }
    IEnumerator TimeLapsedChecker()
    {
        if (MenuController.smoothPursuit)
        {
            yield return new WaitForSeconds(timer);
            CompleteSimulation();
        }
        else if (MenuController.saccade)
        {
            float elapsedTime = 0f;
            float checkInterval = 1f;

            while (elapsedTime < timer)
            {
                float intervalStartTime = Time.time;

                while (Time.time - intervalStartTime < checkInterval)
                {
                    // Perform any logic you need to check every frame here
                    yield return null;
                }

                // Logic to be executed every second
                Debug.Log("One second has passed");
                SaccadePositionChange();
                elapsedTime += checkInterval;
            }
            Debug.Log("30 seconds have passed. Exiting coroutine.");
            CompleteSimulation();
        }
    }
    public void PlaceSphereInCircle()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSaccade).transform;
        newSphereTransform.SetParent(targetsParent.transform);

        placedSpheres.Add(newSphereTransform);
        SaccadePositionChange();
    }
    void SaccadePositionChange()
    {
        if (placedSpheres.Count > 0)
        {
            if (horizontalSaccade)
            {
                int x = UnityEngine.Random.Range(2, 4);
                placedSpheres[0].transform.position = new Vector3(saccadePoints[x].x, saccadePoints[x].y, saccadePoints[x].z);
            }
            else if (verticalSaccade)
            {
                int x = UnityEngine.Random.Range(0, 2);
                placedSpheres[0].transform.position = new Vector3(saccadePoints[x].x, saccadePoints[x].y, saccadePoints[x].z);
            }
        }
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
    public void SaccadeModeChooser(int mode)
    {
        if (mode == 0)
        {
            horizontalSaccade = true;
            verticalSaccade = false;
        }
        else if (mode == 1)
        {
            horizontalSaccade = false;
            verticalSaccade = true;
        }
    }

    public void CompleteSimulation()
    {
        MenuController.instance.CompletePanel();
    }
    public void ResetAll()
    {
        for (int i = 0; i < placedSpheres.Count; i++)
            Destroy(placedSpheres[i].gameObject);

        placedSpheres.Clear();
        CursorGazeBubble3D.instance.dummySpheres.Clear();
        if (CircleDrawer.instance)
        {
            CircleDrawer.instance.lineRenderer.positionCount = 0;
        }
    }
}
