using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestBtn : MonoBehaviour,IPointerDownHandler
{
    public float maxDistance=200;
    private Vector3 initialScale;
    private void Start()
    {
        // GetComponent<Button>().onClick.AddListener(OnClick);
        initialScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button== PointerEventData.InputButton.Right)
        {
            OnClick();
        }
    }

    private void OnClick()
    {
        Debug.LogError("点击上了");
    }

    private void Update()
    {
        // 根据距离调整物体的缩放比例
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance > maxDistance)
        {
            float scaleFactor = distance / maxDistance;
            transform.localScale = initialScale * scaleFactor;
        }
        else
        {
            transform.localScale = initialScale;
        }

    }
}
