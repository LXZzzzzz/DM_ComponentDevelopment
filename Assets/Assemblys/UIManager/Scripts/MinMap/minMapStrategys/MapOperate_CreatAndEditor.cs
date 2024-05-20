using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class MapOperate_CreatAndEditor : MapOperateLogicBase
{
    private string creatTargetTemplate;

    public override void OnEnter(object initData)
    {
        if (initData is string)
        {
            //todo：打开时刻跟随鼠标显示的模板对象
            creatTargetTemplate = (string)initData;
        }
        else if (initData is List<EquipBase>)
        {
            //传过来装备列表，说明这是一级发布了方案，做一个初始化后，直接切回普通模式
            var allObjModels = initData as List<EquipBase>;
            for (int i = 0; i < allObjModels?.Count; i++)
            {
                if (allObjModels[i].GetComponent<EquipBase>() == null) continue;
                creatAirCell(allObjModels[i]);
            }
#if UNITY_EDITOR
            mainLogic.SwitchMapLogic(OperatorState.PlanningPath);
#else
            mainLogic.SwitchMapLogic(OperatorState.Normal);
#endif
        }

        EventManager.Instance.AddEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
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
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        //通知主角在场景对应位置创建实体
        EventManager.Instance.EventTrigger(EventType.CreatEquipEntity.ToString(), creatTargetTemplate, uiPos2WorldPos(pos));
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        //todo:关闭预创建显示的图标

        mainLogic.SwitchMapLogic(OperatorState.Normal);
    }

    public override void OnExit()
    {
        EventManager.Instance.RemoveEventListener<EquipBase>(Enums.EventType.CreatEquipCorrespondingIcon.ToString(), creatAirCell);
    }

    public void creatAirCell(EquipBase equip)
    {
        var itemCell = Object.Instantiate(mainLogic.airIconPrefab, mainLogic.iconCellParent);
        itemCell.gameObject.SetActive(true);
        //传入这个组件的基本信息，和选择后的回调
        itemCell.transform.position = worldPos2UiPos(equip.gameObject.transform.position);
        (itemCell as AirIconCell).Init(equip, equip.BObjectId, mainLogic.OnChooseObj, worldPos2UiPos);
        mainLogic.allIconCells.Add(equip.BObjectId, itemCell);
    }

    public MapOperate_CreatAndEditor(UIMap mainLogic) : base(mainLogic)
    {
    }
}