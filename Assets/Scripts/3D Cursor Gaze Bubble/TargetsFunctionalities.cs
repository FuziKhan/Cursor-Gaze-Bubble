using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetsFunctionalities : MonoBehaviour
{
    public bool isPrimary = false;
    public float interactableTime = 1f;
    public Material primaryMat, dummyMat;

    private float timer = 0f;
    private bool check = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cursor" && isPrimary)
        {
            check = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Cursor" && isPrimary)
        {
            check = false;
            timer = 0;
        }
    }
    private void Update()
    {
        if (check)
        {
            timer += Time.deltaTime;

            // Check if the timer has exceeded the interval
            if (timer >= interactableTime)
            {
                Debug.Log("Objects are overlapping eachother for more than " + timer + " seconds");
                check = false;          //If primary target found stop checking

                if (MenuController.cursorGaze)
                {
                    transform.GetComponent<MeshRenderer>().material = dummyMat;
                    isPrimary = false;

                    TargetsPlacement3D.instance.randomPrimaryTargetCursorGaze();  //Changing the primary target
                }
                else if (MenuController.saccade)
                {
                    TargetsPlacement3D.instance.randomPrimaryTargetSacccade();  //Changing position of the primary target
                }
            }
        }
        else
        {
            if (isPrimary)
            {
                transform.GetComponent<MeshRenderer>().material = primaryMat;
            }
            else
            {
                transform.GetComponent<MeshRenderer>().material = dummyMat;
            }
            //Resetting timer
            timer = 0f;
        }
    }
}
