using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CommanderCell : DMonoBehaviour
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
}