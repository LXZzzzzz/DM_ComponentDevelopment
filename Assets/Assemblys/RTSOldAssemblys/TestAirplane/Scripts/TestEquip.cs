using System.Collections.Generic;
using ToolsLibrary.EffectivenessEvaluation;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class TestEquip : EquipBase
{
    public void Init(string myid)
    {
        BObjectId = myid;
        Debug.LogError("测试组件装备正常挂载继承Mono的脚本"+myid);
    }

    public override List<SkillData> GetSkillsData()
    {
        throw new System.NotImplementedException();
    }

    public override RecordedData GetRecordedData()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelectSkill(SkillType st)
    {
        throw new System.NotImplementedException();
    }

    public override bool OnCheckIsMove()
    {
        throw new System.NotImplementedException();
    }

    public override void OnNullCommand(int type)
    {
        throw new System.NotImplementedException();
    }

    public override void GetCurrentAllMass(out float currentOil, out float totalOil, out float water, out float goods, out float person, out int personType)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClose()
    {
    }
}