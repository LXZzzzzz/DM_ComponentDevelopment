using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using EventType = Enums.EventType;

public class UIThreeDIconView : BasePanel
{
    private Transform parent;
    private ThreeD_AirIconCell tdic;
    private Dictionary<string, ThreeD_AirIconCell> allIconCells;
    private ThreeD_ZiYuanIconCell tzic;

    private List<ThreeD_ZiYuanIconCell> allZiYuanCells;

    public override void Init()
    {
        base.Init();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
        EventManager.Instance.AddEventListener<ZiYuanBase>(EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
        
        parent = transform.Find("IconObjParent");
        tdic = transform.Find("IconPrefabs/ThreeD_AirIconCell").GetComponent<ThreeD_AirIconCell>();
        tzic = transform.Find("IconPrefabs/ThreeD_ZiYuanIconCell").GetComponent<ThreeD_ZiYuanIconCell>();
        allIconCells = new Dictionary<string, ThreeD_AirIconCell>();
        allZiYuanCells = new List<ThreeD_ZiYuanIconCell>();
    }
    private void creatAirCell(EquipBase equip)
    {
        var itemIcon = Instantiate(tdic, parent);
        itemIcon.Init(equip);
        itemIcon.gameObject.SetActive(true);
        allIconCells.Add(equip.BObjectId, itemIcon);
    }

    private void desAir(string id)
    {
        foreach (var iconCell in allIconCells)
        {
            if (string.Equals(iconCell.Key, id))
            {
                Destroy(iconCell.Value.gameObject);
                allIconCells.Remove(id);
                break;
            }
        }
    }
    private void OnAddZyZq(ZiYuanBase zyObj)
    {
        var itemIcon = Instantiate(tzic, parent);
        itemIcon.Init(zyObj);
        itemIcon.gameObject.SetActive(true);
        allZiYuanCells.Add(itemIcon);
    }
    private void OnRemoveZyZq(string deleId)
    {
        for (int i = 0; i < allZiYuanCells.Count; i++)
        {
            if (string.Equals(allZiYuanCells[i].ziYuanItem.BobjectId, deleId))
            {
                //这里应该得检测资源下有没有任务，如果有要删除
                Destroy(allZiYuanCells[i].gameObject);
                allZiYuanCells.RemoveAt(i);
                break;
            }
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
        EventManager.Instance.RemoveEventListener<ZiYuanBase>(EventType.CreatAZiyuanIcon.ToString(), OnAddZyZq);
        EventManager.Instance.RemoveEventListener<string>(EventType.DestoryZiyuanIcon.ToString(), OnRemoveZyZq);
    }
}