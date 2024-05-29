using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class CommanderCell : DMonoBehaviour, IDropHandler
{
    private Text showName;
    private string myId;

    public void Init(string Namestr, string myId, UnityAction<string> callBack)
    {
        if (showName == null)
            showName = GetComponentInChildren<Text>();
        showName.text = Namestr;
        this.myId = myId;
        GetComponentInChildren<Button>().onClick.AddListener(() => callBack(this.myId));
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.Instance.EventTrigger(EventType.AddCommanderForZiYuan.ToString(), myId);
    }
}