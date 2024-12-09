using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.ProgrammePart
{
    public class ProgrammeData
    {
        public string programmeName;

        //场景所有装备数据
        public List<AEquipData> AllEquipDatas;

        public List<AZiYuanData> AllZiYuanDatas;
    }

    //
    public class AEquipData
    {
        public string myId;

        //是否出动
        public int isSetOut;

        //机组信息
        public int jiZuInfo;
    }

    public class AZiYuanData
    {
        public string myId;

        //当前能想到的资源是 补给点油量和物资点物资量，所以都用这个记录
        //后期数据有扩展的话，这里改成VBase数据结构
        public float zyNum;
    }

    public struct JsonVector3
    {
        public float x, y, z;
    }

    //前指所控数据结构
    public class QianZhiData
    {
        public string equipId;

        //可用资源列表
        public List<string> useZyList;

        //可用灾区列表
        public List<string> useTaskList;
    }
}