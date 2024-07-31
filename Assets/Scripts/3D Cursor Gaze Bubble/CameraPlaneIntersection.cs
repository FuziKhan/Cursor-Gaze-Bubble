using UnityEngine;

public class CameraPlaneIntersection : MonoBehaviour
{
    [Header("Main Camera")]
    public Camera cam;  // Reference to the camera

    [Header("Board")]
    public Transform plane;  // Reference to the plane

    private bool stop = false;

    private void OnEnable()
    {
        stop = false;
    }
    void Update()
    {
        if (!stop)
        {
            if (cam == null || plane == null)
                return;

            // Define the viewport corners in normalized screen coordinates
            Vector3[] corners = new Vector3[4];
            corners[0] = new Vector3(0, 0, 0);  // Bottom-left
            corners[1] = new Vector3(1, 0, 0);  // Bottom-right
            corners[2] = new Vector3(0, 1, 0);  // Top-left
            corners[3] = new Vector3(1, 1, 0);  // Top-right

            for (int i = 0; i < corners.Length; i++)
            {
                Ray ray = cam.ViewportPointToRay(corners[i]);
                Plane planeObject = new Plane(plane.up, plane.position);
                if (planeObject.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    Debug.Log("Viewport corner " + i + " intersects the plane at: " + hitPoint);
                    if (i == 0)
                    {
                        CursorGazeBubbleController.instance.lowerLeftBound = hitPoint;
                    }
                    else if (i == 3)
                    {
                        CursorGazeBubbleController.instance.upperRightBound = hitPoint;
                        stop = true;
                        CursorGazeBubbleController.instance.PlaceSpheresRandomly(CursorGazeBubbleController.instance.numberOfSpheres);
                    }
                }
            }
        }
    }
}