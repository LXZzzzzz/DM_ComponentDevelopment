using System.Collections.Generic;
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

    public override void Init()
    {
        base.Init();
        taskCell = transform.Find("View/infos/taskCell").gameObject;
        tasksParent = GetControl<ScrollRect>("ScrollRectPrefab").content;
        GetControl<Button>("sure").onClick.AddListener(OnSure);
        GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIChangePointDataInfo));
        GetControl<Button>("addTaskBtn").onClick.AddListener(OnAddTask);
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
            currentTasks[i].runSkillType = (SkillType)tasksParent.GetChild(currentTasks[i].orderNumber).GetComponentInChildren<Dropdown>().value;
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