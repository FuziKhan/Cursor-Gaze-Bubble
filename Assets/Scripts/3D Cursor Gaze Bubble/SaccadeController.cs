using System;
using System.Collections;
using UnityEngine;

public class SaccadeController : MonoBehaviour
{
    [Header("Prefab For Saccade")]
    public GameObject spherePrefabSaccade; // Prefab of the 3D sphere

    [Header("Saccade Orientation Checks")]
    public bool horizontalSaccade = false;
    public bool verticalSaccade = false;

    [Header("Timer")]
    public float timer = 1f;

    private float flag = 0f;

    private Transform saccadeSphere;

    private void OnEnable()
    {
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
        if (horizontalSaccade)
        {
            float angle = horizontalSaccade ? flag : Mathf.PI; // 0 for up, PI for down
            float x = Mathf.Cos(angle) * CircleDrawer.instance.radius; // Use Cos for vertical
                                                                       // 
            if (flag == 0)
                flag = MathF.PI;
            else
                flag = 0;

            // Update the position of the object
            saccadeSphere.position = new Vector3(x, saccadeSphere.position.y, saccadeSphere.position.z);
        }
        else if (verticalSaccade)
        {
            float angle = verticalSaccade ? flag : Mathf.PI; // 0 for up, PI for down
            float y = Mathf.Cos(angle) * CircleDrawer.instance.radius; // Use Cos for vertical
                                                                       // 
            if (flag == 0)
                flag = MathF.PI;
            else
                flag = 0;

            // Update the position of the object
            saccadeSphere.position = new Vector3(saccadeSphere.position.x, y, saccadeSphere.position.z);
        }
    }
    public IEnumerator TimeLapsedCheckerSaccade()
    {
        for (int i = 0; i < timer; i++)
        {
            // Your command to run every second
            Debug.Log("One second has passed");
            SaccadePositionChange();
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("30 seconds have passed. Exiting coroutine.");
        horizontalSaccade = false;
        verticalSaccade = false;
        CursorController.instance.CompleteSimulation();
    }
    public void PlaceSphereInCircleSaccade()
    {
        Transform newSphereTransform = Instantiate(spherePrefabSaccade).transform;
        newSphereTransform.SetParent(CursorController.instance.targetsParent.transform);

        saccadeSphere = newSphereTransform;
        SaccadePositionChange();
    }
}
