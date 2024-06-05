using System;
using System.Collections.Generic;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class IconCellBase : DMonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    //关联组件ID(组件ID，或者自己生成的点id)
    private string _belongToId;

    private UnityAction<string, PointerEventData.InputButton> chooseCb;

    public string belongToId => _belongToId;


    public void Init(string belongToId, UnityAction<string, PointerEventData.InputButton> chooseCb)
    {
        gameObject.name = belongToId;
        _belongToId = belongToId;
        this.chooseCb = chooseCb;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //鼠标点击后，告诉对方我的类型
        chooseCb?.Invoke(_belongToId, eventData.button);
    }

    protected abstract IconInfoData GetBasicInfo();

    public void RefreshView()
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var data = GetBasicInfo();
        if (data != null)
        {
            data.pointPos = GetComponent<RectTransform>().anchoredPosition;
            UIManager.Instance.ShowPanel<UIHangShowInfo>(UIName.UIHangShowInfo, data);
        }
    }
}


public class IconInfoData
{
    public Vector2 pointPos;
    public string entityName;
    public string entityInfo;
    public List<string> beUseCommanders;
}