using System;
using System.Collections.Generic;
using DM.IFS;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using UnityEngine.Events;
using 导教端_WRJ;
using EventType = Enums.EventType;

public partial class CommanderController : DMonoBehaviour
{
    private EquipBase currentChooseEquip;
    private ZiYuanBase currentChooseZiYuan;

    private PDFReport _pdfReport;
    public List<string> clientOperatorInfos;
    private List<string> showAllOperatorInfos;
    private List<ZiYuanBase> sceneAllzy;
    public int gameType;

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
        EventManager.Instance.AddEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.AddEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.AddEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.AddEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.AddEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.RemoveEventListener<Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.RemoveEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.RemoveEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.RemoveEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.RemoveEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.RemoveEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.RemoveEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.RemoveEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
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
                Camera.main.transform.position = target.position + target.up * 200;
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
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
            return;
        }

        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(equipId, x.BObjectId));

        if (MyDataInfo.gameState == GameState.FirstLevelCommanderEditor || string.Equals(itemEquip.BeLongToCommanderId, MyDataInfo.leadId))
        {
            if (currentChooseEquip != null) currentChooseEquip.isChooseMe = false;
            currentChooseEquip = itemEquip;
            currentChooseEquip.isChooseMe = true;
            currentChooseEquip.CurrentChooseSkillType = SkillType.None;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", currentChooseEquip);
        }
        else
        {
            sender.LogError("该对象不属于我");
        }
    }

    private void OnChangeCurrentZiyuan(string ziyuanId)
    {
        if (string.IsNullOrEmpty(ziyuanId))
        {
            if (currentChooseZiYuan != null)
                currentChooseZiYuan.isChooseMe = false;
            currentChooseZiYuan = null;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
            return;
        }

        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(ziyuanId, allBObjects[i].BObject.Id))
            {
                var itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
                if (itemZy == null) return;
                if (currentChooseZiYuan != null) currentChooseZiYuan.isChooseMe = false;
                currentChooseZiYuan = itemZy;
                currentChooseZiYuan.isChooseMe = true;
                OnCameraContral(1, currentChooseZiYuan.transform);
                EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", currentChooseZiYuan);
                break;
            }
        }
    }

    private void OnChangeTarget(Vector3 pos)
    {
        if (MyDataInfo.gameState != GameState.GameStart || currentChooseEquip == null) return;
        if (currentChooseEquip.isDockingAtTheAirport)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前装备在机场未出库");
            return;
        }

        if (!currentChooseEquip.OnCheckIsMove()) return;

        sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos));
    }

    private bool isLoad = false;

    private void OnCreatEquipLoad(string templateId, string myId)
    {
        isLoad = true;
        OnCreatEquipEntity(templateId, myId);
        isLoad = false;
    }

    private void OnCreatEquipEntity(string templateId, string myId)
    {
        if (sceneAllzy == null)
        {
            sceneAllzy = new List<ZiYuanBase>();
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
                {
                    sceneAllzy.Add(allBObjects[i].GetComponent<ZiYuanBase>());
                }
            }
        }

        IAirPort myAirPort = null;
        if (!string.IsNullOrEmpty(ProgrammeDataManager.Instance.GetEquipDataById(myId).airportId))
        {
            //找到机场，停机
            string airPortId = ProgrammeDataManager.Instance.GetEquipDataById(myId).airportId;
            for (int j = 0; j < allBObjects.Length; j++)
            {
                if (string.Equals(airPortId, allBObjects[j].BObject.Id) && allBObjects[j].GetComponent<ZiYuanBase>() != null)
                {
                    myAirPort = allBObjects[j].GetComponent<ZiYuanBase>() as IAirPort;
                    break;
                }
            }
        }

        if (myAirPort != null && !myAirPort.checkComeIn())
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "该机场已停满！");
            return;
        }

        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(templateId, allBObjects[i].BObject.Id))
            {
                var templateEquip = allBObjects[i].GetComponentInChildren<EquipBase>(true);
                var temporaryEquip = Instantiate(templateEquip, MyDataInfo.SceneGoParent);
                if (!isLoad)
                {
                    temporaryEquip.name = allBObjects[i].BObject.Info.Name + $"_00{MyDataInfo.sceneAllEquips.Count + 1}";
                    ProgrammeDataManager.Instance.GetEquipDataById(myId).myName = temporaryEquip.name;
                }
                else
                    temporaryEquip.name = ProgrammeDataManager.Instance.GetEquipDataById(myId).myName;

                temporaryEquip.BObjectId = myId;
                temporaryEquip.BeLongToCommanderId = ProgrammeDataManager.Instance.GetEquipDataById(myId).controllerId;
                var dataPos = ProgrammeDataManager.Instance.GetEquipDataById(myId).pos;
                temporaryEquip.transform.position = new Vector3(dataPos.x, dataPos.y + 700, dataPos.z);
                temporaryEquip.Init(templateEquip, sceneAllzy);
                temporaryEquip.gameObject.SetActive(true);
                EventManager.Instance.EventTrigger(EventType.CreatEquipCorrespondingIcon.ToString(), temporaryEquip);
                MyDataInfo.sceneAllEquips.Add(temporaryEquip);
                if (myAirPort != null) myAirPort.comeIn(myId);
                else temporaryEquip.isDockingAtTheAirport = false;

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
            OnCreatEquipLoad(data.AllEquipDatas[i].templateId, data.AllEquipDatas[i].myId);
        }

        //找到所有的资源，通过ID找到数据中的对应数据，
        for (int i = 0; i < allBObjects?.Length; i++)
        {
            ZiYuanBase itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
            if (itemZy == null) continue;
            itemZy.SetBeUsedComs(data.ZiYuanControlledList.ContainsKey(itemZy.main.BObjectId) ? data.ZiYuanControlledList[itemZy.main.BObjectId] : null);
            EventManager.Instance.EventTrigger(EventType.InitZiYuanBeUsed.ToString(), itemZy);
        }

        if (MyDataInfo.MyLevel != 1) //一级指挥端当前还开放编辑模式
            EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
        EventManager.Instance.EventTrigger(EventType.ShowProgrammeName.ToString(), data.programmeName);
    }

    //清空场景中的所有方案数据
    private void OnClearScene()
    {
        //清空所有装备
        MyDataInfo.sceneAllEquips.ForEach(x => x.Destroy());
        MyDataInfo.sceneAllEquips.Clear();
        //对所有资源的归属情况都清零
        for (int i = 0; i < allBObjects?.Length; i++)
        {
            ZiYuanBase itemZy = allBObjects[i].GetComponent<ZiYuanBase>();
            if (itemZy == null) continue;
            itemZy.Reset();
            EventManager.Instance.EventTrigger(EventType.InitZiYuanBeUsed.ToString(), itemZy);
        }
    }

    private void OnSendSkillInfo(int messageID, string data)
    {
        sender.RunSend(SendType.SubToMain, main.BObjectId, messageID, data);
    }

    private List<string> itemclientInfos;

    private void OnGeneratePdf()
    {
        if (MyDataInfo.sceneAllEquips.Find(x => !x.isCrash && !x.isDockingAtTheAirport) != null)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前有飞机未入库机场，无法生成报告！");
            return;
        }

        if (showAllOperatorInfos == null) showAllOperatorInfos = new List<string>();
        if (itemclientInfos == null) itemclientInfos = new List<string>();
        showAllOperatorInfos.Clear();
        itemclientInfos.Clear();
        itemclientInfos.AddRange(clientOperatorInfos);
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id) || allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) == null) continue;
            var itemCom = allBObjects[i].GetComponent<CommanderController>();
            itemclientInfos.AddRange(itemCom.clientOperatorInfos);
        }

        itemclientInfos.Sort((x, y) => float.Parse(x.Split("--")[0]).CompareTo(float.Parse(y.Split("--")[0])));

        for (int i = 0; i < itemclientInfos.Count; i++)
        {
            var itemaa = itemclientInfos[i].Split("--");
            showAllOperatorInfos.Add(ConvertSecondsToHHMMSS(float.Parse(itemaa[0])) + "    " + itemaa[1]);
        }

        if (MyDataInfo.sceneAllEquips.Count == 0) return;
        // clientOperatorInfos.sore
        if (gameType == 1)
            GenerateFireExtinguishingReport();
        if (gameType == 2)
            GenerateRescueReport();

        // EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "成功生成评估报告PDF");
    }

    #region 处理接收的消息

    public void Receive_GameStart()
    {
        if (sceneAllzy != null)
            sceneAllzy.ForEach(a => a.OnStart());
    }

    public void Receive_MoveEquipToTarget(string data)
    {
        MsgReceive_Move(data, out string equipId, out Vector3 targetPos);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).MoveToTarget(targetPos);
        var item = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId));
        string who = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, item.BeLongToCommanderId)).PlayerName;
        var playerData = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, item.BeLongToCommanderId));
        EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerData.ColorCode}>{playerData.PlayerName}</color> {item.name}执行机动操作，目标点为{targetPos}");
        clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerData.ClientLevelName}】{item.name}执行机动操作，目标点为{targetPos}");
    }

    public void Receive_ProgrammeData(string data)
    {
        var programmeData = ProgrammeDataManager.Instance.UnPackingData(data);
        OnLoadProgrammeDataSuc(programmeData);
    }


    public void Receive_GameStop()
    {
        if (MyDataInfo.sceneAllEquips.Find(x => !x.isCrash && !x.isDockingAtTheAirport) == null)
            OnGeneratePdf();
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) == null) continue;
            var itemCom = allBObjects[i].GetComponent<CommanderController>();
            itemCom.clientOperatorInfos.Clear();
        }

        EventManager.Instance.EventTrigger(EventType.ClearMsgBox.ToString());
        currentChooseEquip = null;
        OnLoadProgrammeDataSuc(ProgrammeDataManager.Instance.GetCurrentData);
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);
        EventManager.Instance.EventTrigger(EventType.GameStop.ToString());
    }

    public void Receive_TriggerSkill(MessageID messageID, string data)
    {
        if (sceneAllzy == null)
        {
            sceneAllzy = new List<ZiYuanBase>();
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
                {
                    sceneAllzy.Add(allBObjects[i].GetComponent<ZiYuanBase>());
                }
            }
        }

        switch (messageID)
        {
            case MessageID.TriggerGroundReady:
                sender.LogError("收到了起飞前准备的指令");
                var itemAirportId = ProgrammeDataManager.Instance.GetEquipDataById(data).airportId;
                if (string.IsNullOrEmpty(itemAirportId))
                {
                    EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "数据错误，指定的飞机未在机场");
                    return;
                }

                var item = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (item as IGroundReady)?.GroundReady(sceneAllzy.Find(a => string.Equals(a.BobjectId, itemAirportId)) as IAirPort);
                var playerData = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, item.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerData.ColorCode}>{playerData.PlayerName}</color> {item.name}执行起飞前准备操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerData.ClientLevelName}】{item.name}执行起飞前准备操作");
                break;
            case MessageID.TriggerBePutInStorage:
                sender.LogError("收到了入库的指令");
                var itema = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itema as IGroundReady)?.BePutInStorage();
                var playerDataa = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itema.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataa.ColorCode}>{playerDataa.PlayerName}</color> {itema.name}执行入库操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataa.ClientLevelName}】{itema.name}执行入库操作");
                break;
            case MessageID.TriggerTakeOff:
                sender.LogError("收到了起飞的指令");
                var itemb = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemb as ITakeOffAndLand)?.TakeOff();
                var playerDatab = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemb.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatab.ColorCode}>{playerDatab.PlayerName}</color> {itemb.name}执行起飞操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatab.ClientLevelName}】{itemb.name}执行起飞操作");
                break;
            case MessageID.TriggerLanding:
                sender.LogError("收到了降落的指令");
                var itemc = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemc as ITakeOffAndLand)?.Landing();
                var playerDatac = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemc.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatac.ColorCode}>{playerDatac.PlayerName}</color> {itemc.name}执行降落操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatac.ClientLevelName}】{itemc.name}执行降落操作");
                break;
            case MessageID.TriggerSupply:
                sender.LogError("收到了补给的指令");
                var itemS = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemS as ISupply)?.Supply();
                var playerDataS = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemS.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataS.ColorCode}>{playerDataS.PlayerName}</color> {itemS.name}执行补给操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataS.ClientLevelName}】{itemS.name}执行补给操作");
                break;
            case MessageID.TriggerWaterIntaking:
                sender.LogError("收到了取水的指令");
                var itemwi = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemwi as IWatersOperation)?.WaterIntaking_New();
                var playerDatawi = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemwi.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatawi.ColorCode}>{playerDatawi.PlayerName}</color> {itemwi.name}执行取水操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatawi.ClientLevelName}】{itemwi.name}执行取水操作");
                break;
            case MessageID.TriggerWaterPour:
                sender.LogError("收到了投水的指令");
                MsgReceive_WaterPour(data, out string id, out Vector3 pos);
                var itemp = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, id));
                (itemp as IWatersOperation)?.WaterPour(pos);
                var playerDatap = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemp.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatap.ColorCode}>{playerDatap.PlayerName}</color> {itemp.name}执行投水操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatap.ClientLevelName}】{itemp.name}执行投水操作");
                break;
            case MessageID.TriggerLadeGoods:
                sender.LogError("收到了装载资源的指令");
                var iteml = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (iteml as IGoodsOperation)?.LadeGoods();
                var playerDatal = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, iteml.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatal.ColorCode}>{playerDatal.PlayerName}</color> {iteml.name}执行装载资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatal.ClientLevelName}】{iteml.name}执行装载资源的操作");
                break;
            case MessageID.TriggerUnLadeGoods:
                sender.LogError("收到了卸载资源的指令");
                var itemu = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemu as IGoodsOperation)?.UnLadeGoods();
                var playerDatau = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemu.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatau.ColorCode}>{playerDatau.PlayerName}</color> {itemu.name}执行卸载资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatau.ClientLevelName}】{itemu.name}执行卸载资源的操作");
                break;
            case MessageID.TriggerAirDropGoods:
                sender.LogError("收到了空投资源的指令");
                MsgReceive_WaterPour(data, out string adid, out Vector3 adpos);
                var itemad = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, adid));
                (itemad as IGoodsOperation)?.AirdropGoods(adpos);
                var playerDataad = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemad.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataad.ColorCode}>{playerDataad.PlayerName}</color> {itemad.name}执行空投资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataad.ClientLevelName}】{itemad.name}执行空投资源的操作");
                break;
            case MessageID.TriggerManned:
                sender.LogError("收到了装载人员的指令");
                var itemm = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemm as IRescuePersonnelOperation)?.Manned();
                var playerDatam = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemm.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatam.ColorCode}>{playerDatam.PlayerName}</color> {itemm.name}执行装载人员的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatam.ClientLevelName}】{itemm.name}执行装载人员的操作");
                break;
            case MessageID.TriggerPlacementOfPersonnel:
                sender.LogError("收到了安置人员的指令");
                var itempp = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itempp as IRescuePersonnelOperation)?.PlacementOfPersonnel();
                var playerDatapp = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itempp.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatapp.ColorCode}>{playerDatapp.PlayerName}</color> {itempp.name}执行安置人员的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatapp.ClientLevelName}】{itempp.name}执行安置人员的操作");
                break;
            case MessageID.TriggerCableDescentRescue:
                sender.LogError("收到了索降救援的指令");
                var itemcd = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemcd as IRescuePersonnelOperation)?.CableDescentRescue();
                var playerDatacd = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemcd.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatacd.ColorCode}>{playerDatacd.PlayerName}</color> {itemcd.name}执行索降救援操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatacd.ClientLevelName}】{itemcd.name}执行索降救援操作");
                break;
            case MessageID.TriggerReturnFlight:
                sender.LogError("收到了返航的指令");
                var itemrf = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemrf.OnNullCommand(0);
                var playerDatarf = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemrf.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatarf.ColorCode}>{playerDatarf.PlayerName}</color> {itemrf.name}执行返航的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatarf.ClientLevelName}】{itemrf.name}执行返航的操作");
                break;
            case MessageID.TriggerEndTask:
                sender.LogError("收到了结束任务的指令");
                var itemet = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemet.OnNullCommand(1);
                var playerDataet = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemet.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataet.ColorCode}>{playerDataet.PlayerName}</color> {itemet.name}执行结束任务的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataet.ClientLevelName}】{itemet.name}执行结束任务的操作");
                break;
            case MessageID.TriggerEquipCrash:
                sender.LogError("收到了坠毁的指令");
                var itemec = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemec.OnCrash();
                var playerDataec = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemec.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataec.ColorCode}>{playerDataec.PlayerName}</color> {itemec.name}油量已耗尽，坠毁");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataec.ClientLevelName}】{itemec.name}油量已耗尽，坠毁");
                break;
        }
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

    private void MsgReceive_WaterPour(string data, out string id, out Vector3 pos)
    {
        string[] info = data.Split('_');
        pos = new Vector3(float.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]));
        id = info[3];
    }

    #endregion

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }
}