using System;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using 导教端_WRJ;
using EventType = Enums.EventType;
using IWaterIntaking = ToolsLibrary.EquipPart.IWaterIntaking;
using Random = UnityEngine.Random;

public class CommanderController : DMonoBehaviour
{
    private EquipBase currentChooseEquip;
    private ZiYuanBase currentChooseZiYuan;

    private PDFReport _pdfReport;
    public List<string> clientOperatorInfos;

    private bool isMe;

    private void Start()
    {
        clientOperatorInfos = new List<string>();
    }

    public void Init()
    {
        sender.LogError("指挥端组件ID：" + main.BObjectId);
        isMe = true;
        _pdfReport = new PDFReport();
        MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.AddEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.AddEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.AddEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.AddEventListener<string>(EventType.SendWaterInfoToControler.ToString(), OnSendWaterIntaking);
        EventManager.Instance.AddEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.AddEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.AddEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.RemoveEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.RemoveEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.RemoveEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.RemoveEventListener<string>(EventType.SendWaterInfoToControler.ToString(), OnSendWaterIntaking);
        EventManager.Instance.RemoveEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.RemoveEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.RemoveEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
    }

    private DMCameraControl.DMCameraViewMove cvm;
    private DMCameraControl.DMouseOrbit mo;
    private DMCameraControl.ThirdCameraControl tc;

    private void OnCameraSwith(bool isMove)
    {
        if (cvm == null) cvm = Camera.main.gameObject.AddComponent<DMCameraControl.DMCameraViewMove>();
        if (mo == null) mo = Camera.main.gameObject.AddComponent<DMCameraControl.DMouseOrbit>();
        if (tc == null) tc = Camera.main.gameObject.AddComponent<DMCameraControl.ThirdCameraControl>();
        cvm.enabled = isMove;
        mo.enabled = isMove;
        tc.enabled = isMove;
        if (tc.enabled && tc.Target != null)
        {
            tc.Target = tc.Target;
            cvm.enabled = false;
            mo.enabled = false;
        }
    }

    private void OnCameraContral(int type, Transform target)
    {
        if (cvm == null) cvm = Camera.main.gameObject.AddComponent<DMCameraControl.DMCameraViewMove>();
        if (mo == null) mo = Camera.main.gameObject.AddComponent<DMCameraControl.DMouseOrbit>();
        if (tc == null) tc = Camera.main.gameObject.AddComponent<DMCameraControl.ThirdCameraControl>();
        switch (type)
        {
            case 1:
                cvm.enabled = true;
                mo.enabled = true;
                tc.enabled = false;
                Camera.main.transform.position = target.position + target.up * 35;
                Camera.main.transform.rotation = Quaternion.LookRotation(target.forward);
                Camera.main.transform.LookAt(target);
                break;
            case 2:
                cvm.enabled = false;
                mo.enabled = false;
                tc.enabled = true;
                tc.Target = target.gameObject;
                break;
        }
    }

    private void OnChangeCurrentEquip(string equipId)
    {
        // if (MyDataInfo.gameState != GameState.GameStart) return;
        if (string.IsNullOrEmpty(equipId))
        {
            if (currentChooseEquip != null)
                currentChooseEquip.isChooseMe = false;
            currentChooseEquip = null;
            return;
        }

        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(equipId, x.BObjectId));
        if (MyDataInfo.gameState == GameState.FirstLevelCommanderEditor || string.Equals(itemEquip.BeLongToCommanderId, MyDataInfo.leadId))
        {
            if (currentChooseEquip != null) currentChooseEquip.isChooseMe = false;
            currentChooseEquip = itemEquip;
            currentChooseEquip.isChooseMe = true;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", 100);
        }
        else
        {
            sender.LogError("该对象不属于我");
        }
    }

    private void OnChangeCurrentZiyuan(string ziyuanId)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(ziyuanId, allBObjects[i].BObject.Id))
            {
                var itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
                if (itemZy == null) return;
                if (currentChooseZiYuan != null) itemZy.isChooseMe = false;
                currentChooseZiYuan = itemZy;
                currentChooseZiYuan.isChooseMe = true;
                EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", currentChooseZiYuan);
                break;
            }
        }
    }

    private void OnChangeTarget(Vector3 pos)
    {
        if (MyDataInfo.gameState != GameState.GameStart || currentChooseEquip == null) return;
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos));
    }

    private void OnCreatEquipEntity(string templateId, string myId)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(templateId, allBObjects[i].BObject.Id))
            {
                var templateEquip = allBObjects[i].GetComponentInChildren<EquipBase>(true);
                var temporaryEquip = Instantiate(templateEquip, MyDataInfo.SceneGoParent);
                temporaryEquip.name = allBObjects[i].BObject.Info.Name + $"_000{MyDataInfo.sceneAllEquips.Count + 1}";
                temporaryEquip.BObjectId = myId;
                temporaryEquip.Init();
                temporaryEquip.BeLongToCommanderId = ProgrammeDataManager.Instance.GetEquipDataById(myId).controllerId;
                var dataPos = ProgrammeDataManager.Instance.GetEquipDataById(myId).pos;
                temporaryEquip.transform.position = new Vector3(dataPos.x, dataPos.y + 600, dataPos.z);
                EventManager.Instance.EventTrigger(EventType.CreatEquipCorrespondingIcon.ToString(), temporaryEquip);
                MyDataInfo.sceneAllEquips.Add(temporaryEquip);
                break;
            }
        }
    }

    private void OnLoadProgrammeDataSuc(ProgrammeData data)
    {
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 1);
        OnClearScene();
        for (int i = 0; i < data.AllEquipDatas.Count; i++)
        {
            OnCreatEquipEntity(data.AllEquipDatas[i].templateId, data.AllEquipDatas[i].myId);
        }

        //找到所有的资源，通过ID找到数据中的对应数据，
        for (int i = 0; i < allBObjects?.Length; i++)
        {
            ZiYuanBase itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
            if (itemZy == null) continue;
            itemZy.SetBeUsedComs(data.ZiYuanControlledList.ContainsKey(itemZy.main.BObjectId) ? data.ZiYuanControlledList[itemZy.main.BObjectId] : null);
            EventManager.Instance.EventTrigger(EventType.InitZiYuanBeUsed.ToString(), itemZy);
        }

        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
        EventManager.Instance.EventTrigger(EventType.ShowProgrammeName.ToString(), data.programmeName);
    }

    //清空场景中的所有方案数据
    private void OnClearScene()
    {
        //清空所有装备
        MyDataInfo.sceneAllEquips.ForEach(x => x.Destroy());
        //对所有资源的归属情况都清零
        for (int i = 0; i < allBObjects?.Length; i++)
        {
            ZiYuanBase itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
            if (itemZy == null) continue;
            itemZy.SetBeUsedComs(null);
            EventManager.Instance.EventTrigger(EventType.InitZiYuanBeUsed.ToString(), itemZy);
        }
    }

    private void OnSendWaterIntaking(string data)
    {
        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.TriggerWaterIntaking, data);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && MyDataInfo.MyLevel == 1 && isMe)
        {
            var player = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, MyDataInfo.leadId));

            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (!string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id) && allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) == null) continue;
                var itemCom = allBObjects[i].GetComponent<CommanderController>();
                clientOperatorInfos.AddRange(itemCom.clientOperatorInfos);
            }

            _pdfReport.CreateReport(player.UID, "一级指挥端", player.PlayerName, player.RoleId, clientOperatorInfos);
        }
    }

    #region 处理接收的消息

    public void Receive_MoveEquipToTarget(string data)
    {
        MsgReceive_Move(data, out string equipId, out Vector3 targetPos);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).MoveToTarget(targetPos);
        var item = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId));
        string who = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, item.BeLongToCommanderId)).PlayerName;
        if (clientOperatorInfos != null)
            clientOperatorInfos.Add($"玩家{who}控制飞机：{item.name}在{DateTime.Now}执行了机动指令，目标点为{targetPos}");
    }

    public void Receive_ProgrammeData(string data)
    {
        var programmeData = ProgrammeDataManager.Instance.UnPackingData(data);
        OnLoadProgrammeDataSuc(programmeData);
    }

    public void Receive_TriggerWaterIntaking(string data)
    {
        MsgReceive_Water(data, out string id, out Vector3 pos, out float amount);
        (MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id)) as IWaterIntaking).WaterIntaking(pos, 10, amount, true);


        var item = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id));
        string who = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, item.BeLongToCommanderId)).PlayerName;
        if (clientOperatorInfos != null)
            clientOperatorInfos.Add($"玩家{who}控制飞机：{item.name}在{DateTime.Now}执行了取水指令，取水目标点为{pos}");
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