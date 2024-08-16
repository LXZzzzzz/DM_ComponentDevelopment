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

    public override void Init()
    {
        base.Init();
        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        parent = transform.Find("IconObjParent");
        tdic = transform.Find("IconPrefabs/ThreeD_AirIconCell").GetComponent<ThreeD_AirIconCell>();
        tzic = transform.Find("IconPrefabs/ThreeD_ZiYuanIconCell").GetComponent<ThreeD_ZiYuanIconCell>();
        allIconCells = new Dictionary<string, ThreeD_AirIconCell>();
        StartCoroutine(InitZiYuanIcon());
    }

    IEnumerator InitZiYuanIcon()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < allBObjects.Length; i++)
        {
            //找到场景中所有资源，显示到地图上
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 5) != null)
            {
                var itemIcon = Instantiate(tzic, parent);
                itemIcon.Init(allBObjects[i].GetComponent<ZiYuanBase>());
                itemIcon.gameObject.SetActive(true);
            }
        }
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

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.RemoveEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }
}