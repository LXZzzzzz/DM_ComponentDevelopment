using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public abstract class IconCellBase : MonoBehaviour, IPointerClickHandler
{
    //关联组件ID(组件ID，或者自己生成的点id)
    private string _belongToId;

    private UnityAction<string> chooseCb;

    //记录所有经过我的点
    private List<string> _allViaPointIds;

    public string belongToId => _belongToId;

    public List<string> allViaPointIds => _allViaPointIds;

    public virtual void Init(string belongToId, UnityAction<string> chooseCb)
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
    public void AddAttachedPoint(string pointId)
    {
        _allViaPointIds ??= new List<string>();
        _allViaPointIds.Add(pointId);
    }

    public void RefreshView()
    {
        
    }
}