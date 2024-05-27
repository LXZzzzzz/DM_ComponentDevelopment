using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using EquipBase = ToolsLibrary.EquipPart.EquipBase;
using EventType = Enums.EventType;

public class CommanderController : DMonoBehaviour
{
    private EquipBase currentChooseEquip;

    public void Init()
    {
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.AddEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.AddEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.AddEventListener<string>(EventType.SendWaterInfoToControler.ToString(), OnSendWaterIntaking);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.RemoveEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.RemoveEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.RemoveEventListener<string>(EventType.SendWaterInfoToControler.ToString(), OnSendWaterIntaking);
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
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos));
    }

    private void OnCreatEquipEntity(string templateId, string myId)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(templateId, allBObjects[i].BObject.Id))
            {
                var templateEquip = allBObjects[i].GetComponentInChildren<EquipBase>();
                var temporaryEquip = Instantiate(templateEquip, root);
                temporaryEquip.BObjectId = myId;
                temporaryEquip.Init();
                var dataPos = ProgrammeDataManager.Instance.GetEquipDataById(myId).pos;
                temporaryEquip.transform.position = new Vector3(dataPos.x, dataPos.y, dataPos.z);
                EventManager.Instance.EventTrigger(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), temporaryEquip);
                MyDataInfo.sceneAllEquips.Add(temporaryEquip);
                break;
            }
        }
    }

    private void OnLoadProgrammeDataSuc(ProgrammeData data)
    {
        for (int i = 0; i < data.AllEquipDatas.Count; i++)
        {
            OnCreatEquipEntity(data.AllEquipDatas[i].templateId, data.AllEquipDatas[i].myId);
        }

        EventManager.Instance.EventTrigger<object>(EventType.SwitchCreatModel.ToString(), MyDataInfo.sceneAllEquips);
    }

    private void OnSendWaterIntaking(string data)
    {
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.TriggerWaterIntaking, data);
    }


    #region 处理接收的消息

    public void Receive_MoveEquipToTarget(string data)
    {
        MsgReceive_Move(data, out string equipId, out Vector3 targetPos);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).MoveToTarget(targetPos);
    }

    public void Receive_ProgrammeData(string data)
    {
        var programmeData = ProgrammeDataManager.Instance.UnPackingData(data);
        Debug.LogError(programmeData);
        for (int i = 0; i < programmeData.AllEquipDatas.Count; i++)
        {
            OnCreatEquipEntity(programmeData.AllEquipDatas[i].templateId, programmeData.AllEquipDatas[i].myId);
        }

        EventManager.Instance.EventTrigger<object>(EventType.SwitchCreatModel.ToString(), MyDataInfo.sceneAllEquips);
    }

    public void Receive_TriggerWaterIntaking(string data)
    {
        MsgReceive_Water(data, out string id, out Vector3 pos, out float amount);
        (MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id)) as IWaterIntaking).WaterIntaking(pos, 10, amount, true);
    }

    #endregion

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

    private void MsgReceive_Water(string data, out string id, out Vector3 pos, out float amount)
    {
        string[] info = data.Split('_');
        pos = new Vector3(float.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]));
        amount = float.Parse(info[3]);
        id = info[4];
    }

    #endregion
}