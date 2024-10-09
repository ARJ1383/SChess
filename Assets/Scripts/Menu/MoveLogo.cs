using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveLogo : MonoBehaviour
{
    public float duration = 3.0f; // Total time for the animation
    private RawImage rawImage;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Color startColor;
    private Color targetColor;

    // Store original size
    private Vector2 startSize;
    private Vector2 targetSize;

    // Rotation variables
    public float startRotation = 0f; // Starting rotation angle
    public float targetRotation = 360f; // Final rotation angle

    void Start()
    {
        rawImage = GetComponent<RawImage>();

        // Set initial and target positions
        startPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 20);
        targetPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);

        // Set initial color (fully transparent)
        startColor = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 0f);
        rawImage.color = startColor;

        // Target color (fully opaque)
        targetColor = new Color(rawImage.color.r, rawImage.color.g, rawImage.color.b, 1f);

        // Store original size
        startSize = Vector2.zero; // Starting size (0, 0)
        targetSize = rawImage.rectTransform.sizeDelta; // Original size

        // Set initial size
        rawImage.rectTransform.sizeDelta = startSize;

        // Start the animation
        StartCoroutine(FadeMoveScaleRotate());
    }

    System.Collections.IEnumerator FadeMoveScaleRotate()
    {
        float elapsedTime = 0;

        // Animate over time
        while (elapsedTime < duration)
        {
            // Move from start to target position
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);

            // Fade from transparent to opaque
            rawImage.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);

            // Scale from zero size to original size
            rawImage.rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, elapsedTime / duration);

            // Rotate from start rotation to target rotation
            float rotation = Mathf.Lerp(startRotation, targetRotation, elapsedTime / duration);
            transform.rotation = Quaternion.Euler(0f, 0f, rotation); // Rotate around the Z-axis

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure the final position, color, size, and rotation are correct
        transform.localPosition = targetPosition;
        rawImage.color = targetColor;
        rawImage.rectTransform.sizeDelta = targetSize; // Ensure the final size is original size
        transform.rotation = Quaternion.Euler(0f, 0f, targetRotation); // Ensure the final rotation is correct
    }
}
