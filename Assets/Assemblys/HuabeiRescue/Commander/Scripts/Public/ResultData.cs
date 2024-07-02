using System.Collections.Generic;

namespace ReportGenerate
{
    #region 通用
    public class WaterMegData 
    {
        public int sortieIndex; //架次
        public string StartWaterTime;
        public string EndWaterTime;
        public double WaterWeight;
    }

    public class MaterialPersonMegData
    {
        public int sortieIndex; //架次
        public string TakeOffTime;
        public string EndMissionTime;
        public string MaterialTime;
        public double MaterialWeight;
        public double PersonCount;
    }

    /// <summary>
    /// 机型架次数据
    /// </summary>
    public class HeliSortieData
    {
        /// <summary>
        /// 物资装载点的起飞时刻//todo:(物资改)
        /// </summary>
        public double TakeOffTime;

        /// <summary>
        /// 返航时刻(指令得到)
        /// </summary>
        public double ReturnTime;

        /// <summary>
        /// 任务结束时刻（着陆时刻）
        /// </summary>
        public double EndMissonTime;

        /// <summary>
        /// 取水时刻
        /// </summary>
        public double WaterTime;

        /// <summary>
        /// 投水最后的时刻
        /// </summary>
        public double WaterFireTime;

        /// <summary>
        /// 投水总重量
        /// </summary>
        public double WaterZongWeight;

        /// <summary>
        /// 初次投水时间
        /// </summary>
        public double FirstWaterFireTime;

        /// <summary>
        /// 灭火任务总时间
        /// </summary>
        public double ZongWaterMissionTime;

        /// <summary>
        /// 获取物资时刻
        /// </summary>
        public double MaterialTime;

        /// <summary>
        /// 物资投送总重量
        /// </summary>
        public double MaterialWeight;

        /// <summary>
        /// 投送物资最后的时刻
        /// </summary>
        public double MaterialPointTime;

        /// <summary>
        /// 转运人员数量
        /// </summary>
        public double PersonCount;

        /// <summary>
        /// 转运人员首次的时刻 //todo:第一次转运人员时间
        /// </summary>
        public double PersonFirstTime;

        /// <summary>
        /// 转运人员最后的时刻 //todo:最后一次转运人员时间
        /// </summary>
        public double PersonEndTime;

        /// <summary>
        /// 人员转运任务总时间
        /// </summary>
        public double ZongPersonMissionTime;

        /// <summary>
        /// 物资投送任务总时间
        /// </summary>
        public double ZongMaterialMissionTime;
    }

    public class FireData
    {
        public string Id;
        public string Name;

        /// <summary>
        /// 投水重量
        /// </summary>
        public double WaterWeight;

        /// <summary>
        /// 初始燃烧面积
        /// </summary>
        public double initBurnArea;

        /// <summary>
        /// 燃烧面积
        /// </summary>
        public double burnArea;

        /// <summary>
        /// 过火面积
        /// </summary>
        public double burnedArea;

        /// <summary>
        /// 总完成度
        /// </summary>
        public double Degree;
    }

    public class MaterialData
    {
        public string Id;
        public string Name;

        /// <summary>
        /// 投放物资重量
        /// </summary>
        public double MaterialWeight;

        /// <summary>
        /// 转运人员数量
        /// </summary>
        public double PersonCount;

        /// <summary>
        /// 物资投放需求重量
        /// </summary>
        public double PersonMaterialNeed;

        /// <summary>
        /// 物资总完成度
        /// </summary>
        public double MaterialDegree;

        /// <summary>
        /// 人员总完成度
        /// </summary>
        public double PersonDegree;

        /// <summary>
        /// 总完成度
        /// </summary>
        public double ZongDegree;
    }

    /// <summary>
    /// 直升机数据
    /// </summary>
    public class HeliData
    {
        public string Id;
        public string Name;

        /// <summary>
        /// 单价
        /// </summary>
        public double Price;

        /// <summary>
        /// 耗油率
        /// </summary>
        public double Consumption;

        /// <summary>
        /// 巡航速度
        /// </summary>
        public double Speed;


        //以下参数不需要赋值
        public double 单机任务成本;

        //灭火
        public List<double> 单机投水重量效率 = new List<double>();
        public double 单位时间单机投水重量;
        public double 单机投水总重量;

        //物资
        public List<double> 单机物资投放效率 = new List<double>();
        public double 任务结束时各安置点物资投放需求;
        public double 任务结束时各安置点物资投放任务完成度;
        public double 物资投放任务完成度;
        public double 单机投送物资总重量;
        public double 累计投放物资重量;

        //人员
        public double 累计转运人数;
        public List<double> 单机人员转运效率 = new List<double>();
    }

    #endregion

    #region 灭火
    /// <summary>
    /// 系统计算参数（灭火）
    /// </summary>
    public class ResultFireWaterData
    {
        public double 任务结束时过火面积对应的投水总需求;
        public double 灭火任务完成度;
        public double 灭火任务效率;
        public double 过火面积控制率;
        public double 灭火任务总时间;
        public double 任务结束过火总面积最小灭火时间;
        public double 灭火任务总时间效率;
        public double 灭火任务总成本;
        public double 成本基数;
        public double 任务总成本效率;
        public double 单机任务效率;
        public double 总体任务效率;
        public double 协同指挥效能;

        public Dictionary<HeliData, List<HeliSortieData>> 机型架次数据 = new Dictionary<HeliData, List<HeliSortieData>>();
        public List<FireData> 任务结束时各火场数据 = new List<FireData>();
    }

    /// <summary>
    /// 取水灭火（系统参数）
    /// </summary>
    public class ResultFireWaterSystemData
    {
        public double 单位燃烧面积投水需求;
        public double 最大巡航速度;
        public double 单次取水和投水时间;
        public double 吊桶单次最大装载量;
        public double 直升机每飞行小时耗油率;
        public double 最低小时燃油消耗率;
        public double 最低直升机单价;
    }

    /// <summary>
    /// 取水灭火（传入参数）
    /// </summary>
    public class ResultFireWaterOutData
    {
        public double 任务结束时投水总量;
        public double 任务结束时过火总面积;
        public double 任务结束时燃烧面积;
        public double 任务初始燃烧面积;
        public double 开始投水时刻;
        public double 取水点到投水点的最短路径;
        public double 任务结束时刻;
        public double 总航程;
        public int 直升机总架次;
        public int 火场数量;
        public int 取水点数量;
        public double 任务结束时火场投水总重量;

        /// <summary>
        /// 机型架次数据
        /// </summary>
        public Dictionary<HeliData, List<HeliSortieData>> HeliSortieWaterDataList = new Dictionary<HeliData, List<HeliSortieData>>();
        public List<FireData> 任务结束时各火场数据 = new List<FireData>();
    }
    #endregion

    #region 物资和人员
    /// <summary>
    /// 物资投放和转运伤员（系统参数）
    /// </summary>
    public class ResultRescueSystemData
    {
        public double 人均救援物资需求;
        public double 受灾需转运总人数;
        public double 最大巡航速度;
        public double 单次人员吊救时间;
        public double 单次物资投放时间;
        public double 直升机单次最大运载人数;
        public double 直升机单次最大运载物资重量;
        public double 直升机每飞行小时耗油率;
        public double 最低小时燃油消耗率;
        public double 最低直升机单价;
    }

    /// <summary>
    /// 物资投放和转运伤员（传入参数）
    /// </summary>
    public class ResultMaterialPersonOutData
    {
        #region 物资
        public double 任务结束时救援物资投放总重量;
        public double 任务结束时各救援安置点物资投放重量;
        public double 首批救援物资到达安置点时刻;
        public double 物资装载起降点到安置点的最短路径;
        public double 任务结束时刻;
        public double 总航程;
        public int 所有飞机总架次;
        public int 受灾地点数量;
        public int 临时安置点数量;
        #endregion

        #region 人员
        public double 任务结束时转运总人数;
        public double 救援点到安置点的最短路径;
        #endregion

        /// <summary>
        /// 机型架次数据
        /// </summary>
        public Dictionary<HeliData, List<HeliSortieData>> HeliSortieMaterialPersonDataList = new Dictionary<HeliData, List<HeliSortieData>>();
        public List<MaterialData> 任务结束时各安置点数据 = new List<MaterialData>();
    }

    /// <summary>
    /// 系统计算参数（物资投放和转运伤员）
    /// </summary>
    public class ResultMaterialPersonData
    {
        #region 物资
        public double 物资投放总需求;
        public double 任务结束时对应的物资投放总需求;
        public double 物资任务完成度;
        public double 物资任务效率;
        public double 物资投放效率;
        public double 物资投放任务总时间;
        public double 物资投放最短时间;
        public double 物资投放任务时间效率;
        public double 物资投放总体任务效率;
        public double 物资投放单机任务效率;
        public double 物资投放任务协同指挥效能;
        public double 物资投放任务总成本;
        public double 物资投放任务运成本基数;
        public double 物资投放任务总成本效率;
        public double 总体任务效率;
        public double 单机任务效率;
        public double 协同指挥效能;
        #endregion

        #region 人员
        public double 人员转运任务完成度;
        public double 人员转运效率;
        public double 人员转运任务总时间;
        public double 任务结束待转运人数;
        public double 人员转运最短时间;
        public double 人员转运任务时间效率;
        public double 人员转运总体任务效率;
        public double 人员转运单机任务效率;
        public double 人员转运任务协同指挥效能;
        public double 人员转运任务成本基数;
        public double 人员转运任务总成本效率;
        public double 人员转运任务总成本;
        #endregion

        public Dictionary<HeliData, List<HeliSortieData>> 机型架次数据 = new Dictionary<HeliData, List<HeliSortieData>>();
        public List<MaterialData> 任务结束时各安置点数据 = new List<MaterialData>();
    }
    #endregion
}
