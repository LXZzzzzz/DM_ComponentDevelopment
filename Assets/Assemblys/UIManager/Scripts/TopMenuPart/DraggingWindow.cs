using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggingWindow : DMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform myRect;
    private Vector2 lastPos;
    private Vector2 offset;
    private bool isFollow;
    private float xbl;
    private float ybl;

    private void Start()
    {
        myRect = GetComponent<RectTransform>();
        isFollow = false;
        Vector2 canVector2 = GetComponentInParent<Canvas>().GetComponent<RectTransform>().sizeDelta;
        xbl = Screen.width / canVector2.x;
        ybl = Screen.height / canVector2.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPos = new Vector2(eventData.position.x / xbl, eventData.position.y / ybl);
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
            Vector2 nowPos = new Vector2(Input.mousePosition.x / xbl, Input.mousePosition.y / ybl);

            offset = nowPos - lastPos;
            myRect.anchoredPosition += offset;
            lastPos = nowPos;
        }
    }
}