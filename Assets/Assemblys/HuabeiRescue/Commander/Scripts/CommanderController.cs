using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class CommanderController : DMonoBehaviour
{
    private EquipBase currentChooseEquip;
    private Dictionary<string, int> EquipTemp_EntityNums;

    public void Init()
    {
        EquipTemp_EntityNums = new Dictionary<string, int>();
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.AddEventListener<string,Vector3>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.RemoveEventListener<string,Vector3>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
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

    private void OnCreatEquipEntity(string templateId,Vector3 creatPos)
    {
        if (!EquipTemp_EntityNums.ContainsKey(templateId))
            EquipTemp_EntityNums.Add(templateId,1);
        
        //找到模板装备，执行拷贝逻辑
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(templateId,allBObjects[i].BObject.Id))
            {
                var templateEquip = allBObjects[i].GetComponentInChildren<EquipBase>();
                sender.LogError("要创建的对象："+templateEquip);
                sender.LogError("父物体："+root);
                var temporaryEquip = Instantiate(templateEquip, root);
                temporaryEquip.transform.position = creatPos;
                temporaryEquip.BObjectId = templateId + EquipTemp_EntityNums[templateId];
                EventManager.Instance.EventTrigger(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), temporaryEquip);
                MyDataInfo.sceneAllEquips.Add(temporaryEquip);
                EquipTemp_EntityNums[templateId]++;
                break;
            }
        }
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