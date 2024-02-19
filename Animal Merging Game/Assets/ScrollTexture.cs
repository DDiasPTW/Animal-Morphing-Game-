using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
   public float scrollSpeed = 50f; // Speed of the scroll
    private RectTransform rectTransform;
    private float initialY;
    [SerializeField] private float resetThreshold;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialY = rectTransform.anchoredPosition.y;
        // Calculate the point at which to reset the position so the sprite appears to loop infinitely.
        // Since the sprite is taller than the canvas, we adjust when its bottom exits the canvas completely.
        resetThreshold = -(rectTransform.sizeDelta.y - Screen.height)/3;
    }

    void Update()
    {
        // Move the sprite down
        rectTransform.anchoredPosition -= new Vector2(0, scrollSpeed * Time.deltaTime);

        // Check if the sprite has scrolled past the reset threshold
        if (rectTransform.anchoredPosition.y <= resetThreshold)
        {
            // Reset position to give an infinite scroll effect
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, initialY);
        }
    }
}
