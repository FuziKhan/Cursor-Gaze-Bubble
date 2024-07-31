using System;
using System.Collections;
using UnityEngine;

public class SaccadeController : CursorController
{
    [Header("Prefab For Saccade")]
    public GameObject spherePrefabSaccade; // Prefab of the 3D sphere

    [Header("Saccade Orientation Checks")]
    public bool horizontalSaccade = false;
    public bool verticalSaccade = false;

    private Vector3[] saccadePoints;

    private SaccadeController saccadeController;

    private void OnEnable()
    {
        saccadeController = new SaccadeController();

        saccadePoints = CircleDrawer.instance.GetKeyPoints(CircleDrawer.instance.radius);

        PlaceSphereInCircleSaccade();
        StartCoroutine(TimeLapsedCheckerSaccade());
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
    void SaccadePositionChange()
    {
        if (saccadeController.GetPlacedSpheres().Count > 0 && targetsParent.transform.childCount > 0)
        {
            if (horizontalSaccade)
            {
                int x = UnityEngine.Random.Range(2, 4);     //For Right/Left
                saccadeController.GetPlacedSpheres()[0].transform.position = new Vector3(saccadePoints[x].x, saccadePoints[x].y, saccadePoints[x].z);
            }
            else if (verticalSaccade)
            {
                int x = UnityEngine.Random.Range(0, 2);     //For Up/Down
                saccadeController.GetPlacedSpheres()[0].transform.position = new Vector3(saccadePoints[x].x, saccadePoints[x].y, saccadePoints[x].z);
            }
        }
    }
    public IEnumerator TimeLapsedCheckerSaccade()
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
        horizontalSaccade = false;
        verticalSaccade = false;
        CompleteSimulation();
    }
    public void PlaceSphereInCircleSaccade()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSaccade).transform;
        newSphereTransform.SetParent(targetsParent.transform);

        saccadeController.SetPlacedSpheres(newSphereTransform);
        SaccadePositionChange();
    }
}
