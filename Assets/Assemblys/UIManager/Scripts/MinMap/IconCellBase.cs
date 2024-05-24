using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class IconCellBase : DMonoBehaviour, IPointerClickHandler
{
    //关联组件ID(组件ID，或者自己生成的点id)
    private string _belongToId;

    private UnityAction<string> chooseCb;
    
    public string belongToId => _belongToId;
    

    public void Init(string belongToId, UnityAction<string> chooseCb)
    {
        gameObject.name = belongToId;
        _belongToId = belongToId;
        this.chooseCb = chooseCb;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //鼠标点击后，告诉对方我的类型
        chooseCb?.Invoke(_belongToId);
    }

    public void RefreshView()
    {
    }
    
}