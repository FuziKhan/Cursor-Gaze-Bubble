using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using Random = UnityEngine.Random;

public class CursorGazeBubble : MonoBehaviour
{
    public RectTransform bubbleImage; // Reference to the UI bubble image
    public RectTransform targetImage; // Refernece to the UI target image
    public Camera mainCamera; // Reference to the main camera
    public Canvas mainCanvas; // Reference to the main canvas

    [Header("Target Dimensions")]
    [SerializeField]
    private float targetWidth;
    [SerializeField]
    private float targetHeight;
    [Header("Cursor Dimensions")]
    [SerializeField]
    private float bubbleWidth;
    [SerializeField]
    private float bubbleHeight;

    [Header("Target Interactable Time")]
    [SerializeField]
    private float interactableTime = 1f;         //Time interval after which target changes position

    [Header("Selecion Distance From Target")]
    [SerializeField]
    private float selectionDistanceTarget;   

    private float timer = 0f;

    private Vector2 originalSize;

    public List<GameObject> dummyTargets; // List of dummy target objects

    public float maxBubbleSize = 300f;    // Maximum bubble size
    public float minBubbleSize = 50f;    // Minimum bubble size

    public Color dummColor;

    GameObject closestDummy = null; // To keep track of the closest dummy
    float relevantDistance;

    private Action OnInteractionWithTarget;

    public static CursorGazeBubble instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        OnInteractionWithTarget += RandomTargetPositionSetter;

        // Store the original size of the bubble
        originalSize = bubbleImage.sizeDelta;

        //Settings dimensions of target
        targetHeight = targetImage.rect.height;
        targetWidth = targetImage.rect.width;

        //Settings dimensions of bubbleCursor
        bubbleHeight = bubbleImage.rect.height;
        bubbleWidth = bubbleImage.rect.width;

        selectionDistanceTarget = targetImage.rect.width;

        targetImage.GetComponent<Image>().color = Color.black;
    }
    private void Update()
    {
        // Get the mouse position in screen space
        Vector2 mouseScreenPosition = Input.mousePosition;

        // Convert screen position to local position in the canvas
        Vector2 anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mainCanvas.transform as RectTransform, mouseScreenPosition, mainCamera, out anchoredPosition);

        // Get the size of the canvas and the bubble
        RectTransform canvasRectTransform = mainCanvas.transform as RectTransform;
        RectTransform bubbleRectTransform = bubbleImage;

        // Clamp the bubble's anchored position to ensure it stays within the canvas
        float clampedX = Mathf.Clamp(anchoredPosition.x, -canvasRectTransform.rect.width / 2 + bubbleRectTransform.rect.width / 2, canvasRectTransform.rect.width / 2 - bubbleRectTransform.rect.width / 2);
        float clampedY = Mathf.Clamp(anchoredPosition.y, -canvasRectTransform.rect.height / 2 + bubbleRectTransform.rect.height / 2, canvasRectTransform.rect.height / 2 - bubbleRectTransform.rect.height / 2);

        // Set the clamped position
        bubbleRectTransform.anchoredPosition = new Vector2(clampedX, clampedY);

        // Calculate the distance between the bubble and the target
        float distance = Vector3.Distance(bubbleRectTransform.position, targetImage.transform.position);

        //Checking the distance from the target
        if (distance <= selectionDistanceTarget)
        {
            //bubbleHeight = Mathf.Lerp(bubbleHeight, targetHeight, Time.deltaTime * cursorTransitionSpeed);
            //bubbleWidth = Mathf.Lerp(bubbleWidth, targetWidth, Time.deltaTime * cursorTransitionSpeed);

            ////Adjusting size of our cursor according to the target size
            //bubbleImage.sizeDelta = new Vector2(bubbleImage.sizeDelta.x, bubbleHeight);
            //bubbleImage.sizeDelta = new Vector2(bubbleWidth, bubbleImage.sizeDelta.y);

            CheckIfStayOnTarget();
        }
        else if (distance > selectionDistanceTarget)
        {
            //bubbleHeight = Mathf.Lerp(bubbleHeight, originalSize.x, Time.deltaTime * cursorTransitionSpeed);
            //bubbleWidth = Mathf.Lerp(bubbleWidth, originalSize.y, Time.deltaTime * cursorTransitionSpeed);

            ////Resetting the size of the cursor to its original
            //bubbleImage.sizeDelta = new Vector2(bubbleImage.sizeDelta.x, bubbleHeight);
            //bubbleImage.sizeDelta = new Vector2(bubbleWidth, bubbleImage.sizeDelta.y);
        }

        // Get the cursor position
        Vector3 cursorPosition = bubbleImage.position;

        // Get the primary target position
        Vector3 primaryTargetPosition = targetImage.position;

        // Calculate the distance to the primary target
        float primaryDistance = Vector3.Distance(cursorPosition, primaryTargetPosition);
        Debug.Log("Primary Distance= " + primaryDistance);
        // Calculate the minimum distance to any dummy target
        float minDummyDistance = Mathf.Infinity;

        foreach (GameObject dummyTarget in dummyTargets)
        {
            // Calculate the distance from the cursor to the dummy target
            float dummyDistance = Vector3.Distance(cursorPosition, dummyTarget.transform.position);

            // Check if this distance is smaller than the current minimum distance
            if (dummyDistance < minDummyDistance)
            {
                // Update the minimum distance and the closest dummy
                minDummyDistance = dummyDistance;
                closestDummy = dummyTarget;
                Debug.Log("Closest Object= " + closestDummy.name);
            }
        }

        if (minDummyDistance < primaryDistance)
        {
            Debug.Log("Min Dummy Distance Close= " + minDummyDistance);
        }
        else
        {
            minDummyDistance = primaryDistance;
            Debug.Log("Min Dummy Distance Far= " + minDummyDistance);
            // Use the smaller distance for bubble size adjustment
        }


        Debug.Log("Relevant Distance= " + relevantDistance);
        relevantDistance = Mathf.Min(primaryDistance, minDummyDistance);

        // Calculate the bubble size based on the relevant distance
        if (bubbleHeight <= maxBubbleSize && bubbleHeight >= minBubbleSize)
        {
            bubbleHeight = Mathf.Lerp(bubbleHeight, closestDummy.GetComponent<RectTransform>().rect.height, Time.deltaTime * 20);
            bubbleWidth = Mathf.Lerp(bubbleWidth, closestDummy.GetComponent<RectTransform>().rect.width, Time.deltaTime * 20);

            //Adjusting size of our cursor according to the target size
            bubbleImage.sizeDelta = new Vector2(bubbleImage.sizeDelta.x, bubbleHeight / 2);
            bubbleImage.sizeDelta = new Vector2(bubbleWidth / 2, bubbleImage.sizeDelta.y);
        }
    }
    private void RandomTargetPositionSetter()
    {
        Debug.Log("Target Position Changed");
        //// Get canvas size
        //float canvasWidth = mainCanvas.GetComponent<RectTransform>().rect.width;
        //float canvasHeight = mainCanvas.GetComponent<RectTransform>().rect.height;

        //// Generate random position within canvas bounds
        //float randomX = UnityEngine.Random.Range(0, canvasWidth);
        //float randomY = UnityEngine.Random.Range(0, canvasHeight);

        //// Set the UI element's anchored position
        //targetImage.anchoredPosition = new Vector2(randomX - canvasWidth / 2, randomY - canvasHeight / 2);

        targetImage.GetComponent<Image>().color = dummColor;
        int rand = Random.Range(0,dummyTargets.Count);
        dummyTargets[rand].GetComponent<Image>().color = Color.black;
        targetImage = dummyTargets[rand].GetComponent<RectTransform>();
    }
    private void CheckIfStayOnTarget()
    {
        Vector2 position1 = bubbleImage.anchoredPosition;
        Vector2 position2 = targetImage.anchoredPosition;

        long distance = (long)Vector2.Distance(position1, position2);

        if (distance < targetImage.rect.height / 1.1f)
        {
            // Increment the timer by the time since the last frame
            timer += Time.deltaTime;

            // Check if the timer has exceeded the interval
            if (timer >= interactableTime)
            {
                Debug.Log("Objects are closer than " + targetImage.rect.height + " units in " + timer + " seconds");
                OnInteractionWithTarget();
            }
        }
        else
        {
            //Resetting timer
            timer = 0f;
        }
    }
    public List<GameObject> GameObjects
    {
        get { return dummyTargets; }
    }
}