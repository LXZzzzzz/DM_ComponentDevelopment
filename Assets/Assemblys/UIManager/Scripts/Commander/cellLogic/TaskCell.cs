using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine.Events;

public class TaskCell : DraggingFunction
{
    private Toggle isComplete;
    private ITaskProgress tp;
    private Text taskProgress;
    private string taskName;

    public void Init(string taskIndex, ZiYuanBase ziYuan, Func<string, string, bool, bool> changeDataCallBack, UnityAction<bool,string> ctrlCmCb)
    {
        base.Init(ziYuan.BobjectId, changeDataCallBack, ctrlCmCb);
        isComplete = transform.Find("RootInfo/Tog_status").GetComponent<Toggle>();
        tp = ziYuan as ITaskProgress;
        taskName = taskIndex;
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

    public override string GetMyName()
    {
        return taskName;
    }
}