using System.Collections.Generic;
using UnityEngine;

public class CursorGazeBubble3D : MonoBehaviour
{
    public Transform sphere; // Reference to the main sphere GameObject
    public float speed = 0.1f; // Speed at which the sphere follows the mouse
    public Camera mainCamera; // Reference to the main camera
    public List<Transform> dummySpheres; // List of dummy spheres
    public float scaleLerpSpeed = 1f;   //How fast the cursor will scale

    private Vector3 lastMousePosition;

    public static CursorGazeBubble3D instance;
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
            float distance = Vector3.Distance(sphere.position, dummy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestDummy = dummy;
            }
        }
        return closestDummy;
    }
}