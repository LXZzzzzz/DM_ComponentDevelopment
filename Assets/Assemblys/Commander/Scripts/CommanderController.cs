using System;
using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.PathPart;
using UnityEngine;
using EventType = ToolsLibrary.EventType;

public class CommanderController : DMonoBehaviour
{
    private EquipBase currentChooseEquip;

    public void Init()
    {
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip, OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<Vector3>(EventType.MoveToTarget, OnChangeTarget);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip, OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<Vector3>(EventType.MoveToTarget, OnChangeTarget);
    }

    private void OnChangeCurrentEquip(string equipId)
    {
        sender.LogError("收到了选择控制对象的数据" + equipId);
        currentChooseEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(equipId, x.BObjectId));
    }

    private void OnChangeTarget(Vector3 pos)
    {
        sender.LogError("收到了指定移动目标的数据" + pos);
        if (currentChooseEquip == null) return;
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)CommanderMain.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos));
    }

    public void MoveEquipToTarget(string data)
    {
        MsgReceive_Move(data, out string equipId, out Vector3 targetPos);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).MoveToTarget(targetPos);
    }

    #region 数据转换（消息的打包和解析）

    private string MsgSend_Move(string id, Vector3 pos)
    {
        return string.Format($"{id}_{pos.x}_{pos.y}_{pos.z}");
    }

    private void MsgReceive_Move(string data, out string id, out Vector3 pos)
    {
        string[] info = data.Split('_');
        id = info[0];
        pos = new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3]));
    }

    #endregion
}