using System.Collections.Generic;
using ReportGenerate;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    public ComanderData cdata;
    public string misName;
    public string misDescription;

    private void GenerateFireExtinguishingReport()
    {
        PDFReport report = new PDFReport();
        var playerInfo = MyDataInfo.playerInfos.Find(a => a.RoleId == MyDataInfo.leadId);
        string mName = playerInfo.PlayerName, mId = playerInfo.RoleId, mAbstract = misDescription;
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[29]).CompareTo(float.Parse(b.AttributeInfos[29])));
        float minRyxhl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[0]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[28]).CompareTo(float.Parse(b.AttributeInfos[28])));
        float minDj = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[0]);
        ResultFireWaterSystemData rfsystem = new ResultFireWaterSystemData
        {
            单位燃烧面积投水需求 = cdata.dwrsmjtsxq,
            最大巡航速度 = cdata.zdxhsd,
            单次取水和投水时间 = cdata.dcqshtssj,
            吊桶单次最大装载量 = cdata.dtdczdzzl,
            直升机每飞行小时耗油率 = cdata.zsjmfxxshyl,
            最低小时燃油消耗率 = minRyxhl,
            最低直升机单价 = minDj
        };
        float tszl = 0;
        float kstssk = float.MaxValue;
        float rwjssk = float.MinValue;
        float zhc = 0;
        int zjc = 0;
        float ghzmj = 0, rszmj = 0, csrszmj = 0, firetszl = 0;
        float minWater2FireDistance = float.MaxValue;
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            for (int j = 0; j < MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData.Count; j++)
            {
                zjc++;
                tszl += MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData[j].totalWeight;
                if (MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData[j].firstOperationTime < kstssk)
                    kstssk = MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData[j].firstOperationTime;
            }

            if (MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime > rwjssk)
                rwjssk = MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime;
            zhc += MyDataInfo.sceneAllEquips[i].GetRecordedData().allDistanceTravelled;
        }

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is ISourceOfAFire)
            {
                (sceneAllzy[i] as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csrsmj, out float atszl);
                ghzmj += ghmj;
                rszmj += rsmj;
                csrszmj += csrsmj;
                firetszl += atszl;
                for (int j = 0; j < sceneAllzy.Count; j++)
                {
                    if (sceneAllzy[j].ZiYuanType == ZiYuanType.Waters)
                    {
                        if (Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position) < minWater2FireDistance)
                        {
                            minWater2FireDistance = Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position);
                        }
                    }
                }
            }
        }

        sender.LogError("任务结束时过火总面积" + ghzmj);
        ResultFireWaterOutData rfout = new ResultFireWaterOutData
        {
            任务结束时投水总量 = tszl,
            任务结束时过火总面积 = ghzmj,
            任务结束时燃烧面积 = rszmj,
            任务初始燃烧面积 = csrszmj,
            开始投水时刻 = kstssk >= float.MaxValue - 1 ? 0 : kstssk, //增大检测范围，防止浮点误差
            取水点到投水点的最短路径 = minWater2FireDistance,
            任务结束时刻 = rwjssk < 0 ? 0 : rwjssk,
            总航程 = zhc,
            直升机总架次 = zjc,
            火场数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.SourceOfAFire).Count,
            取水点数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.Waters).Count,
            任务结束时火场投水总重量 = firetszl
        };
        rfout.任务结束时各火场数据 = new List<FireData>();
        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is ISourceOfAFire)
            {
                (sceneAllzy[i] as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csrsmj, out float atszl);
                FireData itemFireData = new FireData()
                {
                    Id = sceneAllzy[i].BobjectId, Name = sceneAllzy[i].ziYuanName, burnArea = rsmj, burnedArea = ghmj, initBurnArea = csrsmj, WaterWeight = atszl
                };
                rfout.任务结束时各火场数据.Add(itemFireData);
            }
        }

        Dictionary<string, List<WaterMegData>> heliWaterMegList = new Dictionary<string, List<WaterMegData>>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            HeliData hd1 = new HeliData
            {
                Id = MyDataInfo.sceneAllEquips[i].BObjectId,
                Name = MyDataInfo.sceneAllEquips[i].name,
                Price = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[28]),
                Consumption = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[29]),
                Speed = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[9])
            };
            List<HeliSortieData> hsd1List = new List<HeliSortieData>();
            List<WaterMegData> nwmdList = new List<WaterMegData>();
            var itemDatas = MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData;
            for (int j = 0; j < itemDatas.Count; j++)
            {
                HeliSortieData hsd1 = new HeliSortieData
                {
                    TakeOffTime = itemDatas[j].takeOffTime,
                    ReturnTime = itemDatas[j].returnFlightTime < itemDatas[j].takeOffTime ? itemDatas[j].takeOffTime + 1 : itemDatas[j].returnFlightTime,
                    EndMissonTime = itemDatas[j].landingTime,
                    WaterTime = itemDatas[j].firstLoadingGoodsTime,
                    WaterFireTime = itemDatas[j].lastOperationTime,
                    WaterZongWeight = itemDatas[j].totalWeight,
                    FirstWaterFireTime = itemDatas[j].firstOperationTime,
                    MaterialTime = itemDatas[j].firstLoadingGoodsTime,
                    MaterialWeight = itemDatas[j].totalWeight,
                    MaterialPointTime = itemDatas[j].lastOperationTime,
                    PersonCount = itemDatas[j].numberOfRescues,
                    PersonFirstTime = itemDatas[j].firstLoadingGoodsTime, PersonEndTime = itemDatas[j].lastOperationTime
                };
                Debug.LogError($"{hd1.Name}返航时间{hsd1.ReturnTime}起飞时间{hsd1.TakeOffTime}");
                hsd1List.Add(hsd1);
                WaterMegData mpmd = new WaterMegData
                {
                    sortieIndex = j + 1,
                    StartWaterTime = ConvertSecondsToHHMMSS(itemDatas[j].firstOperationTime),
                    EndWaterTime = ConvertSecondsToHHMMSS(itemDatas[j].lastOperationTime),
                    WaterWeight = itemDatas[j].totalWeight
                };
                nwmdList.Add(mpmd);
            }

            rfout.HeliSortieWaterDataList.Add(hd1, hsd1List);
            heliWaterMegList.Add(hd1.Name, nwmdList);
        }

        EvalManage em = new EvalManage();
        ResultFireWaterData rfwd = em.EvalWaterCompute(rfout, rfsystem);

        report.CreateWaterMissionReport(MyDataInfo.leadId, misName + "-效能评估报告", mName, mId, mAbstract, rfwd, rfout, showAllOperatorInfos, heliWaterMegList);
    }

    private void GenerateRescueReport()
    {
        PDFReport report = new PDFReport();
        var playerInfo = MyDataInfo.playerInfos.Find(a => a.RoleId == MyDataInfo.leadId);
        string mName = playerInfo.PlayerName, mId = playerInfo.RoleId, mAbstract = misDescription;
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[29]).CompareTo(float.Parse(b.AttributeInfos[29])));
        float minRyxhl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[0]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[28]).CompareTo(float.Parse(b.AttributeInfos[28])));
        float minDj = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[0]);

        ResultRescueSystemData rfsystem = new ResultRescueSystemData
        {
            人均救援物资需求 = cdata.dwrsmjtsxq,
            受灾需转运总人数 = cdata.zqxzyzrs,
            最大巡航速度 = cdata.zdxhsd,
            单次人员吊救时间 = cdata.dcrydjsj,
            单次物资投放时间 = cdata.dcwztfsj,
            直升机单次最大运载人数 = cdata.dczdyzrs,
            直升机单次最大运载物资重量 = cdata.dtdczdzzl,
            直升机每飞行小时耗油率 = cdata.zsjmfxxshyl,
            最低小时燃油消耗率 = minRyxhl,
            最低直升机单价 = minDj
        };
        float twzzl = 0;
        float rwjssk = float.MinValue;
        float zhc = 0;
        int zjc = 0;
        float firstTimea = float.MaxValue, totalWeighta = 0;
        int totalPersona = 0;
        float minDisasterArea2RescueStationDis = float.MaxValue;
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            for (int j = 0; j < MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData.Count; j++)
            {
                zjc++;
                twzzl += MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData[j].totalWeight;
            }

            if (MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime > rwjssk)
                rwjssk = MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime;
            zhc += MyDataInfo.sceneAllEquips[i].GetRecordedData().allDistanceTravelled;
        }

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is IRescueStation)
            {
                (sceneAllzy[i] as IRescueStation).getResData(out float firstTime, out float totalWeight, out int totalPerson);
                if (firstTime < firstTimea) firstTimea = firstTime;
                totalWeighta += totalWeight;
                totalPersona += totalPerson;
                for (int j = 0; j < sceneAllzy.Count; j++)
                {
                    if (sceneAllzy[j].ZiYuanType == ZiYuanType.DisasterArea)
                    {
                        if (Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position) < minDisasterArea2RescueStationDis)
                        {
                            minDisasterArea2RescueStationDis = Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position);
                        }
                    }
                }
            }
        }

        ResultMaterialPersonOutData cfout = new ResultMaterialPersonOutData
        {
            任务结束时救援物资投放总重量 = twzzl,
            任务结束时各救援安置点物资投放重量 = totalWeighta,
            首批救援物资到达安置点时刻 = firstTimea,
            物资装载起降点到安置点的最短路径 = minDisasterArea2RescueStationDis,
            任务结束时刻 = rwjssk < 0 ? 0 : rwjssk,
            总航程 = zhc,
            所有飞机总架次 = zjc,
            受灾地点数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.DisasterArea).Count,
            临时安置点数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.RescueStation).Count,
            任务结束时转运总人数 = totalPersona,
            救援点到安置点的最短路径 = minDisasterArea2RescueStationDis
        };
        cfout.任务结束时各安置点数据 = new List<MaterialData>();

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is IRescueStation)
            {
                (sceneAllzy[i] as IRescueStation).getResData(out float firstTime, out float totalWeight, out int totalPerson);
                MaterialData fd1 = new MaterialData
                {
                    Id = sceneAllzy[i].BobjectId,
                    Name = sceneAllzy[i].ziYuanName,
                    MaterialWeight = totalWeight,
                    PersonCount = totalPerson,
                };
                cfout.任务结束时各安置点数据.Add(fd1);
            }
        }

        Dictionary<string, List<MaterialPersonMegData>> heliMegList = new Dictionary<string, List<MaterialPersonMegData>>();
        cfout.HeliSortieMaterialPersonDataList = new Dictionary<HeliData, List<HeliSortieData>>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            HeliData hd1 = new HeliData
            {
                Id = MyDataInfo.sceneAllEquips[i].BObjectId,
                Name = MyDataInfo.sceneAllEquips[i].name,
                Price = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[28]),
                Consumption = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[29]),
                Speed = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[9])
            };
            List<HeliSortieData> hsd1List = new List<HeliSortieData>();
            List<MaterialPersonMegData> nwmdList = new List<MaterialPersonMegData>();
            var itemDatas = MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData;
            for (int j = 0; j < itemDatas.Count; j++)
            {
                HeliSortieData hsd1 = new HeliSortieData
                {
                    TakeOffTime = itemDatas[j].takeOffTime,
                    ReturnTime = itemDatas[j].returnFlightTime < itemDatas[j].takeOffTime ? itemDatas[j].takeOffTime + 1 : itemDatas[j].returnFlightTime,
                    EndMissonTime = MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime,
                    WaterTime = itemDatas[j].firstLoadingGoodsTime,
                    WaterFireTime = itemDatas[j].lastOperationTime,
                    WaterZongWeight = itemDatas[j].totalWeight,
                    FirstWaterFireTime = itemDatas[j].firstOperationTime,
                    MaterialTime = itemDatas[j].firstLoadingGoodsTime,
                    MaterialWeight = itemDatas[j].totalWeight,
                    MaterialPointTime = itemDatas[j].lastOperationTime,
                    PersonCount = itemDatas[j].numberOfRescues,
                    PersonFirstTime = itemDatas[j].firstLoadingGoodsTime, PersonEndTime = itemDatas[j].lastOperationTime
                };
                Debug.LogError($"{hd1.Name}返航时间{hsd1.ReturnTime}起飞时间{hsd1.TakeOffTime}");
                hsd1List.Add(hsd1);
                MaterialPersonMegData mpmd = new MaterialPersonMegData
                {
                    sortieIndex = j + 1,
                    TakeOffTime = ConvertSecondsToHHMMSS(itemDatas[j].takeOffTime),
                    EndMissionTime = ConvertSecondsToHHMMSS(MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime),
                    MaterialTime = ConvertSecondsToHHMMSS(itemDatas[j].lastOperationTime),
                    MaterialWeight = itemDatas[j].totalWeight,
                    PersonCount = itemDatas[j].numberOfRescues
                };
                nwmdList.Add(mpmd);
            }

            cfout.HeliSortieMaterialPersonDataList.Add(hd1, hsd1List);
            heliMegList.Add(hd1.Name, nwmdList);
        }

        EvalManage em = new EvalManage();
        ResultMaterialPersonData rfwd = em.EvalMaterialCompute(cfout, rfsystem);

        report.CreateRescueMissionReport(MyDataInfo.leadId, misName + "-效能评估报告", mName, mId, mAbstract, rfwd, cfout, rfsystem, showAllOperatorInfos, heliMegList);
    }
}

public class ComanderData
{
    public float dwrsmjtsxq;
    public int zqxzyzrs;
    public float zdxhsd;
    public float dcqshtssj;
    public float dcwzzzsj;
    public float dcwztfsj;
    public float dcrydjsj;
    public int dczdyzrs;
    public float dtdczdzzl;
    public float zsjmfxxshyl;
}