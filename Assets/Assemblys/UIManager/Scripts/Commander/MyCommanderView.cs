using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class MyCommanderView : DMonoBehaviour, IDropHandler
{
    private Text playerName;
    private string myId;

    public void Init(string myId, UnityAction<string> callBack)
    {
        if (playerName == null) playerName = GetComponentInChildren<Text>();
        if (MyDataInfo.playerInfos != null)
            playerName.text = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, myId)).PlayerName;
        this.myId = myId;
        GetComponentInChildren<Button>().onClick.AddListener(() => callBack(this.myId));
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.Instance.EventTrigger(EventType.AddCommanderForZiYuan.ToString(), myId);
    }
}