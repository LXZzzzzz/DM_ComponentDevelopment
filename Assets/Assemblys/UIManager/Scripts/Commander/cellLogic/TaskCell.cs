using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TaskCell : DraggingFunction
{
    //todo:还需要实现任务完成的标识
    public void Init(string taskIndex, string taskInfoStr, string entityId, Func<string, string, bool, bool> changeDataCallBack)
    {
        base.Init(entityId, changeDataCallBack);
        transform.Find("Text_taskIndex").GetComponentInChildren<Text>().text = taskIndex;
        transform.Find("Text_taskDescribe").GetComponentInChildren<Text>().text = taskInfoStr;
    }
}