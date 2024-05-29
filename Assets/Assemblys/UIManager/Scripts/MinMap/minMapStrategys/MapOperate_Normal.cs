using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
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
            Debug.Log($"经过了{(targetIconCell as PointIconCell).allViaPointIds?.Count}个点");
#else
                    mainLogic.sender.LogError("选中的标点是" + targetIconCell.belongToId + "的点；" + "名字是：" + targetIconCell.name);
#endif
        }

        if (targetIconCell is ZiYuanIconCell)
        {
            for (int i = 0; i < mainLogic.allBObjects.Length; i++)
            {
                if (string.Equals(mainLogic.allBObjects[i].BObject.Id,targetIconCell.belongToId))
                {
                    EventManager.Instance.EventTrigger(EventType.MapChooseIcon.ToString(),mainLogic.allBObjects[i]);
                    break;
                }
            }
        }

    }

    public override void OnRightClickIcon(IconCellBase clickIcon)
    {
        //打开指令集页面
        if (clickIcon is AirIconCell)
        {
           var itemEquip =MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, clickIcon.belongToId));
           if (!string.Equals(MyDataInfo.leadId, itemEquip.BeLongToCommanderId)) return;
           EventManager.Instance.EventTrigger(EventType.ChooseEquip.ToString(), clickIcon.belongToId);
           RightClickShowInfo info = new RightClickShowInfo()
           {
               PointPos = clickIcon.GetComponent<RectTransform>().anchoredPosition,ShowSkillDatas = itemEquip.mySkills, OnTriggerCallBack = itemEquip.OnSelectSkill
           };
           UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
        }
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

public struct RightClickShowInfo
{
    public Vector2 PointPos;
    public List<SkillData> ShowSkillDatas;
    public UnityAction<SkillType> OnTriggerCallBack;
}