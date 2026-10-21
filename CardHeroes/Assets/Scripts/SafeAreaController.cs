using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaController : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect lastSafeArea = Rect.zero;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        AplicarSafeArea();
    }

    void Update()
    {
        if (Screen.safeArea != lastSafeArea)
        {
            AplicarSafeArea();
        }
    }

    void AplicarSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }
}