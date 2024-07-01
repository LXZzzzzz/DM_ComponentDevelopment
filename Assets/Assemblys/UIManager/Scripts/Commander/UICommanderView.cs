using System;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EquipBase = ToolsLibrary.EquipPart.EquipBase;

public class UICommanderView : BasePanel
{
    private RectTransform equipTypeParent;
    private RectTransform equipParent;
    private RectTransform commanderParent;
    private RectTransform ziYuanParent;
    private RectTransform taskParent;
    private EquipTypeCell etcPrefab;
    private EquipCell ecPrefab;
    private CommanderCell ccPrefab;
    private ZiYuanCell zycPrefab;
    private TaskCell taskPrefab;
    private Text startTime, currentTime;

    private MyCommanderView myCommanderInfoShow;

    private int level;
    private Dictionary<string, string> allCommanderIds; //存储所有指挥端Id和 对应的名称
    private List<EquipCell> allEquipCells; //存储所有装备cell
    private List<ZiYuanCell> allZiYuanCells; //存储所有资源cell，为了后面数据修改
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改
    private Dictionary<string, CommanderCell> allCommanderCells; //存储所有玩家cell


    public override void Init()
    {
        base.Init();
        equipTypeParent = GetControl<ScrollRect>("EquipsTypes").content;
        etcPrefab = GetComponentInChildren<EquipTypeCell>(true);
        equipParent = GetControl<ScrollRect>("EquipsView").content;
        ecPrefab = GetComponentInChildren<EquipCell>(true);
        commanderParent = GetControl<ScrollRect>("CommandersView").content;
        ccPrefab = GetComponentInChildren<CommanderCell>(true);
        ziYuanParent = GetControl<ScrollRect>("ZiYuanView").content;
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        taskParent = GetControl<ScrollRect>("TaskListView").content;
        taskPrefab = GetComponentInChildren<TaskCell>(true);
        startTime = GetControl<Text>("startTimeShow");
        currentTime = GetControl<Text>("currentTimeShow");
        GetControl<Button>("X").onClick.AddListener(() => EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString()));
        GetControl<Toggle>("tog_CtrlEquipTypeView").onValueChanged.AddListener(a =>
        {
            if (!a) EventManager.Instance.EventTrigger(Enums.EventType.CloseCreatTarget.ToString());
        });

        myCommanderInfoShow = GetComponentInChildren<MyCommanderView>(true);

        allCommanderIds = new Dictionary<string, string>();
        allEquipCells = new List<EquipCell>();
        allZiYuanCells = new List<ZiYuanCell>();
        allTaskCells = new List<TaskCell>();
        allCommanderCells = new Dictionary<string, CommanderCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        level = (int)userData;
        GetControl<Toggle>("tog_CtrlEquipTypeView").interactable = level == 1;
#if !UNITY_EDITOR
        showView();
#endif
        EventManager.Instance.AddEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.AddEventListener<ZiYuanBase>(Enums.EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), OnAddEquipView);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), OnRemoveEquip);
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(Enums.EventType.InitZiYuanBeUsed.ToString(), OnInitZiYuanBeUsed);
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


        int taskIndex = 0;
        //获取场景中的资源和任务，展示
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 1 || y.Id == 5) != null)
            {
                var itemObj = allBObjects[i];
                var itemCell = Instantiate(zycPrefab, ziYuanParent);
                itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, itemObj.GetComponent<ZiYuanBase>(), OnChangeZiYuanBelongTo);
                itemCell.gameObject.SetActive(true);
                allZiYuanCells.Add(itemCell);
            }

            //任务列表展示
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 3) != null)
            {
                taskIndex++;
                //任务与资源逻辑应该是一样的
                var itemObj = allBObjects[i];
                var itemCell = Instantiate(taskPrefab, taskParent);
                var itemzy = itemObj.gameObject.GetComponent<ZiYuanBase>();
                itemCell.Init("任务" + taskIndex, itemzy, OnChangeZiYuanBelongTo);
                itemCell.gameObject.SetActive(true);
                allTaskCells.Add(itemCell);
            }
        }
    }

    private void Update()
    {
        if (MyDataInfo.gameStartTime > 0 && MyDataInfo.gameStartTime < 1)
            startTime.text = "开始时间 " + DateTime.Now.ToString("HH:mm:ss");
        currentTime.text = "当前时间 " + DateTime.Now.ToString("HH:mm:ss");
    }

    private void OnChooseEquipType(string id)
    {
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
        else return;

        var currentCommander = MyDataInfo.playerInfos.Find(x => string.Equals(id, x.RoleId));
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

            for (int i = 0; i < allTaskCells.Count; i++)
            {
                allTaskCells[i].gameObject.SetActive(true);
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

            for (int i = 0; i < allTaskCells.Count; i++)
            {
                bool isShow = allTaskCells[i].allcoms.Find(x => string.Equals(x.comId, id));
                allTaskCells[i].gameObject.SetActive(isShow);
            }
        }
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
        var itemTask = allTaskCells.Find(x => string.Equals(x.myEntityId, data.main.BObjectId));
        itemTask?.ShowComCtrls(data.beUsedCommanderIds);


        if (MyDataInfo.MyLevel != 1)
        {
            if (itemZiyuan != null)
            {
                bool isShow = itemZiyuan.allcoms.Find(x => string.Equals(x.comId, MyDataInfo.leadId));
                itemZiyuan.gameObject.SetActive(isShow);
            }

            if (itemTask != null)
            {
                bool isShow = itemTask.allcoms.Find(x => string.Equals(x.comId, MyDataInfo.leadId));
                itemTask.gameObject.SetActive(isShow);
            }
        }
    }

    private void OnChangeEquipBelongTo(string equipId, string commanderId)
    {
        //修改数据中的信息
        ProgrammeDataManager.Instance.GetEquipDataById(equipId).controllerId = commanderId;
    }

    private bool OnChangeZiYuanBelongTo(string ziYuanId, string commanderId, bool addOrRemove)
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
                    break;
                }
            }
        }

        return isChangeSuc;
    }
}