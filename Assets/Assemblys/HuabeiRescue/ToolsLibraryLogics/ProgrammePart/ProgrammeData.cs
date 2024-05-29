using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.ProgrammePart_Logic
{
    public class ProgrammeData
    {
        public string programmeName;

        //场景所有装备数据
        public List<AEquipData> AllEquipDatas;

        //每个指挥端所控算子列表
        public Dictionary<string, List<string>> CommanderControlList;
        
        //每个资源组件都可被哪些指挥端所用
        public Dictionary<string, List<string>> ZiYuanControlledList;
        
        //每个任务都可被哪些指挥端所见
        public Dictionary<string, List<string>> TaskControlledList;
    }

//
    public class AEquipData
    {
        //模板ID
        public string templateId;

        //机场ID
        public string airportId;

        //控制者ID
        public string controllerId;

        public string myId;

        //位置信息，测试阶段可用，实际要依附于机场
        public JsonVector3 pos;
    }

    public struct JsonVector3
    {
        public float x, y, z;
    }
}