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
    private Text zuJianName;
    private Text playerName;
    private string myId;

    public void Init(string Namestr, string myId, UnityAction<string> callBack)
    {
        if (zuJianName == null) zuJianName = transform.Find("text_AssemblyName").GetComponent<Text>();
        if (playerName == null) playerName = transform.Find("text_ClientName").GetComponent<Text>();
        zuJianName.text = Namestr;
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