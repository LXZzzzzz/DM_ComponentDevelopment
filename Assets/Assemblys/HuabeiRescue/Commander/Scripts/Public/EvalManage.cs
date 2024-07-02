using System.Collections.Generic;
using UnityEngine;

namespace ReportGenerate
{
    public class EvalManage
    {
        /// <summary>
        /// 灭火效能计算
        /// </summary>
        public ResultFireWaterData EvalWaterCompute(ResultFireWaterOutData outdata, ResultFireWaterSystemData sysData) 
        {
            #region 灭火任务完成度
            ResultFireWaterData data = new ResultFireWaterData();
            data.任务结束时过火面积对应的投水总需求 = outdata.任务结束时过火总面积 * sysData.单位燃烧面积投水需求;
            double valAwZong = 0f;
            foreach (FireData item in outdata.任务结束时各火场数据)
            {
                double valWaterNeed = item.burnedArea * sysData.单位燃烧面积投水需求;
                if (item.WaterWeight >= valWaterNeed)
                {
                    item.Degree = 1;
                }
                else
                {
                    item.Degree = item.WaterWeight / valWaterNeed;
                }
                data.任务结束时各火场数据.Add(item);

                valAwZong += item.Degree * valWaterNeed;
            }
            data.灭火任务完成度 = valAwZong / data.任务结束时过火面积对应的投水总需求;
            data.灭火任务效率 = data.灭火任务完成度;
            #endregion

            #region 灭火任务总时间
            double ZongEndMissionTime = 0;  //灭火任务的总时间
            double ZongTotalCost = 0;  //灭火任务的总成本
            double ZongWaterWeightVal = 0;  //单机任务效率
            foreach (KeyValuePair<HeliData, List<HeliSortieData>> item in outdata.HeliSortieWaterDataList)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    double ztime = item.Value[i].ReturnTime - item.Value[i].FirstWaterFireTime;
                    item.Value[i].ZongWaterMissionTime = ztime;
                    ZongEndMissionTime += ztime;
                    ZongTotalCost += (item.Key.Price * 0.00029453 + item.Key.Consumption * 0.00087035 + 0.965) * ztime;

                    double MaxWaterWeight = (ztime / ((outdata.取水点到投水点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次取水和投水时间)) * sysData.吊桶单次最大装载量;
                    item.Key.单机投水重量效率.Add(item.Value[i].WaterZongWeight / MaxWaterWeight);

                    double WaterVal = item.Value[i].WaterZongWeight / MaxWaterWeight;
                    if (WaterVal > 0)
                        ZongWaterWeightVal *= WaterVal;

                    item.Key.单机投水总重量 += item.Value[i].WaterZongWeight;
                    item.Key.单机任务成本 += (item.Key.Price * 0.00029453 + item.Key.Consumption * 0.00087035 + 0.965) * ztime;
                }
                item.Key.单位时间单机投水重量 = item.Key.单机投水总重量 / ZongEndMissionTime;
                data.机型架次数据.Add(item.Key, item.Value);
            }
            data.灭火任务总时间 = ZongEndMissionTime;
            data.任务结束过火总面积最小灭火时间 = ((outdata.取水点到投水点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次取水和投水时间) * data.任务结束时过火面积对应的投水总需求 / sysData.吊桶单次最大装载量;

            if (data.灭火任务完成度 == 1)
            {
                data.灭火任务总时间效率 = data.任务结束过火总面积最小灭火时间 / data.灭火任务总时间;
            }
            else
            {
                data.灭火任务总时间效率 = (1 - outdata.任务结束时燃烧面积 / outdata.任务结束时过火总面积) * 0.5;
            }
            #endregion

            #region 灭火任务总成本
            data.灭火任务总成本 = ZongTotalCost;
            data.成本基数 = (sysData.最低直升机单价 * 0.00029453 + sysData.最低小时燃油消耗率 * 0.00087035 + 0.965) * data.任务结束过火总面积最小灭火时间;
            data.任务总成本效率 = data.成本基数 / data.灭火任务总成本;
            #endregion

            #region 灭火任务效率计算
            data.过火面积控制率 = outdata.任务结束时过火总面积 / outdata.任务初始燃烧面积;
            data.总体任务效率 = 0.5 * data.灭火任务效率 + 0.3 * data.灭火任务总时间效率 + 0.2 * data.任务总成本效率;
            data.单机任务效率 = ZongWaterWeightVal;
            data.协同指挥效能 = 100 * (0.5 * data.总体任务效率 + 0.5 * data.单机任务效率);
            #endregion

            return data;
        }

        /// <summary>
        /// 物资人员效能计算
        /// </summary>
        public ResultMaterialPersonData EvalMaterialCompute(ResultMaterialPersonOutData outData, ResultRescueSystemData sysData)
        {
            ResultMaterialPersonData data = new ResultMaterialPersonData();
            #region 物资投放完成度
            data.任务结束时对应的物资投放总需求 = sysData.受灾需转运总人数 * sysData.人均救援物资需求;
            double valAwZong = 0f;
            foreach (MaterialData item in outData.任务结束时各安置点数据)
            {
                item.PersonMaterialNeed = item.PersonCount * sysData.人均救援物资需求;
                if (item.MaterialWeight >= item.PersonMaterialNeed)
                {
                    item.MaterialDegree = 1;
                }
                else
                {
                    item.MaterialDegree = item.MaterialWeight / item.PersonMaterialNeed;
                }
                data.任务结束时各安置点数据.Add(item);


                valAwZong += item.MaterialDegree * item.PersonMaterialNeed;
            }
            data.物资任务完成度 = valAwZong / data.任务结束时对应的物资投放总需求;
            data.物资任务效率 = data.物资任务完成度;
            #endregion

            #region  人员转运完成度
            data.任务结束待转运人数 = sysData.受灾需转运总人数 - outData.任务结束时转运总人数;
            data.人员转运任务完成度 = outData.任务结束时转运总人数 / sysData.受灾需转运总人数;
            data.人员转运效率 = data.人员转运任务完成度;
            #endregion

            #region 救援任务总时间
            double ZongPersonMissionTime = 0;  //人员转运任务的总时间
            double ZongMaterialMissionTime = 0;  //物资投送任务的总时间

            double ZongPersonTotalCost = 0;  //人员任务的总成本
            double ZongMaterialTotalCost = 0;  //物资任务的总成本
            double ZongPersonWeightVal = 0;  //人员单机任务效率
            double ZongMaterialWeightVal = 0;  //物资单机任务效率
            foreach (KeyValuePair<HeliData, List<HeliSortieData>> item in outData.HeliSortieMaterialPersonDataList)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    double zPtime = item.Value[i].PersonEndTime - item.Value[i].PersonFirstTime;
                    item.Value[i].ZongPersonMissionTime = zPtime;
                    ZongPersonMissionTime += zPtime;

                    double zMtime = item.Value[i].MaterialPointTime - item.Value[i].TakeOffTime;
                    item.Value[i].ZongMaterialMissionTime = zMtime;
                    ZongMaterialMissionTime += zMtime;

                    ZongPersonTotalCost += (item.Key.Price * 0.00029453 + item.Key.Consumption * 0.00087035 + 0.965) * zPtime;
                    ZongMaterialTotalCost += (item.Key.Price * 0.00029453 + item.Key.Consumption * 0.00087035 + 0.965) * zMtime;

                    double MaxPersonCount = (zPtime / ((outData.救援点到安置点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次人员吊救时间)) * sysData.直升机单次最大运载人数;
                    item.Key.单机人员转运效率.Add(item.Value[i].PersonCount / MaxPersonCount);
                    double MaxMaterialWeight = (zMtime / ((outData.物资装载起降点到安置点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次物资投放时间)) * sysData.人均救援物资需求;
                    item.Key.单机物资投放效率.Add(item.Value[i].MaterialWeight / MaxMaterialWeight);

                    double PersonVal = item.Value[i].PersonCount / MaxPersonCount;
                    if (PersonVal > 0)
                        ZongPersonWeightVal *= PersonVal;

                    double MaterialVal = item.Value[i].MaterialWeight / MaxMaterialWeight;
                    if (MaterialVal > 0)
                        ZongMaterialWeightVal *= MaterialVal;

                    item.Key.累计转运人数 += item.Value[i].PersonCount;
                    item.Key.累计投放物资重量 += item.Value[i].MaterialWeight;
                }
                item.Key.单机任务成本 += (item.Key.Price * 0.00029453 + item.Key.Consumption * 0.00087035 + 0.965) * (ZongPersonMissionTime + ZongMaterialMissionTime);

                data.机型架次数据.Add(item.Key, item.Value);
            }
            data.人员转运任务总时间 = ZongPersonMissionTime;
            data.物资投放任务总时间 = ZongMaterialMissionTime;

            data.人员转运最短时间 = ((outData.救援点到安置点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次人员吊救时间) * sysData.受灾需转运总人数 / sysData.直升机单次最大运载人数;
            data.物资投放最短时间= ((outData.物资装载起降点到安置点的最短路径 * 2) / sysData.最大巡航速度 + sysData.单次物资投放时间) * data.任务结束时对应的物资投放总需求 / sysData.人均救援物资需求;

            if (data.人员转运任务完成度 == 1)
            {
                data.人员转运任务时间效率 = data.人员转运最短时间 / data.人员转运任务总时间;
            }
            else if(data.人员转运任务完成度 < 1)
            {
                data.人员转运任务时间效率 = (1 - data.任务结束待转运人数 / sysData.受灾需转运总人数) * 0.5;
            }

            if (data.物资任务完成度 == 1)
            {
                data.物资投放任务时间效率 = data.物资投放最短时间 / data.物资投放任务总时间;
            }
            else if (data.物资任务完成度 < 1)
            {
                data.物资投放任务时间效率 = data.物资任务完成度 * 0.5;
            }
            #endregion

            #region 救援任务总成本
            data.人员转运任务总成本 = ZongPersonTotalCost;
            data.物资投放任务总成本 = ZongMaterialTotalCost;

            data.人员转运任务成本基数 = (sysData.最低直升机单价 * 0.00029453 + sysData.最低小时燃油消耗率 * 0.00087035 + 0.965)* data.人员转运最短时间;
            data.物资投放任务运成本基数 = (sysData.最低直升机单价 * 0.00029453 + sysData.最低小时燃油消耗率 * 0.00087035 + 0.965) * data.物资投放最短时间;
            data.人员转运任务总成本效率 = data.人员转运任务成本基数 / data.人员转运任务总成本;
            data.物资投放任务总成本效率 = data.物资投放任务运成本基数 / data.物资投放任务总成本;
            #endregion

            #region 救援任务效率计算
            data.人员转运总体任务效率 = 0.5 * data.人员转运效率 + 0.3 * data.人员转运任务时间效率 + 0.2 * data.人员转运任务总成本;
            data.物资投放总体任务效率 = 0.5 * data.物资任务效率 + 0.3 * data.物资投放任务时间效率 + 0.2 * data.物资投放任务总成本;
            data.人员转运单机任务效率 = ZongPersonWeightVal;
            data.物资投放单机任务效率 = ZongMaterialWeightVal;
            data.人员转运任务协同指挥效能 = 100 * (0.5 * data.人员转运总体任务效率 + 0.5 * data.人员转运单机任务效率);
            data.物资投放任务协同指挥效能 = 100 * (0.5 * data.物资投放总体任务效率 + 0.5 * data.物资投放单机任务效率);
            data.协同指挥效能 = 0.6 * data.人员转运任务协同指挥效能 + 0.4 * data.物资投放任务协同指挥效能;
            #endregion

            return data;
        }
    }
}
