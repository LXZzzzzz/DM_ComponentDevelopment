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
}