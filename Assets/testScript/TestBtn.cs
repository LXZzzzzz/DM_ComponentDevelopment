using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TestBtn : MonoBehaviour,IPointerDownHandler
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
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
}
