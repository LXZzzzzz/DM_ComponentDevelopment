using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;

public class MapOperate_Normal : MapOperateLogicBase
{
    public override void OnEnter()
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
            Debug.Log($"经过了{(targetIconCell as PointIconCell).allViaPointIds?.Count}个点");
#else
                    mainLogic.sender.LogError("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
#endif
        }

        if (targetIconCell is ZiYuanIconCell)
        {
            EventManager.Instance.EventTrigger(EventType.ChooseZiyuan.ToString(), targetIconCell.belongToId);

            for (int i = 0; i < mainLogic.allBObjects.Length; i++)
            {
                if (string.Equals(mainLogic.allBObjects[i].BObject.Id, targetIconCell.belongToId))
                {
                    EventManager.Instance.EventTrigger(EventType.MapChooseIcon.ToString(), mainLogic.allBObjects[i]);
                    break;
                }
            }

            return;
            //如果是机场的话，弹出飞机列表，飞机列表点击起飞，把飞机从机场中去除，并把飞机状态设为起飞状态。 
            ZiYuanBase zy = (targetIconCell as ZiYuanIconCell).ziYuanItem;
            if (MyDataInfo.MyLevel != 1 &&
                (zy.beUsedCommanderIds == null || zy.beUsedCommanderIds.Find(x => string.Equals(x, MyDataInfo.leadId)) == null)) return;
            if (zy.ZiYuanType == ZiYuanType.Airport)
            {
                AirportAircraftsInfo aai = new AirportAircraftsInfo()
                {
                    PointPos = targetIconCell.GetComponent<RectTransform>().anchoredPosition, AircraftDatas = (zy as IAirPort)?.GetAllEquips(),
                    OnRightClickCallBack = a => openRightClickView(a, targetIconCell.GetComponent<RectTransform>().anchoredPosition)
                };
                UIManager.Instance.ShowPanel<UIAirportAircraftShowView>(UIName.UIAirportAircraftShowView, aai);
            }
        }
    }

    private void openRightClickView(string id, Vector2 pos)
    {
        EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), id);
        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id));
        if (itemEquip.currentSkill != SkillType.None) return;
        RightClickShowInfo info = new RightClickShowInfo()
        {
            PointPos = pos, ShowSkillDatas = itemEquip.GetSkillsData(), OnTriggerCallBack = itemEquip.OnSelectSkill
        };
        UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //打开指令集页面
        if (clickIcon is AirIconCell)
        {
            var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, clickIcon.belongToId));
            if (!string.Equals(MyDataInfo.leadId, itemEquip.BeLongToCommanderId)) return;
            if (itemEquip.currentSkill != SkillType.None) return;
            EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), clickIcon.belongToId);
            RightClickShowInfo info = new RightClickShowInfo()
            {
                PointPos = clickIcon.GetComponent<RectTransform>().anchoredPosition + new Vector2(50, 40),
                ShowSkillDatas = itemEquip.GetSkillsData(), OnTriggerCallBack = itemEquip.OnSelectSkill
            };
            if (info.ShowSkillDatas.Find(x => x.isUsable) != null)
                UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
        }
        else if (clickIcon is ZiYuanIconCell)
        {
            EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), clickIcon.belongToId, (clickIcon as ZiYuanIconCell).ziYuanItem.gameObject.transform.position);
            EventManager.Instance.EventTrigger(EventType.ChooseEquipToZiYuanType.ToString(), (int)(clickIcon as ZiYuanIconCell).ziYuanItem.ZiYuanType);
        }
    }

    public override void OnUpdate()
    {
    }

    public override void OnLeftClickMap(Vector2 pos)
    {
#if UNITY_EDITOR
        var itemPoint = GameObject.Instantiate(mainLogic.pointIconPrefab, mainLogic.iconCellParent);
        itemPoint.enabled = false;
        var itemGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        itemGo.transform.position = uiPos2WorldPos(pos);
        itemPoint.GetComponent<RectTransform>().anchoredPosition = worldPos2UiPos(uiPos2WorldPos(pos));
        itemPoint.gameObject.SetActive(true);
#endif
        EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), string.Empty);
        EventManager.Instance.EventTrigger(EventType.ChooseZiyuan.ToString(), string.Empty);
    }

    public override void OnRightClickMap(Vector2 pos)
    {
        EventManager.Instance.EventTrigger(EventType.MoveToTarget.ToString(), string.Empty, uiPos2WorldPos(pos));
        EventManager.Instance.EventTrigger(EventType.ChooseEquipToZiYuanType.ToString(), -1);
    }

    public override void OnExit()
    {
    }

    public MapOperate_Normal(UIMap mainLogic) : base(mainLogic)
    {
    }
}

public struct RightClickShowInfo
{
    public Vector2 PointPos;
    public List<SkillData> ShowSkillDatas;
    public UnityAction<SkillType> OnTriggerCallBack;
}

public struct AirportAircraftsInfo
{
    public Vector2 PointPos;
    public List<string> AircraftDatas;
    public UnityAction<string> OnRightClickCallBack;
}