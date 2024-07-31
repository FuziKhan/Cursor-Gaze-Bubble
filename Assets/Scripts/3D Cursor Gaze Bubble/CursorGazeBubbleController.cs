using UnityEngine;

public class CursorGazeBubbleController : CursorController
{
    [Header("Prefab For Cursor Gaze Bubble")]
    public GameObject spherePrefabCursorGaze; // Prefab of the 3D sphere

    [Header("Sphere Count On Screen")]
    public int numberOfSpheres = 10;

    [HideInInspector]
    public Vector3 upperRightBound, lowerLeftBound;

    private CursorGazeBubbleController cursorGazeBubbleController;

    public static new CursorGazeBubbleController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        cursorGazeBubbleController = new CursorGazeBubbleController();
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
                    cursorGazeBubbleController.SetPlacedSpheres(newSphereTransform);
                    dummySpheres.Add(newSphereTransform);

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
        RandomPrimaryTarget();
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
        foreach (Transform sphere in cursorGazeBubbleController.GetPlacedSpheres())
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
    public void RandomPrimaryTarget()
    {
        cursorGazeBubbleController.GetPlacedSpheres()[UnityEngine.Random.Range(0, cursorGazeBubbleController.GetPlacedSpheres().Count - 1)].GetComponent<TargetsFunctionalities>().isPrimary = true;
    }

}
