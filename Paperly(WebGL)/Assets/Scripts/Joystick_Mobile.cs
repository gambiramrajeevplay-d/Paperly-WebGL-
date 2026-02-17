using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick_Mobile : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;

    [SerializeField] private float returnSpeed = 10f; // Speed of auto return

    private bool isDragging = false;

    public float Horizontal => (handle.anchoredPosition.x / (background.sizeDelta.x / 2));
    public float Vertical => (handle.anchoredPosition.y / (background.sizeDelta.y / 2));

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos = Vector2.ClampMagnitude(pos, background.sizeDelta.x / 2);
        handle.anchoredPosition = pos;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void Update()
    {
        if (!isDragging)
        {
            handle.anchoredPosition = Vector2.Lerp(
                handle.anchoredPosition,
                Vector2.zero,
                returnSpeed * Time.deltaTime
            );
        }
    }
}
