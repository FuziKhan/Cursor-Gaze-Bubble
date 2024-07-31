using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    [Header("Cursor Sphere")]
    public Transform sphere; // Reference to the main sphere GameObject

    [Header("Main Camera")]
    public Camera mainCamera; // Reference to the main camera

    [Header("Speed For Cursor Transition")]
    public float scaleLerpSpeed = 1f;   //How fast the cursor will scale

    [Header("Targets Parent")]
    public GameObject targetsParent;

    [Header("Time For Simulation")]
    public float timer = 30f;

    [Header("Speed For Target Movement")]
    public float speed = 1.0f; // Speed of movement

    [HideInInspector]
    public List<Transform> dummySpheres = new List<Transform>(); // List of dummy spheres

    private Vector3 lastMousePosition;

    public static CursorController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void OnEnable()
    {
        // Initialize the last mouse position
        lastMousePosition = Input.mousePosition;

        if (MenuController.saccade || MenuController.smoothPursuit)
            sphere.localScale = new Vector3(0.2f, 0.2f, 1f);    //Constant scale is required in these modes
    }
    void Update()
    {
        if (sphere != null && mainCamera != null)
        {
            // Get the current mouse position
            Vector3 currentMousePosition = Input.mousePosition;

            // Convert the mouse position to world space
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(currentMousePosition.x, currentMousePosition.y, mainCamera.nearClipPlane));

            // Clamp the sphere's position within the camera's field of vision
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(worldPosition);
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, 0.05f, 0.95f); // Slightly inside the edges to keep the sphere visible
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, 0.05f, 0.95f);
            worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

            // Update the sphere's position on the x and y axes
            sphere.position = new Vector3(worldPosition.x, worldPosition.y, sphere.position.z);

            // Find the closest dummy sphere
            if (dummySpheres.Count > 0)
            {
                Transform closestDummy = FindClosestDummy();

                // Update the sphere's scale to match the closest dummy sphere's scale
                if (closestDummy != null)
                {
                    sphere.localScale = Vector3.Lerp(sphere.localScale, closestDummy.localScale, scaleLerpSpeed * Time.deltaTime);
                }
            }
        }
    }
    Transform FindClosestDummy()
    {
        Transform closestDummy = null;
        float minDistance = float.MaxValue;

        foreach (Transform dummy in dummySpheres)
        {
            if (dummy != null)
            {
                float distance = Vector3.Distance(sphere.position, dummy.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestDummy = dummy;
                }
            }
        }
        return closestDummy;
    }
    protected List<Transform> GetPlacedSpheres()
    {
        return dummySpheres;
    }
    protected void SetPlacedSpheres(Transform obj)
    {
        dummySpheres.Add(obj);
    }
    public void CompleteSimulation()
    {
        MenuController.instance.CompletePanel();
    }
    public void ResetAll()
    {
        if (dummySpheres.Count > 0)
        {
            for (int i = 0; i < dummySpheres.Count; i++)
                Destroy(dummySpheres[i].gameObject);
        }
        else
        {
            for (int i = 0; i < targetsParent.transform.childCount; i++)
            {
                Destroy(targetsParent.transform.GetChild(i).gameObject);
            }
        }
        dummySpheres.Clear();
        if (CircleDrawer.instance)
        {
            CircleDrawer.instance.lineRenderer.positionCount = 0;
        }
    }
}