using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZiYuan_ComanderCell : DMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UnityAction<string, bool> removeCallBack;
    private string _comId;
    private Button removeBtn;

    public string comId => _comId;

    public void Init(string comName, string comId, UnityAction<string, bool> removeCallBack)
    {
        this._comId = comId;
        this.removeCallBack = removeCallBack;

        GetComponentInChildren<Text>(true).text = comName;
        removeBtn = GetComponentInChildren<Button>(true);
        removeBtn.onClick.AddListener(OnRemoveMe);
        removeBtn.gameObject.SetActive(false);
    }

    private void OnRemoveMe()
    {
        removeCallBack(_comId, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        removeBtn.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        removeBtn.gameObject.SetActive(false);
    }
}