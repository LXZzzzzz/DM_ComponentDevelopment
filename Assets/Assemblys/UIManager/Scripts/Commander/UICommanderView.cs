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
    private RectTransform commanderViewGo;
    private RectTransform goListViewGo;
    private GameObject equipViewGo, ziYuanViewGo, taskViewGo;
    private RectTransform equipTypeParent;
    private RectTransform equipParent;
    private RectTransform commanderParent;
    private RectTransform ziYuanParent;
    private RectTransform disasterParent; //灾区组件所展示的列表
    // private RectTransform taskParent;
    private EquipTypeCell etcPrefab;
    private EquipCell ecPrefab;
    private CommanderCell ccPrefab;
    private ZiYuanCell zycPrefab;
    // private TaskCell taskPrefab;
    private Text startTime, currentTime;
    private Button btn_ComUnfold, btn_EquipUnfold, btn_ZiyuanUnfold, btn_TaskUnfold;
    private RectTransform CmGo;

    private MyCommanderView myCommanderInfoShow;

    private int level;
    private Dictionary<string, string> allCommanderIds; //存储所有指挥端Id和 对应的名称
    private List<EquipCell> allEquipCells; //存储所有装备cell
    private List<ZiYuanCell> allZiYuanCells; //存储所有资源cell，为了后面数据修改
    private List<ZiYuanCell> allZyZqCells; //存储所有灾区cell
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改
    private Dictionary<string, CommanderCell> allCommanderCells; //存储所有玩家cell
    private bool isMoveCmGo;


    public override void Init()
    {
        base.Init();
        commanderViewGo = transform.Find("LeftPart/CommandersView").GetComponent<RectTransform>();
        goListViewGo = transform.Find("LeftPart/GoListViews").GetComponent<RectTransform>();
        equipViewGo = transform.Find("LeftPart/GoListViews/EquipListView").gameObject;
        ziYuanViewGo = transform.Find("LeftPart/GoListViews/ZiYuanListView").gameObject;
        taskViewGo = transform.Find("LeftPart/GoListViews/tasksListView").gameObject;
        CmGo = transform.Find("CopyMoverPart").GetComponent<RectTransform>();
        equipTypeParent = GetControl<ScrollRect>("EquipsTypes").content;
        etcPrefab = GetComponentInChildren<EquipTypeCell>(true);
        equipParent = GetControl<ScrollRect>("EquipsView").content;
        ecPrefab = GetComponentInChildren<EquipCell>(true);
        commanderParent = GetControl<ScrollRect>("CommandersView").content;
        ccPrefab = GetComponentInChildren<CommanderCell>(true);
        ziYuanParent = GetControl<ScrollRect>("ZiYuanView").content;
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        disasterParent = GetControl<ScrollRect>("TaskListView").content;
        // taskPrefab = GetComponentInChildren<TaskCell>(true);
        // taskParent = GetControl<ScrollRect>("TaskListView").content;
        startTime = GetControl<Text>("startTimeShow");
        currentTime = GetControl<Text>("currentTimeShow");
        btn_ComUnfold = GetControl<Button>("btn_ComUnfold");
        btn_EquipUnfold = GetControl<Button>("btn_EquipUnfold");
        btn_ZiyuanUnfold = GetControl<Button>("btn_ZiyuanUnfold");
        btn_TaskUnfold = GetControl<Button>("btn_TaskUnfold");
        GetControl<Button>("X").onClick.AddListener(() =>
        {
            GetControl<Toggle>("tog_CtrlEquipTypeView").isOn = false;
            EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString());
        });
        GetControl<Toggle>("tog_CtrlEquipTypeView").onValueChanged.AddListener(a =>
        {
            if (!a) EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString());
        });

        btn_ComUnfold.onClick.AddListener(() => retractOrUnfold(true, 0));
        btn_EquipUnfold.onClick.AddListener(() => retractOrUnfold(true, 1));
        btn_ZiyuanUnfold.onClick.AddListener(() => retractOrUnfold(true, 2));
        btn_TaskUnfold.onClick.AddListener(() => retractOrUnfold(true, 3));
        GetControl<Button>("btn_ComRecover").onClick.AddListener(() => retractOrUnfold(false, 0));
        GetControl<Button>("btn_EquipRecover").onClick.AddListener(() => retractOrUnfold(false, 1));
        GetControl<Button>("btn_ZiyuanRecover").onClick.AddListener(() => retractOrUnfold(false, 2));
        GetControl<Button>("btn_TaskRecover").onClick.AddListener(() => retractOrUnfold(false, 3));


        myCommanderInfoShow = GetComponentInChildren<MyCommanderView>(true);

        allCommanderIds = new Dictionary<string, string>();
        allEquipCells = new List<EquipCell>();
        allZiYuanCells = new List<ZiYuanCell>();
        allCommanderCells = new Dictionary<string, CommanderCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        level = (int)userData;
        GetControl<Toggle>("tog_CtrlEquipTypeView").interactable = level == 1;
        if (level != 1)
            retractOrUnfold(false, 0);
#if !UNITY_EDITOR
        showView();
#endif
        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
        EventManager.Instance.AddEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
        isMoveCmGo = false;
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(Enums.EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
        EventManager.Instance.RemoveEventListener<int, string>(EventType.ChangeObjController.ToString(), OnRunningChangeObjCom);
    }

    private void showView()
    {
        myCommanderInfoShow.Init(MyDataInfo.leadId, OnChooseCommander);
        //获取子指挥官,一级指挥端才需要显示，只显示别人
        if (level == 1)
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                //找到了主角,并且不是自己，就要展示
                if (!string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id) && allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) != null)
                {
                    //如果这个玩家没有进入房间，就跳过
                    string myRoleId = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, allBObjects[i].BObject.Id)).RoleId;
                    if (string.IsNullOrEmpty(myRoleId)) continue;
                    if (MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, allBObjects[i].BObject.Id)).ClientLevel < 0) continue;
                    var itemObj = allBObjects[i];
                    var itemCell = Instantiate(ccPrefab, commanderParent);
                    itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, OnChooseCommander);
                    itemCell.gameObject.SetActive(true);
                    allCommanderIds.Add(itemObj.BObject.Id, itemObj.BObject.Info.Name);
                    allCommanderCells.Add(itemObj.BObject.Id, itemCell);
                }

                if (string.Equals(MyDataInfo.leadId, allBObjects[i].BObject.Id))
                    allCommanderIds.Add(allBObjects[i].BObject.Id, allBObjects[i].BObject.Info.Name);
            }

            //获取场景中标识了模板的对象，展示出来
            for (int i = 0; i < allBObjects.Length; i++)
            {
                var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
                if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 4) != null)
                {
                    var itemObj = allBObjects[i];
                    var itemCell = Instantiate(etcPrefab, equipTypeParent);
                    itemCell.Init(itemObj.name, itemObj.BObject.Id, OnChooseEquipType);
                    itemCell.gameObject.SetActive(true);
                }
            }
        }

        //获取场景中的资源，展示
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 1 || y.Id == 5) != null)
            {
                var itemObj = allBObjects[i];
                ZiYuanBase zyObj = itemObj.GetComponent<ZiYuanBase>();
                bool isDisaster = zyObj.ZiYuanType == ZiYuanType.Hospital || zyObj.ZiYuanType == ZiYuanType.RescueStation ||
                                  zyObj.ZiYuanType == ZiYuanType.DisasterArea || zyObj.ZiYuanType == ZiYuanType.SourceOfAFire;
                if (isDisaster) continue;
                var itemCell = Instantiate(zycPrefab, isDisaster ? disasterParent : ziYuanParent);
                itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, zyObj, OnChangeZiYuanBelongTo, OnMoveCm);
                itemCell.gameObject.SetActive(true);
                allZiYuanCells.Add(itemCell);
            }
        }
    }

    private void Update()
    {
        if (MyDataInfo.gameStartTime > 0 && MyDataInfo.gameStartTime < 1)
            startTime.text = "开始时间 " + DateTime.Now.ToString("HH:mm:ss");
        currentTime.text = "当前时间 " + DateTime.Now.ToString("HH:mm:ss");

        if (isMoveCmGo)
        {
            var rectPos = UIManager.Instance.GetUIPanel<UIMap>(UIName.UIMap).resolutionRatioNormalized(Input.mousePosition);
            CmGo.anchoredPosition = UIManager.Instance.GetUIPanel<UIMap>(UIName.UIMap).mousePos2UI(rectPos);
        }
    }

    private void retractOrUnfold(bool isRetract, int type)
    {
        switch (type)
        {
            case 0:
                btn_ComUnfold.gameObject.SetActive(!isRetract);
                if (!isRetract)
                {
                    commanderViewGo.anchoredPosition = new Vector2(commanderViewGo.anchoredPosition.x - 104, commanderViewGo.anchoredPosition.y);
                    goListViewGo.anchoredPosition = new Vector2(goListViewGo.anchoredPosition.x - 90, commanderViewGo.anchoredPosition.y);
                }
                else
                {
                    commanderViewGo.anchoredPosition = new Vector2(commanderViewGo.anchoredPosition.x + 104, commanderViewGo.anchoredPosition.y);
                    goListViewGo.anchoredPosition = new Vector2(goListViewGo.anchoredPosition.x + 90, commanderViewGo.anchoredPosition.y);
                }

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
            case 3:
                btn_TaskUnfold.gameObject.SetActive(!isRetract);
                taskViewGo.SetActive(isRetract);
                break;
        }
    }

    private void OnChooseEquipType(string id)
    {
        if (ProgrammeDataManager.Instance.GetCurrentData == null)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "请先创建方案再进行编辑！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        if (MyDataInfo.gameState != GameState.FirstLevelCommanderEditor)
        {
            ConfirmatonInfo infob = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "当前阶段不可更改场景中的装备！" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infob);
            return;
        }

        BObjectModel chooseObject = null;
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(id, allBObjects[i].BObject.Id))
            {
                chooseObject = allBObjects[i];
                break;
            }
        }

        if (chooseObject != null)
        {
            EventManager.Instance.EventTrigger<object>(Enums.EventType.TransferEditingInfo.ToString(), chooseObject.BObject.Id);
        }
    }

    private string currentSelectComId = "";

    private void OnChooseCommander(string id)
    {
        if (!string.Equals(currentSelectComId, id))
        {
            if (allCommanderCells.ContainsKey(currentSelectComId)) allCommanderCells[currentSelectComId].SetSelect(false);
            if (allCommanderCells.ContainsKey(id)) allCommanderCells[id].SetSelect(true);
            currentSelectComId = id;
        }
        // else return;

        var currentCommander = MyDataInfo.playerInfos.Find(x => string.Equals(id, x.RoleId));
        //更改了某个对象控制着会调用这个，导致消息列表取消其他内容显示
        EventManager.Instance.EventTrigger(EventType.ChangeCurrentCom.ToString(), currentCommander.ClientLevelName);
        if (currentCommander.ClientLevel == 1)
        {
            for (int i = 0; i < allEquipCells.Count; i++)
            {
                allEquipCells[i].gameObject.SetActive(true);
            }

            for (int i = 0; i < allZiYuanCells.Count; i++)
            {
                allZiYuanCells[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < allEquipCells.Count; i++)
            {
                allEquipCells[i].gameObject.SetActive(string.Equals(allEquipCells[i].equipBeUseCommander, id));
            }

            for (int i = 0; i < allZiYuanCells.Count; i++)
            {
                bool isShow = allZiYuanCells[i].allcoms.Find(x => string.Equals(x.comId, id));
                allZiYuanCells[i].gameObject.SetActive(isShow);
            }
        }

        UIManager.Instance.GetUIPanel<UIAttributeView>(UIName.UIAttributeView).OnChooseCommander(id);
    }

    private void OnAddEquipView(EquipBase equip)
    {
        var itemObj = equip;
        var itemCell = Instantiate(ecPrefab, equipParent);
        itemCell.Init(itemObj, allCommanderIds, OnChangeEquipBelongTo);
        itemCell.gameObject.SetActive(true);
        allEquipCells.Add(itemCell);
        if (MyDataInfo.MyLevel != 1)
        {
            itemCell.gameObject.SetActive(string.Equals(equip.BeLongToCommanderId, MyDataInfo.leadId));
        }
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

    private void OnInitZiYuanBeUsed(ZiYuanBase data)
    {
        var itemZiyuan = allZiYuanCells.Find(x => string.Equals(x.myEntityId, data.main.BObjectId));
        itemZiyuan?.ShowComCtrls(data.beUsedCommanderIds);
        // var itemTask = allTaskCells.Find(x => string.Equals(x.myEntityId, data.main.BObjectId));
        // itemTask?.ShowComCtrls(data.beUsedCommanderIds);


        if (MyDataInfo.MyLevel != 1)
        {
            if (itemZiyuan != null)
            {
                bool isShow = itemZiyuan.allcoms.Find(x => string.Equals(x.comId, MyDataInfo.leadId));
                itemZiyuan.gameObject.SetActive(isShow);
            }
        }
    }

    private void OnRunningChangeObjCom(int objType, string id)
    {
        if (objType == 1)
        {
            var itemEquipCell = allEquipCells.Find(x => string.Equals(x.equipObjectId, id));
            if (itemEquipCell != null) itemEquipCell.RefreshComShow();
        }
        else
        {
            var itemZyCell = allZiYuanCells.Find(x => string.Equals(x.myEntityId, id));
            if (itemZyCell != null) itemZyCell.RefreshComShow();
        }

        OnChooseCommander(MyDataInfo.leadId);
    }

    private void OnChangeEquipBelongTo(string equipId, string commanderId)
    {
        //修改数据中的信息
        ProgrammeDataManager.Instance.GetEquipDataById(equipId).controllerId = commanderId;

        if (MyDataInfo.gameState >= GameState.Preparation)
        {
            ChangeController ccData = new ChangeController()
            {
                objType = 1, ChangeTargetId = equipId, currentComs = new List<string> { commanderId }
            };
            string jsonData = JsonConvert.SerializeObject(ccData);
            string sendData = AESUtils.Encrypt(jsonData);
            //游戏进行中修改的话，发送给所有人
            for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
            {
                sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendChangeController, sendData);
            }
        }
    }

    public bool OnChangeZiYuanBelongTo(string ziYuanId, string commanderId, bool addOrRemove)
    {
        bool isChangeSuc = ProgrammeDataManager.Instance.ChangeZiYuanData(ziYuanId, commanderId, addOrRemove);

        if (isChangeSuc)
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (string.Equals(ziYuanId, allBObjects[i].BObject.Id))
                {
                    var zyObj = allBObjects[i].GetComponent<ZiYuanBase>();
                    if (addOrRemove) zyObj.AddBeUsdCom(commanderId);
                    else zyObj.RemoveBeUsedCom(commanderId);

                    #region 发送给所有人

                    if (MyDataInfo.gameState >= GameState.Preparation)
                    {
                        ChangeController ccData = new ChangeController()
                        {
                            objType = 2, ChangeTargetId = zyObj.BobjectId, currentComs = zyObj.beUsedCommanderIds
                        };
                        string jsonData = JsonConvert.SerializeObject(ccData);
                        string sendData = AESUtils.Encrypt(jsonData);
                        //游戏进行中修改的话，发送给所有人
                        for (int j = 0; j < MyDataInfo.playerInfos.Count; j++)
                        {
                            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[j].RoleId, (int)Enums.MessageID.SendChangeController, sendData);
                        }
                    }

                    #endregion

                    break;
                }
            }
        }

        return isChangeSuc;
    }

    private void OnMoveCm(bool isShow, string showName)
    {
        CmGo.gameObject.SetActive(isShow);
        isMoveCmGo = isShow;
        if (isShow)
        {
            CmGo.GetComponentInChildren<Text>().text = showName;
        }
    }
}