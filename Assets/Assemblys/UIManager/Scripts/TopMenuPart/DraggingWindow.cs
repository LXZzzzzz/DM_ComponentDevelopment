using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingWindow : DMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform myRect;
    private Vector2 lastPos;
    private Vector2 offset;
    private bool isFollow;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
        isFollow = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPos = eventData.position;
        isFollow = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isFollow = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (isFollow)
        {
            offset = eventData.position - lastPos;
            myRect.anchoredPosition += offset;
            lastPos = eventData.position;
        }
    }

    private void Update()
    {
        if (isFollow)
        {
            offset = (Vector2)Input.mousePosition - lastPos;
            myRect.anchoredPosition += offset;
            lastPos = (Vector2)Input.mousePosition;
        }
    }
}