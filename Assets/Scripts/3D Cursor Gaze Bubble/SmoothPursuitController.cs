using System.Collections;
using UnityEngine;

public class SmoothPursuitController : CursorController
{
    [Header("Prefab For Smooth Pursuit")]
    public GameObject spherePrefabSmoothPursuit; // Prefab of the 3D sphere

    private SmoothPursuitController smoothPursuitController;

    private int currentPointIndex = 0;

    private void OnEnable()
    {
        smoothPursuitController = new SmoothPursuitController();

        PlaceSphereInCirclePursuit();

        StartCoroutine(TimeLapsedCheckerPursuit());
    }
    void Update()
    {
        if (true && smoothPursuitController.GetPlacedSpheres().Count > 0)
        {
            // Calculate the new angle based on speed and time
            CircleDrawer.instance.theta += speed * Time.deltaTime;

            // Calculate the new x and y coordinates for a moving object
            float x = CircleDrawer.instance.radius * Mathf.Cos(CircleDrawer.instance.theta);
            float y = CircleDrawer.instance.radius * Mathf.Sin(CircleDrawer.instance.theta);

            // Optionally: Update the position of a moving object
            smoothPursuitController.GetPlacedSpheres()[0].transform.position = new Vector3(x, y, transform.position.z);
        }
    }
    public void PlaceSphereInCirclePursuit()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSmoothPursuit).transform;
        newSphereTransform.SetParent(targetsParent.transform);

        smoothPursuitController.SetPlacedSpheres(newSphereTransform);
    }
    public IEnumerator TimeLapsedCheckerPursuit()
    {
        yield return new WaitForSeconds(timer);
        CompleteSimulation();
    }
}
