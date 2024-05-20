using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType=Enums.EventType;

public class MapOperate_Normal : MapOperateLogicBase
{
    public override void OnEnter(object initData)
    {
    }

    public override void OnLeftClickIcon(IconCellBase targetIconCell)
    {
        //查看他的类型

        if (targetIconCell is AirIconCell)
        {
            //选中对象
            EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), targetIconCell.belongToId);
            //打开装备数据展示界面
        }

        if (targetIconCell is PointIconCell)
        {
            //选中的是标点
#if UNITY_EDITOR
            Debug.Log("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
            Debug.Log($"经过了{targetIconCell.allViaPointIds?.Count}个点");
#else
                    mainLogic.sender.LogError("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
#endif
        }

    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //todo：打开指令集页面
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), uiPos2WorldPos(pos));
    }

    public override void OnExit()
    {
        
    }

    public MapOperate_Normal(UIMap mainLogic) : base(mainLogic)
    {
    }
}
