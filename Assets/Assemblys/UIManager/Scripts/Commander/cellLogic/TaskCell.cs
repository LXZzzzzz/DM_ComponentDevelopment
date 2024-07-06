using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ToolsLibrary;
using ToolsLibrary.EquipPart;

public class TaskCell : DraggingFunction
{
    private Toggle isComplete;
    private ITaskProgress tp;
    private Text taskProgress;

    public void Init(string taskIndex, ZiYuanBase ziYuan, Func<string, string, bool, bool> changeDataCallBack)
    {
        base.Init(ziYuan.BobjectId, changeDataCallBack);
        isComplete = transform.Find("RootInfo/Tog_status").GetComponent<Toggle>();
        tp = ziYuan as ITaskProgress;
        transform.Find("RootInfo/Text_taskIndex").GetComponentInChildren<Text>().text = taskIndex;
        taskProgress = transform.Find("RootInfo/Text_taskName").GetComponentInChildren<Text>();
        transform.Find("describe/Text_taskDescribe").GetComponentInChildren<Text>().text = ziYuan.ziYuanDescribe;
        GetComponentInChildren<Button>().onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.ChooseZiyuan.ToString(), ziYuan.BobjectId));
    }

    private void LateUpdate()
    {
        if (tp == null) return;
        isComplete.isOn = tp.getTaskProgress(out string progressInfo);
        taskProgress.text = progressInfo;
    }
}