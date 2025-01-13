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

    //值班领导灾情确认信息
    public class ZbldZqqr
    {
        public ShowZyDataType szdt;
        public string titleInfo;
        public string typeStr;
        public string scaleStr;
    }
}