using System.Collections.Generic;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPathPointsShow : BasePanel
{
    private PathPointCell pathPointCell;
    private Transform pathPointsParent;

    public override void Init()
    {
        base.Init();
        GetControl<Button>("close").onClick.AddListener(() => Close(UIName.UIPathPointsShow));
        pathPointCell = transform.Find("View/infos/pathPointCell").GetComponent<PathPointCell>();
        pathPointsParent = GetControl<ScrollRect>("ScrollRectPrefab").content;
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        transform.SetAsLastSibling();
        ShowPathPointsData sppd = (ShowPathPointsData)userData;

        for (int i = 0; i < sppd.allViaPointData.Count; i++)
        {
            var pathPointItem = Instantiate(pathPointCell, pathPointsParent);
            pathPointItem.Init(sppd.allViaPointData[i], sppd.RemoveAction, sppd.InsertAction);
            pathPointItem.gameObject.SetActive(true);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        for (int i = 0; i < pathPointsParent.childCount; i++)
        {
            Destroy(pathPointsParent.GetChild(i).gameObject);
        }
    }
}