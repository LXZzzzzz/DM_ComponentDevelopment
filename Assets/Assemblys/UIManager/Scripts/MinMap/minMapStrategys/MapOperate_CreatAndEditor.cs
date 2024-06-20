using System;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;
using Object = UnityEngine.Object;

public class MapOperate_CreatAndEditor : MapOperateLogicBase
{
    private string creatTargetTemplate;

    public override void OnEnter()
    {
        EventManager.Instance.AddEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.AddEventListener<string>(EventType.DestoryEquip.ToString(), destroyAirCell);
        EventManager.Instance.AddEventListener<object>(EventType.TransferEditingInfo.ToString(), parsingData);
        EventManager.Instance.AddEventListener(EventType.CloseCreatTarget.ToString(), closeCreatTarget);
    }


    private void parsingData(object data)
    {
        if (data is BObjectModel[])
        {
            var initData = data as BObjectModel[];
            //场景初始化逻辑,走完切回默认模式
            for (int i = 0; i < initData.Length; i++)
            {
                //找到场景中所有资源，显示到地图上
                var tagItem = initData[i].BObject.Info.Tags.Find(x => x.Id == 1010);
                if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 1 || y.Id == 5) != null)
                {
                    creatZiYuanCell(initData[i].BObject.Id, initData[i].gameObject.transform.position);
                }
            }

            mainLogic.SwitchMapLogic(OperatorState.Normal);
        }
        else if (data is string)
        {
            mainLogic.TempIcon.gameObject.SetActive(true);
            creatTargetTemplate = (string)data;
            mainLogic.TempIcon.GetComponent<Image>().sprite = UIManager.Instance.PicBObjects[creatTargetTemplate];
        }
        else if (data is List<EquipBase>)
        {
            Debug.LogError("这里应该不会走了");
            //传过来装备列表，说明这是一级发布了方案，做一个初始化后，直接切回普通模式
            var allObjModels = data as List<EquipBase>;
            for (int i = 0; i < allObjModels?.Count; i++)
            {
                if (allObjModels[i].GetComponent<EquipBase>() == null) continue;
                creatAirCell(allObjModels[i]);
            }
        }
    }


    public override void OnLeftClickIcon(IconCellBase clickIcon)
    {
        //todo:检测当前对象是否是机场属性，是的话才能执行创建
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //打开设置位置的功能
    }

    public override void OnUpdate()
    {
        if (!string.IsNullOrEmpty(creatTargetTemplate))
        {
            var rectPos = mainLogic.resolutionRatioNormalized(Input.mousePosition);
            mainLogic.TempIcon.anchoredPosition = mainLogic.mousePos2UI(rectPos);
        }
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        if (string.IsNullOrEmpty(creatTargetTemplate)) return;
        //这里先去数据管理器里申请创建，然后将数据ID传给创建者
        string equipId = ProgrammeDataManager.Instance.AddEquip(creatTargetTemplate, uiPos2WorldPos(pos));
        ProgrammeDataManager.Instance.GetEquipDataById(equipId).controllerId = MyDataInfo.leadId;
        //通知主角在场景对应位置创建实体
        EventManager.Instance.EventTrigger(EventType.CreatEquipEntity.ToString(), creatTargetTemplate, equipId);
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        mainLogic.TempIcon.gameObject.SetActive(false);
        creatTargetTemplate = String.Empty;
    }

    private void closeCreatTarget()
    {
        mainLogic.TempIcon.gameObject.SetActive(false);
        creatTargetTemplate = String.Empty;
    }

    public override void OnExit()
    {
        EventManager.Instance.RemoveEventListener<EquipBase>(EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
        EventManager.Instance.RemoveEventListener<string>(EventType.DestoryEquip.ToString(), destroyAirCell);
        EventManager.Instance.RemoveEventListener<object>(EventType.TransferEditingInfo.ToString(), parsingData);
        EventManager.Instance.RemoveEventListener(EventType.CloseCreatTarget.ToString(), closeCreatTarget);
    }

    private void creatAirCell(EquipBase equip)
    {
        var itemCell = Object.Instantiate(mainLogic.airIconPrefab, mainLogic.iconCellParent);
        itemCell.gameObject.SetActive(true);
        //传入这个组件的基本信息，和选择后的回调
        itemCell.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(equip.gameObject.transform.position);
        (itemCell as AirIconCell).Init(equip, equip.BObjectId, mainLogic.OnChooseObj, worldPos2UiPos);
        mainLogic.allIconCells.Add(equip.BObjectId, itemCell);
    }

    private void destroyAirCell(string id)
    {
        foreach (var iconCell in mainLogic.allIconCells)
        {
            if (string.Equals(iconCell.Key, id))
            {
                iconCell.Value.DestroyMe();
                Object.Destroy(iconCell.Value.gameObject);
                mainLogic.allIconCells.Remove(id);
                break;
            }
        }
    }

    private void creatZiYuanCell(string entityId, Vector3 pos)
    {
        var itemCell = Object.Instantiate(mainLogic.ziYuanIconPrefab, mainLogic.iconCellParent);
        itemCell.gameObject.SetActive(true);
        //传入这个组件的基本信息，和选择后的回调
        itemCell.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(pos);
        (itemCell as ZiYuanIconCell).Init(entityId, mainLogic.OnChooseObj);
        mainLogic.allIconCells.Add(entityId, itemCell);
    }

    public MapOperate_CreatAndEditor(UIMap mainLogic) : base(mainLogic)
    {
    }
}