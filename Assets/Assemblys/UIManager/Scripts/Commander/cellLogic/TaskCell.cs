using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine.Events;

public class TaskCell : DMonoBehaviour
{
    private Toggle isComplete;
    private ITaskProgress tp;
    private Text taskProgress;
    private Slider slider_Progress;

    public string myEntityId => tp.getAssociationAssemblyId();

    public void Init(string taskIndex, ZiYuanBase ziYuan)
    {
        tp = ziYuan as ITaskProgress;
        isComplete = transform.Find("RootInfo/Tog_status").GetComponent<Toggle>();
        transform.Find("RootInfo/Text_zaiQuName").GetComponent<Text>().text = ziYuan.ziYuanName;
        transform.Find("RootInfo/Text_taskIndex").GetComponentInChildren<Text>().text = taskIndex + ':';
        taskProgress = transform.Find("RootInfo/Text_taskName").GetComponentInChildren<Text>();
        slider_Progress = transform.Find("RootInfo/Slider_Progress").GetComponentInChildren<Slider>();
        // transform.Find("describe/Text_taskDescribe").GetComponentInChildren<Text>().text = ziYuan.ziYuanDescribe;
        GetComponentInChildren<Button>().onClick.AddListener(() =>
            EventManager.Instance.EventTrigger(Enums.EventType.DqChooseGo.ToString(), tp.getAssociationAssemblyId()));
    }

    private void Update()
    {
        if (tp == null) return;
        isComplete.isOn = tp.getTaskProgress(out string progressInfo, out float progressNum);
        taskProgress.text = progressInfo;
        slider_Progress.value = progressNum;
    }
}