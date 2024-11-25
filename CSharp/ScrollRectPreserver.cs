using UnityEngine;
using UnityEngine.UI;

public class ScrollRectPreserver : MonoBehaviour
{
    private ScrollRect scrollRect;
    private Vector2 previousContentPosition;

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void SaveScrollPosition()
    {
        previousContentPosition = scrollRect.content.anchoredPosition;
    }

    public void RestoreScrollPosition()
    {
        Canvas.ForceUpdateCanvases();
        scrollRect.content.anchoredPosition = previousContentPosition;
    }
}
