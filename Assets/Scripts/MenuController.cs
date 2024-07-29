using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static bool cursorGaze, saccade, smoothPursuit = false;

    public GameObject cursorGazeScript, targetPlacementScript,
        cameraPlaneIntersectionScript, circleDrwaerScript;

    public void SelectMode(int mode)
    {
        gameObject.SetActive(false);

        switch (mode)
        {
            case 0:
                Debug.Log("Cursor Gaze Bubble Selected");

                cursorGaze = true;
                saccade = false;
                smoothPursuit = false;

                cursorGazeScript.SetActive(true);
                targetPlacementScript.SetActive(true);
                cameraPlaneIntersectionScript.SetActive(true);
                break;
            case 1:
                Debug.Log("Saccade Selected");

                cursorGaze = false;
                saccade = true;
                smoothPursuit = false;

                circleDrwaerScript.SetActive(true);     //This should always get active 1st
                cursorGazeScript.SetActive(true);
                targetPlacementScript.SetActive(true);
                break;
            case 2:
                Debug.Log("Smooth Pursuit Selected");

                cursorGaze = false;
                saccade = false;
                smoothPursuit = true;

                circleDrwaerScript.SetActive(true);     //This should always get active 1st
                cursorGazeScript.SetActive(true);
                targetPlacementScript.SetActive(true);
                break;
        }
    }
}
