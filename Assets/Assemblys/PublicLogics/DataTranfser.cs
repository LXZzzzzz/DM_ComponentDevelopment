using Enums;

namespace DataTranfsers
{
    using System.Collections.Generic;

    public class ChangeController
    {
        public int objType;
        public string ChangeTargetId;
        public List<string> currentComs;
    }


    public class ShowInfoClass
    {
        public ShowZyDataType szdt;
        public string dataStr;
    }

    //值班领导灾情确认信息
    public class ZbldZqqr
    {
        public string titleInfo;
        public string typeStr;
        public string scaleStr;
        public string hcmjStr;
        public string wztfzlStr;
        public string dzyrsStr;
    }

    public class ZbldZbqr
    {
        public List<zbcellInfo> equipInfo;
    }

    public class zbcellInfo
    {
        public string eId;
        public bool isUse;
        public int chooseJz;
        public int chooseBzz;
    }

    public class ZbldRwystj
    {
        public string qxtj;
        public int szNum;
        public int yyNum;
        public int bjNum;
        public int qsdNum;
        public int azdNum;
        public int qjdNum;
        public int hcNum;
    }

    public class XczhRwqzb
    {
        public List<zbcellInfo2> zbRwqInfo;
    }

    public class zbcellInfo2
    {
        public string id; //直升机ID
        public string jx; //机型
        public string zyl; //载油量
        public string zzl; //载重量
        public string whsj; //地面维护时间
    }

    /// <summary>
    /// 分数统计结构
    /// </summary>
    public class ScoreStatistics
    {
        //一级指挥分数
        public int qrzqxx;
        public int cdzbxxqr;
        public int cdryxxqr;
        public int zchxsb;
        public int xdrw;
        public float firstZhyTotalScore;

        //二级指挥分数
        public int lsrw;
        public int qrzbztxx;
        public int fprwbxdrw;
        public int qrtqczbg;
        public float secondZhyTotalScore;

        public List<SecondZhyScore> thirdZhyScores;
    }

    public class SecondZhyScore
    {
        public string roleId;
        public int qrzyl;
        public int rwqyhxgh;
        public int xxczhybg;
        public float jzZhyTotalScore;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
}