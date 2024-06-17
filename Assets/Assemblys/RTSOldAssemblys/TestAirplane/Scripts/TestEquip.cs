using ToolsLibrary.EquipPart;
using UnityEngine;

public class TestEquip : EquipBase
{
    public void Init(string myid)
    {
        BObjectId = myid;
        Debug.LogError("测试组件装备正常挂载继承Mono的脚本"+myid);
    }

    public override void OnSelectSkill(SkillType st)
    {
        throw new System.NotImplementedException();
    }

    protected override void OnClose()
    {
    }
}