using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EquipBase = ToolsLibrary.EquipPart.EquipBase;
using EventType = Enums.EventType;

public class UIDirectorView : BasePanel
{
    private RectTransform equipParent;
    private RectTransform ziYuanParent;
    private RectTransform taskParent; //灾区组件所展示的列表
    private RectTransform trainParent;

    private EquipCell ecPrefab;
    private ZiYuanCell zycPrefab;
    private TaskCell taskPrefab;
    private TrainCell trainPrefab;
    private Text startTime, currentTime;

    private int level;
    private Dictionary<string, string> allCommanderIds; //存储所有指挥端Id和 对应的名称
    private List<EquipCell> allEquipCells; //存储所有装备cell
    private List<ZiYuanCell> allZiYuanCells; //存储所有资源cell，为了后面数据修改
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改
    private List<TrainCell> allTrainCells; //存储所有训练点


    public override void Init()
    {
        base.Init();
        equipParent = GetControl<ScrollRect>("EquipsView").content;
        ecPrefab = GetComponentInChildren<EquipCell>(true);
        ziYuanParent = GetControl<ScrollRect>("ZiYuanView").content;
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        taskParent = GetControl<ScrollRect>("TaskListView").content;
        taskPrefab = GetComponentInChildren<TaskCell>(true);
        trainParent = GetControl<ScrollRect>("TrainsView").content;
        trainPrefab = GetComponentInChildren<TrainCell>(true);
        startTime = GetControl<Text>("startTimeShow");
        currentTime = GetControl<Text>("currentTimeShow");


        allCommanderIds = new Dictionary<string, string>();
        allEquipCells = new List<EquipCell>();
        allZiYuanCells = new List<ZiYuanCell>();
        allTaskCells = new List<TaskCell>();
        allTrainCells = new List<TrainCell>();
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
        EventManager.Instance.AddEventListener<string>(EventType.HideGoIcon.ToString(), OnHideZyShow);
        EventManager.Instance.AddEventListener<string>(EventType.CompleteATrainPoint.ToString(), OnCompleteTrainPoint);
        showTrainsData();
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
        EventManager.Instance.RemoveEventListener<string>(EventType.HideGoIcon.ToString(), OnHideZyShow);
        EventManager.Instance.RemoveEventListener<string>(EventType.CompleteATrainPoint.ToString(), OnCompleteTrainPoint);
    }

    private void Update()
    {
        //这个是在游戏开始第一帧才更新“开始时间”
        if (MyDataInfo.gameStartTime > 0 && MyDataInfo.gameStartTime < 1)
            startTime.text = "开始时间 " + DateTime.Now.ToString("HH:mm:ss");
        currentTime.text = "当前时间 " + DateTime.Now.ToString("HH:mm:ss");

        for (int i = 0; i < allEquipCells.Count; i++)
        {
            allEquipCells[i].gameObject.SetActive(allEquipCells[i].equipGoIsShow);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("看卡卡可能喀什地方你卡上饭卡函数的返回");
        }
    }

    private string currentSelectComId = "";
    private bool isRefreshMsgView = true;

    private void OnAddEquipView(EquipBase equip)
    {
        var itemObj = equip;
        var itemCell = Instantiate(ecPrefab, equipParent);
        itemCell.Init(level, itemObj, allCommanderIds, null);
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
            itemCell.Init(level, zyObj, null);
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
        for (int i = 0; i < allTaskCells.Count; i++)
        {
            if (string.Equals(allTaskCells[i].myEntityId, deleId))
            {
                //这里应该得检测资源下有没有任务，如果有要删除
                Destroy(allTaskCells[i].gameObject);
                allTaskCells.RemoveAt(i);
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

    private void OnHideZyShow(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            allZiYuanCells.ForEach(x => x.gameObject.SetActive(true));
            allTaskCells.ForEach(x => x.gameObject.SetActive(true));
            return;
        }

        allZiYuanCells.ForEach(x => x.gameObject.SetActive(!string.Equals(x.myEntityId, id)));
        allTaskCells.ForEach(x => x.gameObject.SetActive(!string.Equals(x.myEntityId, id)));
    }

    private void showTrainsData()
    {
        string filePath = Path.Combine(Application.dataPath, "MapLib", "XmlData", "TrainPointData.xml");

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        // 读取文件内容
        string fileContent = File.ReadAllText(filePath);

        // 创建一个 XmlDocument 对象
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(fileContent);

        // 获取根节点
        XmlNode root = xmlDoc.DocumentElement;

        // 遍历所有节点
        foreach (XmlNode bookNode in root.ChildNodes)
        {
            string id = bookNode.Attributes["id"].Value;

            string title = bookNode["title"]?.InnerText;
            string type = bookNode["type"]?.InnerText;
            string score = bookNode["score"]?.InnerText;
            if (string.IsNullOrEmpty(score) || int.Parse(score) == 0) continue;

            TrainCell itemCell = Instantiate(trainPrefab, trainParent);
            itemCell.Init(id, title, type, score);
            itemCell.gameObject.SetActive(true);
            allTrainCells.Add(itemCell);
        }
    }

    private void OnCompleteTrainPoint(string type)
    {
        allTrainCells.ForEach(x => x.SetComplete(type));
    }
}