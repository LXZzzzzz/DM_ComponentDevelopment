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
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (string.Equals(allBObjects[i].BObject.Id, myId))
                {
                    playerName.text = allBObjects[i].BObject.Info.Name;
                    break;
                }
            }
        }

        this.myId = myId;
        GetComponentInChildren<Button>().onClick.AddListener(() => callBack(this.myId));
    }

    public void OnDrop(PointerEventData eventData)
    {
        EventManager.Instance.EventTrigger(EventType.AddCommanderForZiYuan.ToString(), myId);
    }
}