using System.Collections.Generic;

namespace ToolsLibrary.EffectivenessEvaluation
{
    //某架飞机数据结构
    public class RecordedData
    {
        public List<SingleSortieData> eachSortieData;

        //第一次从机场起飞时刻
        public float takeOffTime;

        //点击结束任务时刻
        public float endTaskTime;

        //记录行驶的总路程
        public float allDistanceTravelled;
    }

    //每架次数据（每次起飞则创建一组新数据）
    public class SingleSortieData
    {
        //起飞时刻/物资点起飞时刻
        public float takeOffTime;

        //返航时刻
        public float returnFlightTime;

        //降落时刻
        public float landingTime;

        //第一次取水时刻/第一次取物资时刻
        public float firstLoadingGoodsTime;

        //第一次投水时刻/第一次投放物资时刻
        public float firstOperationTime;

        //最后一次投水时刻/最后一次投物资时刻
        public float lastOperationTime;

        //第一次转运人员时间
        public float firstRescuePersonTime;

        //最后一次转运人员时间
        public float lastRescuePersonTime;

        //安置人员时刻
        public float placementOfPersonTime;

        //投水总重/投物资总重
        public float totalWeight;

        //救人数量
        public int numberOfRescues;

        //一个救援或物资任务中走过的直线距离
        public float waterDistance;
        public float goodsDistance;
        public float personDistance;
    }
}