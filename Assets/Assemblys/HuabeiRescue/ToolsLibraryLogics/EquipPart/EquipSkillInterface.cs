using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.EquipPart_Logic
{
    public interface IWaterIntaking
    {
        //取水参数回传
        void WaterIntaking(Vector3 pos, float range, float amount, bool isExecuteImmediately);
        //取水前检查最大取水量
        float CheckCapacity();
        
    }

    public class SkillData
    {
        public SkillType SkillType;
        public string skillName;
    }

    public enum SkillType
    {
        WaterIntaking
    }
}