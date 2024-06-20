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
    private GameObject selectImg;

    public void Init(string Namestr, string myId, UnityAction<string> callBack)
    {
        if (zuJianName == null) zuJianName = transform.Find("text_AssemblyName").GetComponent<Text>();
        if (playerName == null) playerName = transform.Find("text_ClientName").GetComponent<Text>();
        selectImg = transform.Find("bg_Select").gameObject;
        zuJianName.text = Namestr;
        transform.Find("bg_Normal").GetComponent<Image>().color = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, myId)).MyColor;
        if (MyDataInfo.playerInfos != null)
            playerName.text = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, myId)).PlayerName;
        this.myId = myId;
        GetComponentInChildren<Button>().onClick.AddListener(() => callBack(this.myId));
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.Instance.EventTrigger(EventType.AddCommanderForZiYuan.ToString(), myId);
    }

    public void SetSelect(bool isChoose)
    {
        selectImg?.SetActive(isChoose);
    }
}