using UnityEngine;

public class TargetsFunctionalities : MonoBehaviour
{
    [Header("Primary Target Check")]
    public bool isPrimary = false;

    [Header("Interactable Time")]
    public float interactableTime = 1f;

    [Header("Materials For Targets")]
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

                transform.GetComponent<MeshRenderer>().material = dummyMat;
                isPrimary = false;

                CursorGazeBubbleController.instance.RandomPrimaryTarget();
            }
        }
        else if (MenuController.cursorGaze)
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
