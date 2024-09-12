using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UICommanderDirector : BasePanel
{
    private Transform mbParent;
    private ZaiQuTemplateCell zqc;

    private Transform zyParent;
    private ZiYuanCell zyZqPrefab;

    private List<ZiYuanCell> allZiYuanCells; //存储所有资源cell，为了后面数据修改
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改

    public override void Init()
    {
        base.Init();
        mbParent = GetControl<ScrollRect>("ScrollRectZQMB").content;
        zqc = transform.GetComponentInChildren<ZaiQuTemplateCell>(true);
        zyParent = GetControl<ScrollRect>("ScrollRectDQZQ").content;
        zyZqPrefab = transform.GetComponentInChildren<ZiYuanCell>(true);
        allZiYuanCells = new List<ZiYuanCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
#if !UNITY_EDITOR
        showZaiqu();
#endif
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.RemoveEventListener<string>(EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
    }

    private void showZaiqu()
    {
        //获取场景全部组件，并找到灾区模板
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem == null || tagItem.SubTags.Find(y => y.Id == 6) == null) continue;

            if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
            {
                var zyItem = allBObjects[i].GetComponent<ZiYuanBase>();
                var zqitem = Instantiate(zqc, mbParent);
                zqitem.Init(zyItem);
                zqitem.gameObject.SetActive(true);
            }
        }
    }

    private void OnAddZyZq(ZiYuanBase zyObj)
    {
        bool isDisaster = zyObj.ZiYuanType == ZiYuanType.Hospital || zyObj.ZiYuanType == ZiYuanType.RescueStation ||
                          zyObj.ZiYuanType == ZiYuanType.DisasterArea || zyObj.ZiYuanType == ZiYuanType.SourceOfAFire;
        if (!isDisaster) return;

        ZiYuanCell itemCell = Instantiate(zyZqPrefab, zyParent);
        itemCell.Init(zyObj.ziYuanName, zyObj.BobjectId, zyObj, null, null);
        itemCell.gameObject.SetActive(true);
        allZiYuanCells.Add(itemCell);
    }

    private void OnRemoveZyZq(string deleId)
    {
        for (int i = 0; i < allZiYuanCells.Count; i++)
        {
            if (string.Equals(allZiYuanCells[i].myEntityId, deleId))
            {
                //todo:这里应该得检测资源下有没有任务，如果有要删除
                Destroy(allZiYuanCells[i].gameObject);
                allZiYuanCells.RemoveAt(i);
                break;
            }
        }
    }
}