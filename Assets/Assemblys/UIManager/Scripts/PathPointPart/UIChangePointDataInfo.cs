using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UIChangePointDataInfo : BasePanel
{
    private List<TaskBase> currentTasks;
    private GameObject taskCell;
    private Transform tasksParent;

    private string pointId;
    private Dictionary<string, SkillType> skillTypes;

    public override void Init()
    {
        base.Init();
        taskCell = transform.Find("View/infos/taskCell").gameObject;
        tasksParent = GetControl<ScrollRect>("ScrollRectPrefab").content;
        GetControl<Button>("sure").onClick.AddListener(OnSure);
        GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIChangePointDataInfo));
        GetControl<Button>("addTaskBtn").onClick.AddListener(OnAddTask);
        skillTypes = new Dictionary<string, SkillType>();
        skillTypes.Add("起飞", SkillType.TakeOff);
        skillTypes.Add("补给", SkillType.Supply);
        skillTypes.Add("降落", SkillType.Landing);
        skillTypes.Add("取水", SkillType.WaterIntaking);
        skillTypes.Add("投水", SkillType.WaterPour);
        skillTypes.Add("装载物资", SkillType.LadeGoods);
        skillTypes.Add("卸载物资", SkillType.UnLadeGoods);
        skillTypes.Add("空投物资", SkillType.AirdropGoods);
        skillTypes.Add("装载人员", SkillType.Manned);
        skillTypes.Add("安置人员", SkillType.PlacementOfPersonnel);
        skillTypes.Add("索降救援", SkillType.CableDescentRescue);
        var dd = taskCell.GetComponentInChildren<Dropdown>();
        dd.options.Clear();
        dd.options.Add(new Dropdown.OptionData("起飞"));
        dd.options.Add(new Dropdown.OptionData("降落"));
        dd.options.Add(new Dropdown.OptionData("补给"));
        if (MyDataInfo.gameScene == 1)
        {
            dd.options.Add(new Dropdown.OptionData("取水"));
            dd.options.Add(new Dropdown.OptionData("投水"));
        }

        if (MyDataInfo.gameScene == 2)
        {
            dd.options.Add(new Dropdown.OptionData("装载物资"));
            dd.options.Add(new Dropdown.OptionData("卸载物资"));
            dd.options.Add(new Dropdown.OptionData("空投物资"));
            dd.options.Add(new Dropdown.OptionData("装载人员"));
            dd.options.Add(new Dropdown.OptionData("安置人员"));
            dd.options.Add(new Dropdown.OptionData("索降救援"));
        }
    }

    private void OnAddTask()
    {
        if (currentTasks == null) currentTasks = new List<TaskBase>();
        var itemTask = Instantiate(taskCell, tasksParent);
        itemTask.transform.GetComponentInChildren<Text>().text = $"第{currentTasks.Count + 1}个操作：";
        itemTask.gameObject.SetActive(true);
        currentTasks.Add(new taskTest() { orderNumber = currentTasks.Count, testStr = "啦啦啦啦", isRuned = false });
    }

    private void OnSure()
    {
        for (int i = 0; i < currentTasks.Count; i++)
        {
            Dropdown dp = tasksParent.GetChild(currentTasks[i].orderNumber).GetComponentInChildren<Dropdown>();
            currentTasks[i].runSkillType = skillTypes[dp.options[dp.value].text];
        }

        PathPointManager.Instance.ChangePointDataInfo(pointId, currentTasks);
        Close(UIName.UIChangePointDataInfo);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        transform.SetAsLastSibling();
        PathPoint ppd = (PathPoint)userData;
        pointId = ppd.pointId;
        currentTasks = ppd.tasks;

        for (int i = 0; i < currentTasks.Count; i++)
        {
            var itemTask = Instantiate(taskCell, tasksParent);
            itemTask.transform.GetComponentInChildren<Text>().text = $"第{currentTasks[i].orderNumber}个操作：";
            itemTask.GetComponentInChildren<Dropdown>().value = (int)currentTasks[i].runSkillType;
            itemTask.gameObject.SetActive(true);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        for (int i = 0; i < tasksParent.childCount; i++)
        {
            Destroy(tasksParent.GetChild(i).gameObject);
        }
    }
}