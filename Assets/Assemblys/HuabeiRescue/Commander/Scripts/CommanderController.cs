using System;
using System.Collections;
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
    private Func<Vector3, Vector2> CalculateLatLon;
    private GameObject clouds;
    private int zaiquIdNum;
    private GameObject cameraFllowGo;
    private List<ZiYuanBase> sceneAlltempzy;

    private bool isMe;

    private void Start()
    {
        clientOperatorInfos = new List<string>();
    }

    public void Init(Func<Vector3, Vector2> callback)
    {
        sender.LogError("指挥端组件ID：" + main.BObjectId);
        isMe = true;
        zaiquIdNum = 1;
        CalculateLatLon = callback;
        InitZiyuan();
        _pdfReport = new PDFReport();
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.AddEventListener<string>(EventType.DqChooseGo.ToString(), OnChooseAGo);
        EventManager.Instance.AddEventListener<string, Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        // EventManager.Instance.AddEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.AddEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.AddEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.AddEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.AddEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.AddEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.AddEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
        EventManager.Instance.AddEventListener<string, Vector3>(EventType.CreatZaiQuZy.ToString(), OnCreatZaiQuZy);
        EventManager.Instance.AddEventListener<ZyVariableDataBase>(EventType.CreatZaiQuZyRun.ToString(), OnSendCreatZaiQuZy);
        EventManager.Instance.AddEventListener<Vector2>(EventType.MarkMapPoints.ToString(), OnSendMarkMapPoint);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryZaiQuzy.ToString(), OnSendDeleZaiQuzy);
        EventManager.Instance.AddEventListener(EventType.ShowMisDescription.ToString(), SendTaskSureMsg);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.RemoveEventListener<string>(EventType.DqChooseGo.ToString(), OnChooseAGo);
        EventManager.Instance.RemoveEventListener<string, Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        // EventManager.Instance.RemoveEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.RemoveEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.RemoveEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.RemoveEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.RemoveEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.RemoveEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.RemoveEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
        EventManager.Instance.RemoveEventListener<string, Vector3>(EventType.CreatZaiQuZy.ToString(), OnCreatZaiQuZy);
        EventManager.Instance.RemoveEventListener<ZyVariableDataBase>(EventType.CreatZaiQuZyRun.ToString(), OnSendCreatZaiQuZy);
        EventManager.Instance.RemoveEventListener<Vector2>(EventType.MarkMapPoints.ToString(), OnSendMarkMapPoint);
        EventManager.Instance.RemoveEventListener<string>(EventType.DestoryZaiQuzy.ToString(), OnSendDeleZaiQuzy);
        EventManager.Instance.RemoveEventListener(EventType.ShowMisDescription.ToString(), SendTaskSureMsg);
    }

    private void InitZiyuan()
    {
        if (sceneAllzy == null)
        {
            sceneAllzy = new List<ZiYuanBase>();
            MyDataInfo.sceneAllZiYuan = new List<ZiYuanBase>();
            for (int i = 0; i < allBObjects.Length; i++)
            {
                var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
                if (tagItem == null || tagItem.SubTags.Find(y => y.Id == 6 || y.Id == 4) != null)
                {
                    if (tagItem != null)
                    {
                        allBObjects[i].transform.GetChild(0).gameObject.SetActive(false);
                        if (sceneAlltempzy == null) sceneAlltempzy = new List<ZiYuanBase>();
                        sceneAlltempzy.Add(allBObjects[i].transform.GetChild(0).GetComponent<ZiYuanBase>());
                    }

                    continue;
                }

                if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
                {
                    var zyItem = allBObjects[i].GetComponent<ZiYuanBase>();
                    zyItem.latAndLon = CalculateLatLon(zyItem.transform.position);
                    EventManager.Instance.EventTrigger(EventType.CreatAZiyuanIcon.ToString(), zyItem);
                    sceneAllzy.Add(zyItem);
                    MyDataInfo.sceneAllZiYuan.Add(zyItem);
                    // if(zyItem is ITaskProgress) //证明该资源包含任务数据
                }
            }
        }

        //这里在测试完创建删除后要放开，改为导教端控制游戏暂停，修改灾区数据
        // EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);

        clouds = GameObject.Find("Expanse Sky/Cumulus Clouds");
    }

    private void Update()
    {
        if (!isMe) return;
        if (MyDataInfo.gameState != GameState.None && MyDataInfo.gameState != GameState.GamePause && MyDataInfo.gameState != GameState.GameStop)
        {
            MyDataInfo.gameStartTime += Time.deltaTime * MyDataInfo.speedMultiplier;
        }

        if (clouds)
        {
            clouds.SetActive(Camera.main != null && Camera.main.gameObject.transform.position.y < 2000);
        }

        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 2);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendGameStart, ((int)(MyDataInfo.gameStartTime * 1000)).ToString());
        // }
        //
        // if (Input.GetKeyDown(KeyCode.L))
        // {
        //     OnGetTurnBack();
        // }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.LogError(MyDataInfo.gameState);
        }
    }

    private void SendTaskSureMsg()
    {
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), misDescription);

        // EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), misDescription, () =>
        // {
        //     //接收灾情任务，此时计时器开始
        //
        //     // OnSendSkillInfo((int)MessageID.SendReceiveTask, "");
        //     // for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        //     // {
        //     //     sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)MessageID.SendReceiveTask, "");
        //     // }
        // });
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
        if (!isMove) return;
        if (cameraFllowGo != null)
        {
            OnCameraContral(2, cameraFllowGo.transform);
        }
        else tc.enabled = false;
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
                cameraFllowGo = null;
                break;
            case 2:
                cvm.enabled = false;
                mo.enabled = false;
                tc.enabled = true;
                tc.Target = cameraFllowGo = target.gameObject;
                break;
        }
    }

    private void OnChangeCurrentEquip(string equipId)
    {
        //大庆版本这里不走了
        // if (MyDataInfo.gameState != GameState.GameStart) return;
        if (string.IsNullOrEmpty(equipId))
        {
            if (currentChooseEquip != null)
                currentChooseEquip.isChooseMe = false;
            currentChooseEquip = null;
            if (MyDataInfo.MyLevel != -1)
                EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
            return;
        }

        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(equipId, x.BObjectId));

        if (string.Equals(itemEquip.BeLongToCommanderId, MyDataInfo.leadId))
        {
            if (currentChooseEquip != null) currentChooseEquip.isChooseMe = false;
            currentChooseEquip = itemEquip;
            currentChooseEquip.isChooseMe = true;
            currentChooseEquip.CurrentChooseSkillType = SkillType.None;
            if (MyDataInfo.MyLevel != -1)
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
            if (MyDataInfo.MyLevel != -1)
                EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
            return;
        }

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (string.Equals(ziyuanId, sceneAllzy[i].BobjectId))
            {
                var itemZy = sceneAllzy[i];
                if (itemZy == null) return;
                if (currentChooseZiYuan != null) currentChooseZiYuan.isChooseMe = false;
                currentChooseZiYuan = itemZy;
                currentChooseZiYuan.isChooseMe = true;
                OnCameraContral(1, currentChooseZiYuan.transform);
                if (MyDataInfo.MyLevel != -1)
                    EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", currentChooseZiYuan);
                break;
            }
        }
    }

    private void OnChangeTarget(string targetId, Vector3 pos)
    {
        if (MyDataInfo.gameState != GameState.GameStart || currentChooseEquip == null) return;

        if (!currentChooseEquip.OnCheckIsMove()) return;

        OnSendSkillInfo((int)MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos, targetId));
        // sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos, targetId));
    }

    private void OnLoadProgrammeDataSuc(ProgrammeData data)
    {
        for (int i = 0; i < data.AllEquipDatas.Count; i++)
        {
            EventManager.Instance.EventTrigger(EventType.InitEquipData.ToString(), data.AllEquipDatas[i]);
        }

        for (int i = 0; i < data.AllZiYuanDatas.Count; i++)
        {
            //资源数据的修改直接修改实体，UI上不做体现
            //这里现在需要传的数据就只有一个数值，暂时这样写，后面如果有数据扩展，就在ProgrammeData在中直接存ZyVariableDataBase，这里解析直接传
            ZyVariableDataBase itemData = null;
            var item = sceneAllzy.Find(x => string.Equals(x.BobjectId, data.AllZiYuanDatas[i].myId));
            switch (item.ZiYuanType)
            {
                case ZiYuanType.Supply:
                    itemData = new SupplyVariableData() { oilNum = data.AllZiYuanDatas[i].zyNum };
                    break;
                case ZiYuanType.GoodsPoint:
                    itemData = new GoodsPointVariableData() { goodsNum = data.AllZiYuanDatas[i].zyNum };
                    break;
            }

            item.SetVariableData(itemData);


            // EventManager.Instance.EventTrigger(EventType.InitZiYuanBeUsed.ToString(), itemZy);
        }

        // 这里地图状态应该都是默认，这个阶段没有创建需求
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 3);
        // EventManager.Instance.EventTrigger(EventType.ShowProgrammeName.ToString(), data.programmeName);
    }

    //清空场景中的所有方案数据
    private void OnClearScene()
    {
        //清空所有装备
        MyDataInfo.sceneAllEquips.ForEach(x => x.Destroy());
        MyDataInfo.sceneAllEquips.Clear();
        //对所有资源的归属情况都清零
        for (int i = 0; i < sceneAllzy?.Count; i++)
        {
            ZiYuanBase itemZy = sceneAllzy[i];
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
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前有直升机未入库机场，无法生成报告！");
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

        if (playerEquips == null)
        {
            playerEquips = new Dictionary<string, List<string>>();
            for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
            {
                var itemAllEquip = MyDataInfo.sceneAllEquips.FindAll(x => string.Equals(MyDataInfo.playerInfos[i].RoleId, x.BeLongToCommanderId));
                if (!playerEquips.ContainsKey(MyDataInfo.playerInfos[i].ClientLevelName))
                    playerEquips.Add(MyDataInfo.playerInfos[i].ClientLevelName, new List<string>());
                for (int j = 0; j < itemAllEquip.Count; j++)
                {
                    playerEquips[MyDataInfo.playerInfos[i].ClientLevelName].Add(itemAllEquip[j].name);
                }
            }
        }

        if (playerZiyuans == null)
        {
            playerZiyuans = new Dictionary<string, List<string>>();
            for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
            {
                var itemAllZy = sceneAllzy.FindAll(x => x.beUsedCommanderIds?.Find(y =>
                    string.Equals(y, MyDataInfo.playerInfos[i].RoleId)) != null);
                if (!playerZiyuans.ContainsKey(MyDataInfo.playerInfos[i].ClientLevelName))
                    playerZiyuans.Add(MyDataInfo.playerInfos[i].ClientLevelName, new List<string>());
                for (int j = 0; j < itemAllZy.Count; j++)
                {
                    playerZiyuans[MyDataInfo.playerInfos[i].ClientLevelName].Add(itemAllZy[j].ziYuanName);
                }
            }
        }

        // clientOperatorInfos.sore
        if (gameType == 1)
            GenerateFireExtinguishingReport();
        if (gameType == 2)
            GenerateRescueReport();

        // EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "成功生成评估报告PDF");
    }

    private void OnCreatZaiQuZy(string zyId, Vector3 pos)
    {
        if (MyDataInfo.MyLevel > 0) return;

        var tempZy = sceneAlltempzy.Find(x => string.Equals(x.BobjectId, zyId));

        this.zyId = zyId;
        this.pos = pos;
        EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "ChangeZyData", tempZy.ZiYuanType);
    }

    private string zyId;
    private Vector3 pos;

    private void OnSendCreatZaiQuZy(ZyVariableDataBase vData)
    {
        EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString());
        EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 3);
        CreatZaiquData data = new CreatZaiquData()
        {
            tempId = zyId, pos = new JsonVector3() { x = pos.x, y = pos.y, z = pos.z }, zaiquId = zyId + (zaiquIdNum += 1), isDele = 0, vData = vData
        };
        var creatDataStr = MsgSend_CreatZaiqu(data);
        OnSendSkillInfo((int)MessageID.SendChangeZaiqu, creatDataStr);
    }

    private void OnSendDeleZaiQuzy(string zyid)
    {
        if (MyDataInfo.MyLevel > 0) return;

        CreatZaiquData data = new CreatZaiquData()
        {
            zaiquId = zyid, isDele = 1
        };
        var creatDataStr = MsgSend_CreatZaiqu(data);
        OnSendSkillInfo((int)MessageID.SendChangeZaiqu, creatDataStr);
    }

    private void OnSendMarkMapPoint(Vector2 mapPoint)
    {
        string itemData = mapPoint.x + "_" + mapPoint.y;
        OnSendSkillInfo((int)MessageID.SendMarkMapPoint, itemData);
    }

    private void OnChangeZaiqu(CreatZaiquData data)
    {
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 1);
        if (data.isDele == 1)
        {
            for (int i = 0; i < sceneAllzy.Count; i++)
            {
                if (string.Equals(sceneAllzy[i].BobjectId, data.zaiquId))
                {
                    EventManager.Instance.EventTrigger(EventType.DestoryZiyuanIcon.ToString(), data.zaiquId);
                    Destroy(sceneAllzy[i].gameObject);
                    sceneAllzy.RemoveAt(i);
                    MyDataInfo.sceneAllZiYuan.RemoveAt(i);
                    break;
                }
            }

            return;
        }

        // bool isfind = false;
        // ZiYuanBase templateZaiqu = null;
        // for (int i = 0; i < allBObjects.Length; i++)
        // {
        //     if (string.Equals(allBObjects[i].BObject.Id, data.tempId))
        //     {
        //         isfind = true;
        //         templateZaiqu = allBObjects[i].transform.GetChild(0).GetComponent<ZiYuanBase>();
        //         break;
        //     }
        // }

        ZiYuanBase templateZaiqu = sceneAlltempzy.Find(x => string.Equals(x.BobjectId, data.tempId));
        if (templateZaiqu == null)
        {
            Debug.LogError("创建的目标对象未找到");
            return;
        }

        var temporaryZaiqu = Instantiate(templateZaiqu, MyDataInfo.SceneGoParent);
        temporaryZaiqu.transform.position = new Vector3(data.pos.x, data.pos.y, data.pos.z);
        float posY = GetCurrentGroundHeight(temporaryZaiqu.transform);
        var zaiQuPosition = temporaryZaiqu.transform.position;
        zaiQuPosition = new Vector3(zaiQuPosition.x, posY, zaiQuPosition.z);
        temporaryZaiqu.transform.position = zaiQuPosition;
        temporaryZaiqu.latAndLon = CalculateLatLon(zaiQuPosition);
        temporaryZaiqu.gameObject.SetActive(true);

        //这里可以使用多态，延迟初始化，这里暂时先这样写，功能完成后，进行优化
        if (temporaryZaiqu is ISourceOfAFire)
        {
            var firData = data.vData as FireVariableData;
            Debug.LogError(firData.fs);
            (temporaryZaiqu as ISourceOfAFire).fireInit(firData.fs, firData.pd, firData.csrsmj, data.zaiquId, "#800049", "#cb488f");
            temporaryZaiqu.ziYuanName = firData.ZyName;
            temporaryZaiqu.ZiyuanIcon = templateZaiqu.ZiyuanIcon;
        }

        if (temporaryZaiqu is IDisasterArea)
        {
            var disData = data.vData as DisasterVariableData;
            (temporaryZaiqu as IDisasterArea).disasterInit(data.zaiquId, disData.personNum, disData.type, "#800049", "#cb488f");
            temporaryZaiqu.ziYuanName = disData.ZyName;
        }
        // temporaryZaiqu.Reset();

        EventManager.Instance.EventTrigger(EventType.CreatAZiyuanIcon.ToString(), temporaryZaiqu);
        sceneAllzy.Add(temporaryZaiqu);
        MyDataInfo.sceneAllZiYuan.Add(temporaryZaiqu);


        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 3);
        
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(),"有新发现灾情，请处理");
    }

    private void OnChangeJizhangView(List<string> bindingZys)
    {
        //如果是机长的话，要让机长页面上不属于自己的资源都不显示
        if (MyDataInfo.MyLevel == 3)
        {
            EventManager.Instance.EventTrigger(EventType.ChangeJiZhangView.ToString(), bindingZys);
        }
    }

    private float GetCurrentGroundHeight(Transform go)
    {
        // 射线的起点是当前物体的位置
        Ray ray = new Ray(go.position + Vector3.up * 10000, Vector3.down);

        // 存储射线碰撞信息的变量
        RaycastHit hit;

        // 检测射线是否碰撞到任何物体
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // 打印碰撞点的坐标
            Debug.Log("Hit Point: " + hit.point);
            return hit.point.y;
        }
        else
        {
            // 如果没有碰撞到任何物体
            Debug.Log("No hit");
            return -1;
        }
    }
}