using System.Collections;
using UnityEngine;

public class SmoothPursuitController : MonoBehaviour
{
    [Header("Prefab For Smooth Pursuit")]
    public GameObject spherePrefabSmoothPursuit; // Prefab of the 3D sphere

    [Header("Speed For Target Movement")]
    public float speed = 1.0f; // Speed of movement

    [Header("Timer")]
    public float timer = 1f;

    private Transform smoothPursuitSphere;

    private int currentPointIndex = 0;

    private void OnEnable()
    {
        PlaceSphereInCirclePursuit();

        StartCoroutine(TimeLapsedCheckerPursuit());
    }
    void Update()
    {
        if (true)
        {
            // Calculate the new angle based on speed and time
            CircleDrawer.instance.theta += speed * Time.deltaTime;

            // Calculate the new x and y coordinates for a moving object
            float x = CircleDrawer.instance.radius * Mathf.Cos(CircleDrawer.instance.theta);
            float y = CircleDrawer.instance.radius * Mathf.Sin(CircleDrawer.instance.theta);

            // Update the position of a moving object
            smoothPursuitSphere.position = new Vector3(x, y, transform.position.z);
        }
    }
    public void PlaceSphereInCirclePursuit()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSmoothPursuit).transform;
        newSphereTransform.SetParent(CursorController.instance.targetsParent.transform);

        smoothPursuitSphere = newSphereTransform;
    }
    public IEnumerator TimeLapsedCheckerPursuit()
    {
        yield return new WaitForSeconds(timer);
        CursorController.instance.CompleteSimulation();
    }
}
