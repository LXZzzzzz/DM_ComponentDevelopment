using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;

public class ZiYuanCell : DraggingFunction
{
    private ZiYuanBase _ziYuan;
    private float checkTimer;
    private GameObject chooseImg;

    public void Init(string pointName, string entityId, ZiYuanBase ziyuan, Func<string, string, bool, bool> changeDataCallBack)
    {
        base.Init(entityId, changeDataCallBack);

        _ziYuan = ziyuan;
        chooseImg = transform.Find("ChooseImg").gameObject;
        transform.Find("Text_name").GetComponent<Text>().text = pointName;
        transform.Find("Text_describe").GetComponent<Text>().text = pointName;
        GetComponent<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.ChooseZiyuan.ToString(), entityId));
    }

    private void Update()
    {
        if (Time.time > checkTimer)
        {
            checkTimer = Time.time + 1 / 25f;
            chooseImg.SetActive(_ziYuan.isChooseMe);
        }
    }
}