using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetsPlacements : MonoBehaviour
{
    public RectTransform canvasRectTransform;
    public GameObject imagePrefab; // Prefab of the circular UI image
    public Transform targetsParent;
    public int numberOfImages = 10;

    private List<RectTransform> placedImages = new List<RectTransform>();

    private void Awake()
    {
        PlaceImagesRandomly(numberOfImages);

        for (int i = 0; i < placedImages.Count; i++)
        {
            CursorGazeBubble.instance.dummyTargets.Add(placedImages[i].gameObject);
        }
        CursorGazeBubble.instance.targetImage = CursorGazeBubble.instance.dummyTargets[Random.Range(0, numberOfImages)].GetComponent<RectTransform>();
    }
    void Start()
    {
        
    }

    void PlaceImagesRandomly(int count)
    {
        for (int i = 0; i < count; i++)
        {
            bool positionFound = false;
            RectTransform newImageRectTransform = Instantiate(imagePrefab, canvasRectTransform).GetComponent<RectTransform>();

            float width = Random.Range(50f, 200f);
            float height = width; // For circular images, width and height are the same

            newImageRectTransform.sizeDelta = new Vector2(width, height);
            float halfWidth = width / 2;
            float halfHeight = height / 2;

            while (!positionFound)
            {
                //Vector2 randomPosition = new Vector2(
                //    Random.Range(halfWidth, canvasRectTransform.rect.width - halfWidth),
                //    Random.Range(halfHeight, canvasRectTransform.rect.height - halfHeight)
                //);
                Vector2 randomPosition = new Vector2(
                    Random.Range(-canvasRectTransform.rect.width/2, canvasRectTransform.rect.width/2),
                    Random.Range(-canvasRectTransform.rect.height/2, canvasRectTransform.rect.height / 2)
                );

                newImageRectTransform.anchoredPosition = randomPosition;

                if (!IsOverlapping(newImageRectTransform))
                {
                    placedImages.Add(newImageRectTransform);
                    positionFound = true;
                }
            }
            placedImages[i].SetParent(targetsParent);
        }
    }

    bool IsOverlapping(RectTransform newRect)
    {
        foreach (RectTransform rect in placedImages)
        {
            if (RectOverlaps(newRect, rect))
            {
                return true;
            }
        }
        return false;
    }

    bool RectOverlaps(RectTransform rect1, RectTransform rect2)
    {
        float radius1 = rect1.rect.width / 2;
        float radius2 = rect2.rect.width / 2;

        Vector2 center1 = rect1.anchoredPosition;
        Vector2 center2 = rect2.anchoredPosition;

        float distance = Vector2.Distance(center1, center2);

        return distance < (radius1 + radius2);
    }
}
