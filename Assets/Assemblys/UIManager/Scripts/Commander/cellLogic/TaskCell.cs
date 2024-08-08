using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UnityEngine.Events;

public class TaskCell : DMonoBehaviour
{
    private Toggle isComplete;
    private ITaskProgress tp;
    private Text taskProgress;
    private RectTransform comShowParent;
    private ZiYuan_ComanderCell commanderShowCell;
    private List<ZiYuan_ComanderCell> _allcoms;
    public List<ZiYuan_ComanderCell> allcoms => _allcoms;

    public void Init(string taskIndex, ZiYuanBase ziYuan)
    {
        isComplete = transform.Find("RootInfo/Tog_status").GetComponent<Toggle>();
        comShowParent = GetComponentInChildren<ScrollRect>(true).content;
        commanderShowCell = GetComponentInChildren<ZiYuan_ComanderCell>(true);
        tp = ziYuan as ITaskProgress;
        transform.Find("RootInfo/Text_taskIndex").GetComponentInChildren<Text>().text = taskIndex;
        taskProgress = transform.Find("RootInfo/Text_taskName").GetComponentInChildren<Text>();
        transform.Find("describe/Text_taskDescribe").GetComponentInChildren<Text>().text = ziYuan.ziYuanDescribe;
        GetComponentInChildren<Button>().onClick.AddListener(() =>
            EventManager.Instance.EventTrigger(Enums.EventType.ChooseZiyuan.ToString(), tp.getAssociationAssemblyId()));
        _allcoms = new List<ZiYuan_ComanderCell>();
    }

    private void LateUpdate()
    {
        if (tp == null) return;
        isComplete.isOn = tp.getTaskProgress(out string progressInfo);
        taskProgress.text = progressInfo;
        if (ProgrammeDataManager.Instance.GetCurrentData != null && ProgrammeDataManager.Instance.GetCurrentData.ZiYuanControlledList.ContainsKey(tp.getAssociationAssemblyId()))
            ChangeComsView(ProgrammeDataManager.Instance.GetCurrentData.ZiYuanControlledList[tp.getAssociationAssemblyId()]);
    }

    private void ChangeComsView(List<string> coms)
    {
        if (coms.Count == allcoms.Count) return;
        for (int i = 0; i < coms.Count; i++)
        {
            if (allcoms.Find(x => string.Equals(x.comId, coms[i])) == null)
            {
                var itemCom = Instantiate(commanderShowCell, comShowParent);
                itemCom.Init(getComName(coms[i]), coms[i], null);
                itemCom.gameObject.SetActive(true);
                allcoms.Add(itemCom);
            }
        }

        for (int i = 0; i < allcoms.Count; i++)
        {
            if (coms.Find(x => string.Equals(x, allcoms[i].comId)) == null)
            {
                Destroy(allcoms[i].gameObject);
                allcoms.Remove(allcoms[i]);
                break;
            }
        }
    }

    private string getComName(string id)
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (string.Equals(allBObjects[i].BObject.Id, id))
            {
                return allBObjects[i].BObject.Info.Name;
            }
        }

        return "无";
    }
}