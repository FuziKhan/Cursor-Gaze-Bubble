using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static bool cursorGaze, saccade, smoothPursuit = false;

    public CursorGazeBubble3D cursorGazeScript;
    public TargetsPlacement3D targetPlacementScript;
    public CameraPlaneIntersection cameraPlaneIntersectionScript;
    public CircleDrawer circleDrwaerScript;

    public GameObject saccadeModes;
    public GameObject completePanel;
    public GameObject modes;

    public GameObject backButton;

    public static MenuController instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SelectMode(int mode)
    {
        gameObject.SetActive(false);
        saccadeModes.SetActive(false);
        backButton.SetActive(true);

        switch (mode)
        {
            //The seuqence of turning on scripts is vital
            case 0:
                Debug.Log("Cursor Gaze Bubble Selected");

                cursorGaze = true;
                saccade = false;
                smoothPursuit = false;

                cursorGazeScript.enabled = true;
                targetPlacementScript.enabled = true;
                cameraPlaneIntersectionScript.enabled = true;
                break;
            case 1:
                Debug.Log("Saccade Selected");

                cursorGaze = false;
                saccade = true;
                smoothPursuit = false;

                circleDrwaerScript.enabled = true;     //This should always get active 1st
                cursorGazeScript.enabled = true;
                targetPlacementScript.enabled = true;
                break;
            case 2:
                Debug.Log("Smooth Pursuit Selected");

                cursorGaze = false;
                saccade = false;
                smoothPursuit = true;

                circleDrwaerScript.enabled = true;     //This should always get active 1st
                cursorGazeScript.enabled = true;
                targetPlacementScript.enabled = true;
                break;
        }
    }
    public void CompletePanel()
    {
        Debug.Log("Simulation Completed");
        gameObject.SetActive(true);
        modes.SetActive(false);
        completePanel.SetActive(true);
        backButton.SetActive(false);
    }
    public void Home()
    {
        modes.SetActive(true);
        cursorGazeScript.enabled = false;
        targetPlacementScript.enabled = false;
        cameraPlaneIntersectionScript.enabled = false;
        circleDrwaerScript.enabled = false;

        TargetsPlacement3D.instance.ResetAll();
    }
    public void BackButton()
    {
        backButton.SetActive(false);

        TargetsPlacement3D.instance.CompleteSimulation();
    }
}
