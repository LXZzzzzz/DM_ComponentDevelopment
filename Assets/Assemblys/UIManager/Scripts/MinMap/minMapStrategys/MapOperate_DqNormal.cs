using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using EventType = Enums.EventType;

public class MapOperate_DqNormal : MapOperateLogicBase
{
    public MapOperate_DqNormal(UIMap mainLogic) : base(mainLogic)
    {
    }

    public override void OnEnter()
    {
    }

    public override void OnLeftClickIcon(IconCellBase clickIcon)
    {
        if (clickIcon is AirIconCell)
        {
            if (MyDataInfo.MyLevel != 3)
            {
                EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), clickIcon.belongToId);
                return;
            }

            var equip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, (clickIcon as AirIconCell).belongToId));
            if (string.Equals(equip.BeLongToCommanderId, MyDataInfo.leadId))
                EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), clickIcon.belongToId);
            else
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "机长只能查看自己所控直升机");
        }
        else if (clickIcon is ZiYuanIconCell)
        {
            EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), clickIcon.belongToId);
        }
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
    }

    public override void OnUpdate()
    {
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
        EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), string.Empty);
    }

    public override void OnRightClickMap(Vector2 pos)
    {
    }

    public override void OnExit()
    {
    }
}