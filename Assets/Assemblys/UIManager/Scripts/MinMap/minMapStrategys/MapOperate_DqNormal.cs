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
        EventManager.Instance.EventTrigger(EventType.DqChooseGo.ToString(), clickIcon.belongToId);
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