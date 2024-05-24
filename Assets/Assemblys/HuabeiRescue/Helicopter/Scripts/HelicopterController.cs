using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart_Logic;
using UnityEngine;

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

                break;
        }
    }

    public void WaterIntaking(Vector3 pos, float range, float amount)
    {
        //判断自己是否处于取水地的范围内，不在的话调move机动到目的地，然后，执行取水逻辑
        if (Vector3.Distance(transform.position, pos) > range)
        {
            MoveToTarget(pos);
        }
    }

    public float CheckCapacity()
    {
        return 10;
    }
}