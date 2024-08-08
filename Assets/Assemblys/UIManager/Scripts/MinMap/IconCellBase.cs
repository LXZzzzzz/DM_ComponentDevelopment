using System;
using System.Collections.Generic;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class IconCellBase : DMonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
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
        if (!HasParent(eventData.pointerEnter, "Root")) return;
        var data = GetBasicInfo();
        if (data != null)
        {
            data.pointPos = GetComponent<RectTransform>().anchoredPosition + new Vector2(50, 40);
            UIManager.Instance.ShowPanel<UIHangShowInfo>(UIName.UIHangShowInfo, data);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.HidePanel(UIName.UIHangShowInfo.ToString());
    }

    public virtual void DestroyMe()
    {
    }

    public bool HasParent(GameObject child, string parentToCheck)
    {
        // 从child开始向上遍历父物体
        while (child != null)
        {
            // 检查当前父物体是否是我们要查找的
            if (child.name == parentToCheck)
            {
                return true; // 找到了指定的父物体
            }

            // 移动到下一个父物体
            child = child.transform.parent?.gameObject;
        }

        return false; // 没有找到指定的父物体
    }
}


public class IconInfoData
{
    public Vector2 pointPos;
    public string entityName;
    public string entityInfo;
    public List<string> beUseCommanders;
    public bool isAir = false;
    public float currentOilMass;
    public float maxOilMass;
    public float waterNum;
    public float goodsNum;
    public float personNum;
    public int personType;
}