using System;
using System.Collections.Generic;
using Enums;
using Newtonsoft.Json;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine;
using EventType = Enums.EventType;

public partial class CommanderController
{
    #region 处理接收的消息

    public void Receive_GameStart()
    {
        if (sceneAllzy != null)
            sceneAllzy.ForEach(a => a.OnStart());
    }

    public void Receive_MoveEquipToTarget(string data)
    {
        MsgReceive_Move(data, out string equipId, out Vector3 targetPos, out string targetId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).MoveToTarget(targetPos);
        var item = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId));
        var playerData = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, item.BeLongToCommanderId));
        var targetZy = string.IsNullOrEmpty(targetId) ? null : sceneAllzy.Find(a => string.Equals(a.BobjectId, targetId));
        try
        {
            EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(),
                $"<color={playerData.ColorCode}>{playerData.ClientLevelName}</color> {item.name}执行机动操作，  目标点为{(targetZy != null ? targetZy.ziYuanName : CalculateLatLon(targetPos).ToString())}");
            clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerData.ClientLevelName}】{item.name}执行机动操作，  目标点为{(targetZy != null ? targetZy.ziYuanName : CalculateLatLon(targetPos).ToString())}");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public void Receive_ProgrammeData(string data)
    {
        var programmeData = ProgrammeDataManager.Instance.UnPackingData(data);
        OnLoadProgrammeDataSuc(programmeData);
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "收到上级派发任务，请接收");
    }


    public void Receive_GameStop()
    {
        var myInfo = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, MyDataInfo.leadId));
        if (!MyDataInfo.isPlayBack && myInfo.ClientLevel == 1 && MyDataInfo.sceneAllEquips.Find(x => !x.isCrash && !x.isDockingAtTheAirport) == null)
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

    public void Receive_ChangeController(string info)
    {
        if (MyDataInfo.MyLevel == 1) return;
        string deStr = AESUtils.Decrypt(info);
        var data = JsonConvert.DeserializeObject<ChangeController>(deStr);
        if (data.objType == 1)
        {
            var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, data.ChangeTargetId));
            if (itemEquip != null) itemEquip.BeLongToCommanderId = data.currentComs[0];
            var itemEquipData = ProgrammeDataManager.Instance.GetCurrentData.AllEquipDatas.Find(x => string.Equals(x.myId, data.ChangeTargetId));
            if (itemEquipData != null) itemEquipData.controllerId = data.currentComs[0];
        }
        else
        {
            //这里是资源的权限更改
            var itemZy = sceneAllzy.Find(x => string.Equals(x.BobjectId, data.ChangeTargetId));
            if (itemZy != null) itemZy.SetBeUsedComs(data.currentComs);

            if (ProgrammeDataManager.Instance.GetCurrentData.ZiYuanControlledList.ContainsKey(data.ChangeTargetId))
            {
                var itemZyData = ProgrammeDataManager.Instance.GetCurrentData.ZiYuanControlledList[data.ChangeTargetId];
                if (itemZyData != null)
                {
                    itemZyData.Clear();
                    itemZyData.AddRange(data.currentComs.ToArray());
                }
            }
            else
            {
                for (int i = 0; i < data.currentComs.Count; i++)
                {
                    ProgrammeDataManager.Instance.ChangeZiYuanData(data.ChangeTargetId, data.currentComs[i], true);
                }
            }
        }

        //通知UI层面更改这个对象的控制者的显示
        EventManager.Instance.EventTrigger(EventType.ChangeObjController.ToString(), data.objType, data.ChangeTargetId);
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
                    var zyItem = allBObjects[i].GetComponent<ZiYuanBase>();
                    zyItem.latAndLon = CalculateLatLon(zyItem.transform.position);
                    sceneAllzy.Add(zyItem);
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
                    EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "数据错误，指定的直升机未在机场");
                    return;
                }

                var item = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (item as IGroundReady)?.GroundReady(sceneAllzy.Find(a => string.Equals(a.BobjectId, itemAirportId)) as IAirPort);
                var playerData = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, item.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerData.ColorCode}>{playerData.ClientLevelName}</color> {item.name}执行起飞前准备操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerData.ClientLevelName}】{item.name}执行起飞前准备操作");
                break;
            case MessageID.TriggerBePutInStorage:
                sender.LogError("收到了入库的指令");
                var itema = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itema as IGroundReady)?.BePutInStorage();
                var playerDataa = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itema.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataa.ColorCode}>{playerDataa.ClientLevelName}</color> {itema.name}执行入库操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataa.ClientLevelName}】{itema.name}执行入库操作");
                break;
            case MessageID.TriggerTakeOff:
                sender.LogError("收到了起飞的指令");
                var itemb = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemb as ITakeOffAndLand)?.TakeOff();
                var playerDatab = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemb.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatab.ColorCode}>{playerDatab.ClientLevelName}</color> {itemb.name}执行起飞操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatab.ClientLevelName}】{itemb.name}执行起飞操作");
                break;
            case MessageID.TriggerLanding:
                sender.LogError("收到了降落的指令");
                var itemc = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemc as ITakeOffAndLand)?.Landing();
                var playerDatac = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemc.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatac.ColorCode}>{playerDatac.ClientLevelName}</color> {itemc.name}执行降落操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatac.ClientLevelName}】{itemc.name}执行降落操作");
                break;
            case MessageID.TriggerSupply:
                sender.LogError("收到了补给的指令");
                var itemS = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemS as ISupply)?.Supply();
                var playerDataS = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemS.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataS.ColorCode}>{playerDataS.ClientLevelName}</color> {itemS.name}执行补给操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataS.ClientLevelName}】{itemS.name}执行补给操作");
                break;
            case MessageID.TriggerWaterIntaking:
                sender.LogError("收到了取水的指令");
                var itemwi = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemwi as IWatersOperation)?.WaterIntaking_New();
                var playerDatawi = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemwi.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatawi.ColorCode}>{playerDatawi.ClientLevelName}</color> {itemwi.name}执行取水操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatawi.ClientLevelName}】{itemwi.name}执行取水操作");
                break;
            case MessageID.TriggerWaterPour:
                sender.LogError("收到了投水的指令");
                MsgReceive_WaterPour(data, out string id, out Vector3 pos);
                var itemp = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, id));
                (itemp as IWatersOperation)?.WaterPour(pos);
                var playerDatap = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemp.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatap.ColorCode}>{playerDatap.ClientLevelName}</color> {itemp.name}执行投水操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatap.ClientLevelName}】{itemp.name}执行投水操作");
                break;
            case MessageID.TriggerLadeGoods:
                sender.LogError("收到了装载资源的指令");
                var iteml = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (iteml as IGoodsOperation)?.LadeGoods();
                var playerDatal = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, iteml.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatal.ColorCode}>{playerDatal.ClientLevelName}</color> {iteml.name}执行装载资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatal.ClientLevelName}】{iteml.name}执行装载资源的操作");
                break;
            case MessageID.TriggerUnLadeGoods:
                sender.LogError("收到了卸载资源的指令");
                var itemu = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemu as IGoodsOperation)?.UnLadeGoods();
                var playerDatau = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemu.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatau.ColorCode}>{playerDatau.ClientLevelName}</color> {itemu.name}执行卸载资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatau.ClientLevelName}】{itemu.name}执行卸载资源的操作");
                break;
            case MessageID.TriggerAirDropGoods:
                sender.LogError("收到了空投资源的指令");
                MsgReceive_WaterPour(data, out string adid, out Vector3 adpos);
                var itemad = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, adid));
                (itemad as IGoodsOperation)?.AirdropGoods(adpos);
                var playerDataad = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemad.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataad.ColorCode}>{playerDataad.ClientLevelName}</color> {itemad.name}执行空投资源的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataad.ClientLevelName}】{itemad.name}执行空投资源的操作");
                break;
            case MessageID.TriggerManned:
                sender.LogError("收到了装载人员的指令");
                var itemm = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemm as IRescuePersonnelOperation)?.Manned();
                var playerDatam = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemm.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatam.ColorCode}>{playerDatam.ClientLevelName}</color> {itemm.name}执行装载人员的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatam.ClientLevelName}】{itemm.name}执行装载人员的操作");
                break;
            case MessageID.TriggerPlacementOfPersonnel:
                sender.LogError("收到了安置人员的指令");
                var itempp = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itempp as IRescuePersonnelOperation)?.PlacementOfPersonnel();
                var playerDatapp = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itempp.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatapp.ColorCode}>{playerDatapp.ClientLevelName}</color> {itempp.name}执行安置人员的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatapp.ClientLevelName}】{itempp.name}执行安置人员的操作");
                break;
            case MessageID.TriggerCableDescentRescue:
                sender.LogError("收到了索降救援的指令");
                var itemcd = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                (itemcd as IRescuePersonnelOperation)?.CableDescentRescue();
                var playerDatacd = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemcd.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatacd.ColorCode}>{playerDatacd.ClientLevelName}</color> {itemcd.name}执行吊运救援操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatacd.ClientLevelName}】{itemcd.name}执行吊运救援操作");
                break;
            case MessageID.TriggerReturnFlight:
                sender.LogError("收到了返航的指令");
                var itemrf = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemrf.OnNullCommand(0);
                var playerDatarf = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemrf.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDatarf.ColorCode}>{playerDatarf.ClientLevelName}</color> {itemrf.name}执行返航的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDatarf.ClientLevelName}】{itemrf.name}执行返航的操作");
                break;
            case MessageID.TriggerEndTask:
                sender.LogError("收到了结束任务的指令");
                var itemet = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemet.OnNullCommand(1);
                var playerDataet = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemet.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataet.ColorCode}>{playerDataet.ClientLevelName}</color> {itemet.name}执行结束任务的操作");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataet.ClientLevelName}】{itemet.name}执行结束任务的操作");
                break;
            case MessageID.TriggerEquipCrash:
                sender.LogError("收到了坠毁的指令");
                var itemec = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, data));
                itemec.OnCrash();
                EventManager.Instance.EventTrigger(EventType.crashIcon.ToString(), data);
                var playerDataec = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemec.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataec.ColorCode}>{playerDataec.ClientLevelName}</color> {itemec.name}油量已耗尽，坠毁");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataec.ClientLevelName}】{itemec.name}油量已耗尽，坠毁");
                break;
            case MessageID.TriggerOnlyShow:
                sender.LogError("收到了文本指令");
                var showInfo = data.Split('_');
                var itemes = MyDataInfo.sceneAllEquips.Find(a => string.Equals(a.BObjectId, showInfo[0]));
                var playerDataes = MyDataInfo.playerInfos.Find(a => string.Equals(a.RoleId, itemes.BeLongToCommanderId));
                EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), $"<color={playerDataes.ColorCode}>{playerDataes.ClientLevelName}</color> {itemes.name}进行{showInfo[1]}");
                clientOperatorInfos.Add(MyDataInfo.gameStartTime + $"--【{playerDataes.ClientLevelName}】{itemes.name}进行{showInfo[1]}");
                break;
            case MessageID.TriggerReport:
                //todo:获取到了指定玩家的报备指令，存起来，生成报告的时候去加分
                if (reportPlayers == null) reportPlayers = new List<string>();
                if (!reportPlayers.Contains(data)) reportPlayers.Add(data);
                break;
        }
    }

    public void Receive_TextMsgRecord(string data)
    {
        clientOperatorInfos.Add(MyDataInfo.gameStartTime + "--" + data);
        EventManager.Instance.EventTrigger(EventType.ShowAMsgInfo.ToString(), data);
    }

    public void Receive_CreatZaiqu(string data)
    {
        var itemdata = MsgReceive_CreatZaiqu(data);
        OnChangeZaiqu(itemdata);
    }

    public void Receive_ShowMarkPoint(string data)
    {
        string[] strPos = data.Split('_');
        Vector2 itempos = new Vector2(float.Parse(strPos[0]), float.Parse(strPos[1]));
        EventManager.Instance.EventTrigger(EventType.ShowMarkMapPoint.ToString(), itempos);
    }

    #endregion

    #region 数据转换（消息的打包和解析）

    private string MsgSend_Move(string id, Vector3 pos, string targetId)
    {
        return string.Format($"{id}_{pos.x}_{pos.y}_{pos.z}_{targetId}");
    }

    private void MsgReceive_Move(string data, out string id, out Vector3 pos, out string targetId)
    {
        string[] info = data.Split('_');
        id = info[0];
        pos = new Vector3(float.Parse(info[1]), float.Parse(info[2]), float.Parse(info[3]));
        targetId = info[4];
    }

    private void MsgReceive_WaterPour(string data, out string id, out Vector3 pos)
    {
        string[] info = data.Split('_');
        pos = new Vector3(float.Parse(info[0]), float.Parse(info[1]), float.Parse(info[2]));
        id = info[3];
    }

    private string MsgSend_CreatZaiqu(CreatZaiquData data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        return AESUtils.Encrypt(jsonData);
    }

    private CreatZaiquData MsgReceive_CreatZaiqu(string dataStr)
    {
        string deStr = AESUtils.Decrypt(dataStr);
        sender.LogError("解析收到的数据" + deStr);
        var currentData = JsonConvert.DeserializeObject<CreatZaiquData>(deStr);
        return currentData;
    }

    #endregion
}

public class CreatZaiquData
{
    public string tempId;
    public string zaiquId;
    public JsonVector3 pos;
    public int isDele;
}