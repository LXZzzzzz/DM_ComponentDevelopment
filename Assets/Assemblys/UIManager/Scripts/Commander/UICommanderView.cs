using System;
using System.Collections.Generic;
using DM.Core.Map;
using DM.IFS;
using Newtonsoft.Json;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EquipBase = ToolsLibrary.EquipPart.EquipBase;
using EventType = Enums.EventType;

public class UICommanderView : BasePanel
{
    private GameObject equipViewGo, ziYuanViewGo;
    private RectTransform equipParent;
    private RectTransform ziYuanParent;
    private RectTransform taskParent; //灾区组件所展示的列表

    private EquipCell ecPrefab;
    private ZiYuanCell zycPrefab;
    private TaskCell taskPrefab;
    private Text startTime, currentTime;
    private Button btn_EquipUnfold, btn_ZiyuanUnfold;

    private int level;
    private Dictionary<string, string> allCommanderIds; //存储所有指挥端Id和 对应的名称
    private List<EquipCell> allEquipCells; //存储所有装备cell
    private List<ZiYuanCell> allZiYuanCells; //存储所有资源cell，为了后面数据修改
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改


    public override void Init()
    {
        base.Init();
        equipViewGo = transform.Find("LeftPart/GoListViews/EquipListView").gameObject;
        ziYuanViewGo = transform.Find("LeftPart/GoListViews/ZiYuanListView").gameObject;
        equipParent = GetControl<ScrollRect>("EquipsView").content;
        ecPrefab = GetComponentInChildren<EquipCell>(true);
        ziYuanParent = GetControl<ScrollRect>("ZiYuanView").content;
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        taskParent = transform.Find("RightPart").GetComponentInChildren<ScrollRect>(true).content;
        taskPrefab = transform.Find("RightPart").GetComponentInChildren<TaskCell>(true);
        startTime = GetControl<Text>("startTimeShow");
        currentTime = GetControl<Text>("currentTimeShow");
        btn_EquipUnfold = GetControl<Button>("btn_EquipUnfold");
        btn_ZiyuanUnfold = GetControl<Button>("btn_ZiyuanUnfold");

        btn_EquipUnfold.onClick.AddListener(() => retractOrUnfold(true, 1));
        btn_ZiyuanUnfold.onClick.AddListener(() => retractOrUnfold(true, 2));
        GetControl<Button>("btn_EquipRecover").onClick.AddListener(() => retractOrUnfold(false, 1));
        GetControl<Button>("btn_ZiyuanRecover").onClick.AddListener(() => retractOrUnfold(false, 2));
        GetControl<Button>("btn_TaskRecover").onClick.AddListener(() => retractOrUnfold(false, 3));


        allCommanderIds = new Dictionary<string, string>();
        allEquipCells = new List<EquipCell>();
        allZiYuanCells = new List<ZiYuanCell>();
        allTaskCells = new List<TaskCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        level = (int)userData;

        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
        // EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.CreatATaskIcon.ToString(), OnAddTask);
        EventManager.Instance.AddEventListener<AEquipData>(EventType.InitEquipData.ToString(), OnInitEquipData);
        // EventManager.Instance.AddEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);//修改权限后，更新页面
        EventManager.Instance.AddEventListener<List<string>>(EventType.ChangeJiZhangView.ToString(), OnChangeZyShow);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(Enums.EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
        // EventManager.Instance.RemoveEventListener<ZiYuanBase>(Enums.EventType.CreatATaskIcon.ToString(), OnAddTask);
        EventManager.Instance.RemoveEventListener<AEquipData>(EventType.InitEquipData.ToString(), OnInitEquipData);
        // EventManager.Instance.RemoveEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        EventManager.Instance.RemoveEventListener<List<string>>(EventType.ChangeJiZhangView.ToString(), OnChangeZyShow);
    }

    private void showView()
    {
        //设置自己的信息
        // myCommanderInfoShow.Init(MyDataInfo.leadId, OnChooseCommander);
        //获取子指挥官,一级指挥端才需要显示，只显示别人
        if (level == 1)
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                //找到了主角,并且不是自己，就要展示,展示所有占席位玩家
                // if (!string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id) && allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) != null)
                // {
                //     //如果这个玩家没有进入房间，就跳过
                //     string myRoleId = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, allBObjects[i].BObject.Id)).RoleId;
                //     if (string.IsNullOrEmpty(myRoleId)) continue;
                //     if (MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, allBObjects[i].BObject.Id)).ClientLevel < 0) continue;
                //     var itemObj = allBObjects[i];
                //     var itemCell = Instantiate(ccPrefab, commanderParent);
                //     itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, OnChooseCommander);
                //     itemCell.gameObject.SetActive(true);
                //     allCommanderIds.Add(itemObj.BObject.Id, itemObj.BObject.Info.Name);
                //     allCommanderCells.Add(itemObj.BObject.Id, itemCell);
                // }

                if (string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id))
                    allCommanderIds.Add(allBObjects[i].BObject.Id, allBObjects[i].BObject.Info.Name);
            }
        }
    }

    private void Update()
    {
        //这个是在游戏开始第一帧才更新“开始时间”
        if (MyDataInfo.gameStartTime > 0 && MyDataInfo.gameStartTime < 1)
            startTime.text = "开始时间 " + DateTime.Now.ToString("HH:mm:ss");
        currentTime.text = "当前时间 " + DateTime.Now.ToString("HH:mm:ss");
    }

    private void retractOrUnfold(bool isRetract, int type)
    {
        switch (type)
        {
            case 0:
                //隐藏和显示玩家列表
                // btn_ComUnfold.gameObject.SetActive(!isRetract);
                // if (!isRetract)
                // {
                //     commanderViewGo.anchoredPosition = new Vector2(commanderViewGo.anchoredPosition.x - 104, commanderViewGo.anchoredPosition.y);
                //     goListViewGo.anchoredPosition = new Vector2(goListViewGo.anchoredPosition.x - 90, commanderViewGo.anchoredPosition.y);
                // }
                // else
                // {
                //     commanderViewGo.anchoredPosition = new Vector2(commanderViewGo.anchoredPosition.x + 104, commanderViewGo.anchoredPosition.y);
                //     goListViewGo.anchoredPosition = new Vector2(goListViewGo.anchoredPosition.x + 90, commanderViewGo.anchoredPosition.y);
                // }

                break;
            case 1:
                btn_EquipUnfold.gameObject.SetActive(!isRetract);
                equipViewGo.SetActive(isRetract);
                if (!isRetract)
                {
                    GetControl<Toggle>("tog_CtrlEquipTypeView").isOn = false;
                    EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString());
                }

                break;
            case 2:
                btn_ZiyuanUnfold.gameObject.SetActive(!isRetract);
                ziYuanViewGo.SetActive(isRetract);
                break;
        }
    }

    private string currentSelectComId = "";
    private bool isRefreshMsgView = true;

    private void OnAddEquipView(EquipBase equip)
    {
        var itemObj = equip;
        var itemCell = Instantiate(ecPrefab, equipParent);
        itemCell.Init(level, itemObj, allCommanderIds, OnChangeEquipData);
        itemCell.gameObject.SetActive(true);
        allEquipCells.Add(itemCell);
        if (MyDataInfo.MyLevel == 3 && !string.Equals(equip.BeLongToCommanderId, MyDataInfo.leadId))
            itemCell.gameObject.SetActive(false);
    }


    private void OnRemoveEquip(string id)
    {
        for (int i = 0; i < allEquipCells.Count; i++)
        {
            if (string.Equals(allEquipCells[i].equipObjectId, id))
            {
                Destroy(allEquipCells[i].gameObject);
                allEquipCells.RemoveAt(i);
                break;
            }
        }
    }

    int taskIndex = 0;

    private void OnAddZyZq(ZiYuanBase zyObj)
    {
        bool isDisaster = zyObj.ZiYuanType == ZiYuanType.RescueStation || zyObj.ZiYuanType == ZiYuanType.DisasterArea || zyObj.ZiYuanType == ZiYuanType.SourceOfAFire;

        if (isDisaster)
        {
            taskIndex++;
            TaskCell itemCell = Instantiate(taskPrefab, taskParent);
            itemCell.Init("任务" + taskIndex, zyObj);
            itemCell.gameObject.SetActive(true);
            allTaskCells.Add(itemCell);
        }
        else
        {
            ZiYuanCell itemCell = Instantiate(zycPrefab, ziYuanParent);
            itemCell.Init(level, zyObj, OnChangeZiYuanData);
            itemCell.gameObject.SetActive(true);
            allZiYuanCells.Add(itemCell);
        }
    }

    private void OnRemoveZyZq(string deleId)
    {
        for (int i = 0; i < allZiYuanCells.Count; i++)
        {
            if (string.Equals(allZiYuanCells[i].myEntityId, deleId))
            {
                //这里应该得检测资源下有没有任务，如果有要删除
                Destroy(allZiYuanCells[i].gameObject);
                allZiYuanCells.RemoveAt(i);
                break;
            }
        }
    }

    private void OnInitEquipData(AEquipData aeData)
    {
        if (MyDataInfo.MyLevel == 3)
        {
            Debug.LogError(allEquipCells == null);
            Debug.LogError(allEquipCells?.Count);
            Debug.LogError(aeData);
        }

        allEquipCells.Find(x => string.Equals(x.equipObjectId, aeData.myId)).RefreshComShow(aeData);
    }

    private void OnChangeZyShow(List<string> showZys)
    {
        allZiYuanCells.ForEach(x =>
            x.gameObject.SetActive(showZys.Find(y => string.Equals(x.myEntityId, y)) != null)
        );
        allTaskCells.ForEach(x =>
            x.gameObject.SetActive(showZys.Find(y => string.Equals(x.myEntityId, y)) != null)
        );
    }

    private void OnChangeEquipData(AEquipData edata)
    {
        //这里考虑删掉这个回调，在cell中直接调用
        ProgrammeDataManager.Instance.ChangeEquipData(edata);

        // //修改数据中的信息
        // ProgrammeDataManager.Instance.GetEquipDataById(equipId).controllerId = commanderId;
        //
        // if (MyDataInfo.gameState >= GameState.Preparation)
        // {
        //     ChangeController ccData = new ChangeController()
        //     {
        //         objType = 1, ChangeTargetId = equipId, currentComs = new List<string> { commanderId }
        //     };
        //     string jsonData = JsonConvert.SerializeObject(ccData);
        //     string sendData = AESUtils.Encrypt(jsonData);
        //     //游戏进行中修改的话，发送给所有人
        //     EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendChangeController, sendData);
        // }
    }

    public void OnChangeZiYuanData(AZiYuanData zdata)
    {
        //这个方法要改成修改资源数据
        ProgrammeDataManager.Instance.ChangeZiyuanData(zdata);


        // bool isChangeSuc = ProgrammeDataManager.Instance.ChangeZiYuanData(ziYuanId, commanderId, addOrRemove);

        // if (isChangeSuc)
        // {
        //     for (int i = 0; i < allBObjects.Length; i++)
        //     {
        //         if (string.Equals(ziYuanId, allBObjects[i].BObject.Id))
        //         {
        //             var zyObj = allBObjects[i].GetComponent<ZiYuanBase>();
        //             if (addOrRemove) zyObj.AddBeUsdCom(commanderId);
        //             else zyObj.RemoveBeUsedCom(commanderId);
        //
        //             #region 发送给所有人
        //
        //             if (MyDataInfo.gameState >= GameState.Preparation)
        //             {
        //                 ChangeController ccData = new ChangeController()
        //                 {
        //                     objType = 2, ChangeTargetId = zyObj.BobjectId, currentComs = zyObj.beUsedCommanderIds
        //                 };
        //                 string jsonData = JsonConvert.SerializeObject(ccData);
        //                 string sendData = AESUtils.Encrypt(jsonData);
        //                 //游戏进行中修改的话，发送给所有人
        //
        //                 EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendChangeController, sendData);
        //             }
        //
        //             #endregion
        //
        //             break;
        //         }
        //     }
        // }
        //
        // return isChangeSuc;
    }
}