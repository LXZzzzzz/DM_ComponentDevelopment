using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class DraggingFunction : DMonoBehaviour, IDragHandler
{
    private string _myEntityId;
    private Func<string, string, bool, bool> changeDataCallBack;
    private RectTransform comShowParent;
    private ZiYuan_ComanderCell commanderShowCell;
    private bool isInit;
    private List<ZiYuan_ComanderCell> _allcoms;

    public string myEntityId => _myEntityId;

    public List<ZiYuan_ComanderCell> allcoms => _allcoms;

    public void Init(string entityId, Func<string, string, bool, bool> changeDataCallBack)
    {
        //显示名称，存储对应实体ID，界面显示自己都可被哪些人使用
        isAddEvent = true;
        comShowParent = GetComponentInChildren<ScrollRect>(true).content;
        commanderShowCell = GetComponentInChildren<ZiYuan_ComanderCell>(true);


        _myEntityId = entityId;
        this.changeDataCallBack = changeDataCallBack;
        _allcoms = new List<ZiYuan_ComanderCell>();
    }

    public void ShowComCtrls(List<string> canBeUseCommanders)
    {
        isInit = true;
        for (int i = 0; i < allcoms.Count;)
        {
            ChangeCommanders(allcoms[i].comId, false);
        }

        for (int i = 0; i < canBeUseCommanders?.Count; i++)
        {
            ChangeCommanders(canBeUseCommanders[i], true);
        }

        isInit = false;
    }

    private void Update()
    {
        if (!isAddEvent && Input.GetMouseButtonUp(0))
        {
            isAddEvent = true;
            EventManager.Instance.RemoveEventListener<string>(EventType.AddCommanderForZiYuan.ToString(), OnAddCommander);
        }
    }

    private bool isAddEvent;

    public void OnDrag(PointerEventData eventData)
    {
        if (MyDataInfo.gameState == GameState.GameStart) return;
        //注册事件
        if (isAddEvent)
        {
            isAddEvent = false;
            EventManager.Instance.AddEventListener<string>(EventType.AddCommanderForZiYuan.ToString(), OnAddCommander);
        }
    }

    private void OnAddCommander(string commanderId)
    {
        isAddEvent = true;
        EventManager.Instance.RemoveEventListener<string>(EventType.AddCommanderForZiYuan.ToString(), OnAddCommander);
        ChangeCommanders(commanderId, true);
    }

    private void ChangeCommanders(string comId, bool isAdd)
    {
        if (isInit || changeDataCallBack(_myEntityId, comId, isAdd))
        {
            //操作界面显示
            if (isAdd)
            {
                var itemCom = Instantiate(commanderShowCell, comShowParent);
                itemCom.Init(getComName(comId), comId, ChangeCommanders);
                itemCom.gameObject.SetActive(true);
                allcoms.Add(itemCom);
            }
            else
            {
                var removeItem = allcoms.Find(x => string.Equals(comId, x.comId));
                allcoms.Remove(removeItem);
                Destroy(removeItem.gameObject);
            }
        }
    }


    private string getComName(string id)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, id))
            {
                return allBObjects[i].BObject.Info.Name;
            }
        }

        return "未找到指定ID指挥端";
    }
}