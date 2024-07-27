using System;
using System.Collections.Generic;
using UnityEngine;

public class TargetsPlacement3D : MonoBehaviour
{
    public Camera cam; // Reference to the camera
    public GameObject spherePrefab; // Prefab of the 3D sphere
    public Transform planeTransform; // Reference to the 3D plane
    public GameObject targetsParent;
    public int numberOfSpheres = 10;

    public Vector3 upperRightBound, lowerLeftBound;

    public Action randomPrimaryTarget;

    private List<Transform> placedSpheres = new List<Transform>();

    public static TargetsPlacement3D instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        randomPrimaryTarget += RandomPrimaryTarget;
    }

    public void PlaceSpheresRandomly(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool positionFound = false;
            Transform newSphereTransform = Instantiate(spherePrefab).transform;

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
        randomPrimaryTarget();
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
}
