using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.EquipPart_Logic
{
    public class EquipSkillInterface
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

    public interface IWaterIntaking
    {
        //执行取水逻辑
        void WaterIntaking(Vector3 pos,float range, float amount);
        //取水前检查最大取水量
        float CheckCapacity();
    }

}