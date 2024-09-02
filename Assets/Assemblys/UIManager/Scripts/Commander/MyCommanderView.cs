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
    private Text playerLevelName;
    private Text playerName;
    private string myId;

    public void Init(string myId, UnityAction<string> callBack)
    {
        if (playerLevelName == null) playerLevelName = GetComponentInChildren<Text>();
        if (playerName == null) playerName = transform.Find("text_ClientName").GetComponent<Text>();
        if (MyDataInfo.playerInfos != null)
        {
            var myInfo = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, myId));
            playerLevelName.text = myInfo.ClientLevelName;
            playerName.text = myInfo.PlayerName;
        }

        this.myId = myId;
        GetComponentInChildren<Button>().onClick.AddListener(() => callBack(this.myId));
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.Instance.EventTrigger(EventType.AddCommanderForZiYuan.ToString(), myId);
    }
}