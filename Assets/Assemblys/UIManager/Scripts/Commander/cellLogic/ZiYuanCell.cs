using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ZiYuanCell : DraggingFunction
{
    private ZiYuanBase _ziYuan;
    private float checkTimer;
    private GameObject chooseImg;

    public void Init(string pointName, string entityId, ZiYuanBase ziyuan, Func<string, string, bool, bool> changeDataCallBack, UnityAction<bool,string> ctrlCmCb)
    {
        base.Init(entityId, changeDataCallBack, ctrlCmCb);

        _ziYuan = ziyuan;
        chooseImg = transform.Find("ChooseImg").gameObject;
        transform.Find("Text_name").GetComponent<Text>().text = _ziYuan.ziYuanName;
        transform.Find("Text_describe").GetComponent<Text>().text = _ziYuan.ziYuanDescribe;
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.ChooseZiyuan.ToString(), entityId));
    }

    private void LateUpdate()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_ziYuan.isChooseMe);
        }
    }

    public override string GetMyName()
    {
        return _ziYuan.ziYuanName;
    }
}