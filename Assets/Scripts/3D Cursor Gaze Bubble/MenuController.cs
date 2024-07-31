using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static bool cursorGaze, saccade, smoothPursuit = false;

    [Header("Controller Scripts")]
    public CursorController cursorGazeScript;
    public CursorGazeBubbleController cursorGazeBubbleScript;
    public CameraPlaneIntersection cameraPlaneIntersectionScript;
    public CircleDrawer circleDrawerScript;
    public SaccadeController saccadeController;
    public SmoothPursuitController smoothPursuitController;

    [Header("Saccade Modes Menu")]
    public GameObject saccadeModes;

    [Header("Complete Menu")]
    public GameObject completePanel;

    [Header("Modes Menu")]
    public GameObject modes;

    [Header("Back Button")]
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
                cursorGazeBubbleScript.enabled = true;
                cameraPlaneIntersectionScript.enabled = true;
                break;
            case 1:
                Debug.Log("Saccade Selected");

                cursorGaze = false;
                saccade = true;
                smoothPursuit = false;

                circleDrawerScript.enabled = true;     //This should always get active 1st
                cursorGazeScript.enabled = true;
                saccadeController.enabled = true;
                break;
            case 2:
                Debug.Log("Smooth Pursuit Selected");

                cursorGaze = false;
                saccade = false;
                smoothPursuit = true;

                circleDrawerScript.enabled = true;     //This should always get active 1st
                cursorGazeScript.enabled = true;
                smoothPursuitController.enabled = true;
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
        cursorGazeBubbleScript.enabled = false;
        cameraPlaneIntersectionScript.enabled = false;
        circleDrawerScript.enabled = false;
        saccadeController.enabled = false;
        smoothPursuitController.enabled = false;

        saccadeController.horizontalSaccade = false;
        saccadeController.verticalSaccade = false;

        saccadeController.StopAllCoroutines();
        smoothPursuitController.StopAllCoroutines();

        CursorController.instance.ResetAll();
    }
    public void BackButton()
    {
        backButton.SetActive(false);

        CursorController.instance.CompleteSimulation();
    }
}
