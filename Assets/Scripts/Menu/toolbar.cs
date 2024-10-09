using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class toolbar : MonoBehaviour
{
    // Start is called before the first frame update
    public float loadTime = 5.0f; // Time to fill the bar completely
    private RectTransform rawImageRect; // Reference to the RectTransform of the RawImage
    private float originalWidth; // Original width of the RawImage
    private float elapsedTime = 0f; // Tracks the elapsed time for loading

    void Start()
    {
        // Get the RectTransform component of the RawImage
        rawImageRect = GetComponent<RectTransform>();

        // Store the original width of the RawImage
        originalWidth = rawImageRect.sizeDelta.x;

        // Set the pivot to (0, 0.5) to make the RawImage expand from the left side
        rawImageRect.pivot = new Vector2(0f, 0.5f);

        // Set the initial width to 0 (starting empty)
        rawImageRect.sizeDelta = new Vector2(0, rawImageRect.sizeDelta.y);
    }

    void Update()
    {
        // Increase elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate the loading progress (from 0 to 1)
        float progress = Mathf.Clamp01(elapsedTime / loadTime);

        // Update the width of the RawImage based on the loading progress
        rawImageRect.sizeDelta = new Vector2(originalWidth * progress, rawImageRect.sizeDelta.y);
    }
}







