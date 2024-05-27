using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public class HelicopterController : EquipBase, IWaterIntaking
{
    public override void Init()
    {
        base.Init();
        //初始化飞机的基本属性
        mySkills.Add(new SkillData() { SkillType = SkillType.WaterIntaking, skillName = "取水" });
    }

    public override void OnSelectSkill(SkillType st)
    {
        if (mySkills.Find(x => x.SkillType == st) == null) return; //技能数据错误
        switch (st)
        {
            case SkillType.WaterIntaking:
                //这里需要打开取水UI界面，并把自己以接口形式传过去
                EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", this);
                break;
        }
    }

    public void WaterIntaking(Vector3 pos, float range, float amount, bool isExecuteImmediately)
    {
        Debug.LogError("取水技能参数回传"+pos+amount);
        if (!isExecuteImmediately)
        {
            //把参数传给主角，将参数传给所有客户端，统一执行
            EventManager.Instance.EventTrigger(EventType.SendWaterInfoToControler.ToString(), MsgSend_Water(BObjectId, pos, amount));
            return;
        }
        //判断自己是否处于取水地的范围内，不在的话调move机动到目的地，然后，执行取水逻辑
        if (Vector3.Distance(transform.position, pos) > range)
        {
            MoveToTarget(pos);
        }
    }

    public float CheckCapacity()
    {
        Debug.LogError("检查数量");
        return 10;
    }
    private string MsgSend_Water(string id,Vector3 pos, float amount)
    {
        return string.Format($"{pos.x}_{pos.y}_{pos.z}_{amount}_{id}");
    }
}