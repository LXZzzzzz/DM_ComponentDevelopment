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
        MyDataInfo.gameState = GameState.FirstLevelCommanderEditor;
        EventManager.Instance.AddEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.AddEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.AddEventListener<string, Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.AddEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.AddEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.AddEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.AddEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.AddEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.AddEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.AddEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
        EventManager.Instance.AddEventListener<string, Vector3>(EventType.CreatZaiQuZy.ToString(), OnSendCreatZaiQuZy);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryZaiQuzy.ToString(), OnSendDeleZaiQuzy);
    }

    public void Terminate()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseEquip.ToString(), OnChangeCurrentEquip);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChooseZiyuan.ToString(), OnChangeCurrentZiyuan);
        EventManager.Instance.RemoveEventListener<string, Vector3>(EventType.MoveToTarget.ToString(), OnChangeTarget);
        EventManager.Instance.RemoveEventListener<string, string>(EventType.CreatEquipEntity.ToString(), OnCreatEquipEntity);
        EventManager.Instance.RemoveEventListener<ProgrammeData>(EventType.LoadProgrammeDataSuc.ToString(), OnLoadProgrammeDataSuc);
        EventManager.Instance.RemoveEventListener<int, string>(EventType.SendSkillInfoForControler.ToString(), OnSendSkillInfo);
        EventManager.Instance.RemoveEventListener<bool>(EventType.CameraSwitch.ToString(), OnCameraSwith);
        EventManager.Instance.RemoveEventListener<int, Transform>(EventType.CameraControl.ToString(), OnCameraContral);
        EventManager.Instance.RemoveEventListener(EventType.ClearProgramme.ToString(), OnClearScene);
        EventManager.Instance.RemoveEventListener(EventType.GeneratePDF.ToString(), OnGeneratePdf);
        EventManager.Instance.RemoveEventListener<string, Vector3>(EventType.CreatZaiQuZy.ToString(), OnSendCreatZaiQuZy);
        EventManager.Instance.RemoveEventListener<string>(EventType.DestoryZaiQuzy.ToString(), OnSendDeleZaiQuzy);
    }

    private void InitZiyuan()
    {
        if (sceneAllzy == null)
        {
            sceneAllzy = new List<ZiYuanBase>();
            for (int i = 0; i < allBObjects.Length; i++)
            {
                var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
                if (tagItem == null || tagItem.SubTags.Find(y => y.Id == 6 || y.Id == 3 || y.Id == 4) != null)
                {
                    if (tagItem != null)
                    {
                        allBObjects[i].transform.GetChild(0).gameObject.SetActive(false);
                    }

                    continue;
                }

                if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
                {
                    var zyItem = allBObjects[i].GetComponent<ZiYuanBase>();
                    zyItem.latAndLon = CalculateLatLon(zyItem.transform.position);
                    EventManager.Instance.EventTrigger(EventType.CreatAZiyuanIcon.ToString(), zyItem);
                    sceneAllzy.Add(zyItem);
                }
            }

            StartCoroutine(showTask());
        }

        //这里在测试完创建删除后要放开，改为导教端控制游戏暂停，修改灾区数据
        // EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 0);

        clouds = GameObject.Find("Expanse Sky/Cumulus Clouds");
    }

    IEnumerator showTask()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            //任务列表展示
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 3) != null)
            {
                var itemObj = allBObjects[i];
                var itemzy = itemObj.gameObject.GetComponent<ZiYuanBase>();
                EventManager.Instance.EventTrigger(EventType.CreatATaskIcon.ToString(), itemzy);
                yield return 1;
            }
        }
    }

    private void Update()
    {
        if (!isMe) return;
        if (clouds)
        {
            clouds.SetActive(Camera.main != null && Camera.main.gameObject.transform.position.y < 1000);
        }
    }

    public void SendTaskSureMsg()
    {
        EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), misDescription, () =>
        {
            //接收灾情任务，此时计时器开始

            OnSendSkillInfo((int)MessageID.SendReceiveTask, "");
            // for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
            // {
            //     sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)MessageID.SendReceiveTask, "");
            // }
        });
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
            if (MyDataInfo.MyLevel != -1)
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
        if (currentChooseEquip.isDockingAtTheAirport)
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "当前装备在机场未出库");
            return;
        }

        if (!currentChooseEquip.OnCheckIsMove()) return;

        OnSendSkillInfo((int)MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos, targetId));
        // sender.RunSend(SendType.SubToMain, main.BObjectId, (int)Enums.MessageID.MoveToTarget, MsgSend_Move(currentChooseEquip.BObjectId, pos, targetId));
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
        IAirPort myAirPort = null;
        if (!string.IsNullOrEmpty(ProgrammeDataManager.Instance.GetEquipDataById(myId).airportId))
        {
            //找到机场，停机
            string airPortId = ProgrammeDataManager.Instance.GetEquipDataById(myId).airportId;
            for (int j = 0; j < sceneAllzy.Count; j++)
            {
                if (string.Equals(airPortId, sceneAllzy[j].BobjectId))
                {
                    myAirPort = sceneAllzy[j] as IAirPort;
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

    private void OnSendCreatZaiQuZy(string zyId, Vector3 pos)
    {
        if (MyDataInfo.MyLevel > 0) return;

        //先打开属性面板设置属性值，点击确定，把数据和模板id发给所有玩家，在消息接收地方，再执行创建逻辑
        CreatZaiquData data = new CreatZaiquData()
        {
            tempId = zyId, pos = new JsonVector3() { x = pos.x, y = pos.y, z = pos.z }, zaiquId = zyId + (zaiquIdNum += 1), isDele = 0
        };
        var creatDataStr = MsgSend_CreatZaiqu(data);
        sender.LogError("发送创建灾区的消息" + creatDataStr);
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

    private void OnChangeZaiqu(CreatZaiquData data)
    {
        if (data.isDele == 1)
        {
            for (int i = 0; i < sceneAllzy.Count; i++)
            {
                if (string.Equals(sceneAllzy[i].BobjectId, data.zaiquId))
                {
                    EventManager.Instance.EventTrigger(EventType.DestoryZiyuanIcon.ToString(), data.zaiquId);
                    Destroy(sceneAllzy[i].gameObject);
                    sceneAllzy.RemoveAt(i);
                    break;
                }
            }

            return;
        }

        bool isfind = false;
        ZiYuanBase templateZaiqu = null;
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, data.tempId))
            {
                isfind = true;
                templateZaiqu = allBObjects[i].transform.GetChild(0).GetComponent<ZiYuanBase>();
                break;
            }
        }

        if (!isfind || templateZaiqu == null)
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
        switch (temporaryZaiqu.ZiYuanType)
        {
            case ZiYuanType.SourceOfAFire:
                (temporaryZaiqu as ISourceOfAFire).fireInit(5, 5, 10000, data.zaiquId, "#800049", "#cb488f");
                break;
        }
        // temporaryZaiqu.Reset();

        EventManager.Instance.EventTrigger(EventType.CreatAZiyuanIcon.ToString(), temporaryZaiqu);
        sceneAllzy.Add(temporaryZaiqu);
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