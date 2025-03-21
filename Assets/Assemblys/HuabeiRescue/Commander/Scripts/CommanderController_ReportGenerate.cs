using System;
using System.Collections.Generic;
using DataTranfsers;
using Enums;
using Newtonsoft.Json;
using ReportGenerate;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public partial class CommanderController
{
    public ComanderData cdata;
    public string misName;
    public string misDescription;
    private Dictionary<string, List<string>> playerEquips, playerZiyuans;
    private List<string> reportPlayers = new List<string>();
    private PersonAssessment_TxtData _personAssessmentTxtData;
    private string tianqiInfoStr;
    private Dictionary<string, string> guzhangInfoStr = new Dictionary<string, string>();
    private ScoreStatistics scoreData;

    public void OnGetPdfData_ShowZyDataType(string param)
    {
        string dataStr = AESUtils.Decrypt(param);
        if (_personAssessmentTxtData == null) _personAssessmentTxtData = new PersonAssessment_TxtData();
        var data = JsonConvert.DeserializeObject<ShowInfoClass>(dataStr);
        switch (data.szdt)
        {
            case ShowZyDataType.zqxxShow:
                var datazq = JsonConvert.DeserializeObject<ZbldZqqr>(data.dataStr);
                _personAssessmentTxtData.zhlx = datazq.typeStr;
                _personAssessmentTxtData.zqgm = datazq.scaleStr;
                _personAssessmentTxtData.hcmj = datazq.hcmjStr;
                _personAssessmentTxtData.wztfzl = datazq.wztfzlStr;
                _personAssessmentTxtData.dzyrs = datazq.dzyrsStr;
                break;
            case ShowZyDataType.rwxxShow:
                var datarw = JsonConvert.DeserializeObject<ZbldRwystj>(data.dataStr);
                _personAssessmentTxtData.rwystj = new TaskElements() { jc = 1, bjd = datarw.bjNum, lsqjd = datarw.qjdNum, qsd = datarw.qsdNum, azd = datarw.azdNum, yy = datarw.yyNum };
                break;
            case ShowZyDataType.rwqzbShow:
                var datazb = JsonConvert.DeserializeObject<XczhRwqzb>(data.dataStr);
                _personAssessmentTxtData.jzxx = new List<UnitInfo>();
                for (int i = 0; i < datazb.zbRwqInfo.Count; i++)
                {
                    var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, datazb.zbRwqInfo[i].id));
                    var strs = itemEquip.textInfo.Split('_');
                    List<string> sbs = new List<string>();
                    if (itemEquip.isTS) sbs.Add("吊桶");
                    if (itemEquip.isSJJY) sbs.Add("吊索");
                    _personAssessmentTxtData.jzxx.Add(new UnitInfo()
                    {
                        jzName = MyDataInfo.BeUsedJizus[int.Parse(strs[5])], jx = itemEquip.name, zzsb = sbs,
                        zyl = float.Parse(datazb.zbRwqInfo[i].zyl), zzl = float.Parse(datazb.zbRwqInfo[i].zzl), dmwhTime = 8
                    });
                }

                break;
        }
    }

    public void OnGetPdfData_TrainsPintType(TrainsPintType type, string data)
    {
        switch (type)
        {
            case TrainsPintType.ZBLDRouteDeclaration:
                var airLineDatas = data.Split('_');
                _personAssessmentTxtData.hxgh = airLineDatas[0];
                _personAssessmentTxtData.hlds = new List<string>();
                for (int i = 1; i < airLineDatas.Length; i++)
                {
                    _personAssessmentTxtData.hlds.Add(airLineDatas[i]);
                }

                break;
            case TrainsPintType.ZBLDSendTask:
                _personAssessmentTxtData.rwjl = data;
                break;
        }
    }

    private void OnGetScore(string data)
    {
        scoreData = JsonConvert.DeserializeObject<ScoreStatistics>(data);
        //合一下各端总分
        scoreData.firstZhyTotalScore = 100 * ((scoreData.qrzqxx + scoreData.cdzbxxqr + scoreData.cdryxxqr + scoreData.zchxsb + scoreData.xdrw) * 0.091f / (50 * 0.091f));
        scoreData.secondZhyTotalScore = 100 * (((scoreData.lsrw + scoreData.qrzbztxx) * 0.204f + scoreData.fprwbxdrw * 0.223f + scoreData.qrtqczbg * 0.482f) / (20 * 0.204f + 10 * 0.223f + 10 * 0.482f));
        for (int i = 0; i < scoreData.thirdZhyScores.Count; i++)
        {
            var itemJz = scoreData.thirdZhyScores[i];
            itemJz.jzZhyTotalScore = 100 * ((itemJz.qrzyl + itemJz.rwqyhxgh + itemJz.xxczhybg) * 0.482f / (30 * 0.482f));
        }
    }

    private void GenerateFireExtinguishingReport()
    {
        if (_personAssessmentTxtData == null) _personAssessmentTxtData = new PersonAssessment_TxtData();
        PDFReport report = new PDFReport();
        var playerInfo = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, MyDataInfo.leadId));
        string mName = playerInfo.PlayerName, mId = playerInfo.RoleId, mAbstract = misDescription;
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[30]).CompareTo(float.Parse(b.AttributeInfos[30])));
        float minRyxhl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[30]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[29]).CompareTo(float.Parse(b.AttributeInfos[29])));
        float minDj = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[29]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(b.AttributeInfos[9]).CompareTo(float.Parse(a.AttributeInfos[9])));
        float maxSpeed = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[9]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(b.AttributeInfos[15]).CompareTo(float.Parse(a.AttributeInfos[15])));
        float maxZzl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[15]);
        float minWatersTime = float.MaxValue;
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            float itemWatersTime = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[16]) + float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[17]);
            if (itemWatersTime < minWatersTime) minWatersTime = itemWatersTime;
        }

        ResultFireWaterSystemData rfsystem = new ResultFireWaterSystemData
        {
            单位燃烧面积投水需求 = cdata.dwrsmjtsxq,
            最大巡航速度 = maxSpeed,
            单次取水和投水时间 = minWatersTime / 60f,
            吊桶单次最大装载量 = maxZzl,
            最低小时燃油消耗率 = minRyxhl,
            最低直升机单价 = minDj
        };
        float tszl = 0;
        float kstssk = float.MaxValue;
        float rwjssk = float.MinValue;
        float zhc = 0;
        int zjc = 0;
        float ghzmj = 0, rszmj = 0, csghzmj = 0, csrszmj = 0, firetszl = 0;
        float minWater2FireDistance = float.MaxValue;
        _personAssessmentTxtData.rwys = new TaskElements2();
        _personAssessmentTxtData.rwys.sdz = new List<string>();
        _personAssessmentTxtData.rwys.hcmj = new List<float>();
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
                (sceneAllzy[i] as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float atszl);
                ghzmj += ghmj;
                rszmj += rsmj;
                csghzmj += csghmj;
                csrszmj += csrsmj;
                firetszl += atszl;
                _personAssessmentTxtData.rwys.sdz.Add(sceneAllzy[i].ziYuanName);
                _personAssessmentTxtData.rwys.hcmj.Add(csrsmj);
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

        ResultFireWaterOutData rfout = new ResultFireWaterOutData
        {
            任务结束时投水总量 = tszl,
            任务结束时过火总面积 = ghzmj,
            任务结束时燃烧面积 = rszmj,
            任务初始过火总面积 = csghzmj,
            任务初始燃烧面积 = csrszmj,
            开始投水时刻 = ConvertSecondsToHHMMSS(kstssk >= float.MaxValue / 2 ? 0 : kstssk), //增大检测范围，防止浮点误差
            取水点到投水点的最短路径 = minWater2FireDistance / 1000f,
            任务结束时刻 = rwjssk < 0 ? MyDataInfo.gameStartTime / 3600f : rwjssk / 3600f,
            总航程 = zhc / 1000f,
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
                (sceneAllzy[i] as ISourceOfAFire).getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float atszl);
                FireData itemFireData = new FireData()
                {
                    Id = sceneAllzy[i].BobjectId, Name = sceneAllzy[i].ziYuanName, burnArea = rsmj, burnedArea = ghmj,
                    initBurnedArea = csghmj, initBurnArea = csrsmj, WaterWeight = atszl
                };
                rfout.任务结束时各火场数据.Add(itemFireData);
            }
        }

        Dictionary<string, List<WaterMegData>> heliWaterMegList = new Dictionary<string, List<WaterMegData>>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            float endMissonTime = 0;
            if (MyDataInfo.sceneAllEquips[i].GetRecordedData().takeOffTime > 1)
                endMissonTime = MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime < 1 ? MyDataInfo.gameStartTime : MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime;
            HeliData hd1 = new HeliData
            {
                Id = MyDataInfo.sceneAllEquips[i].BObjectId,
                Name = MyDataInfo.sceneAllEquips[i].name,
                Price = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[29]),
                Consumption = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[30]),
                Speed = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[9]),
                TakeOffTime = MyDataInfo.sceneAllEquips[i].GetRecordedData().takeOffTime / 3600f,
                EndMissonTime = endMissonTime / 3600,
                WatersTime = (float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[16]) + float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[17])) / 60f,
                WatersMaxCoune = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[15]),
                IsCrash = MyDataInfo.sceneAllEquips[i].isCrash
            };
            List<HeliSortieData> hsd1List = new List<HeliSortieData>();
            List<WaterMegData> nwmdList = new List<WaterMegData>();
            var itemDatas = MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData;
            for (int j = 0; j < itemDatas.Count; j++)
            {
                HeliSortieData hsd1 = new HeliSortieData
                {
                    FirstWaterTime = itemDatas[j].firstLoadingGoodsTime / 3600f,
                    WaterFireTime = itemDatas[j].lastOperationTime / 3600f,
                    WaterZongWeight = itemDatas[j].totalWeight,
                    FirstWaterFireTime = itemDatas[j].firstOperationTime * 3600f
                };
                hsd1List.Add(hsd1);
                WaterMegData mpmd = new WaterMegData
                {
                    sortieIndex = j + 1,
                    StartWaterTime = ConvertSecondsToHHMMSS(itemDatas[j].firstLoadingGoodsTime),
                    EndWaterTime = ConvertSecondsToHHMMSS(itemDatas[j].lastOperationTime),
                    WaterWeight = itemDatas[j].totalWeight
                };
                nwmdList.Add(mpmd);
            }

            rfout.HeliSortieWaterDataList.Add(hd1, hsd1List);
            heliWaterMegList.Add(hd1.Name, nwmdList);
        }

        #region 岗位职责能力评估

        List<RescueForces> itemRescueForcesList = new List<RescueForces>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            if (MyDataInfo.sceneAllEquips[i].gameObject.activeSelf)
            {
                var itemEquip = MyDataInfo.sceneAllEquips[i];
                if (!string.IsNullOrEmpty(itemEquip.textInfo))
                {
                    var strs = itemEquip.textInfo.Split('_');
                    itemRescueForcesList.Add(new RescueForces()
                    {
                        jx = itemEquip.name, bh = itemEquip.name, jz = MyDataInfo.BeUsedJizus[int.Parse(strs[5])],
                        jzrs = "--", bzz = MyDataInfo.BeUsedBaozhangs[int.Parse(strs[6])], nun = "--"
                    });
                }
            }
        }

        parfw_level1 level1 = new parfw_level1()
        {
            zhlx = _personAssessmentTxtData.zhlx, zqgm = _personAssessmentTxtData.zqgm, hcmj = _personAssessmentTxtData.hcmj, cdjyll = itemRescueForcesList,
            hxgh = _personAssessmentTxtData.hxgh, hlds = _personAssessmentTxtData.hlds, rwystj = _personAssessmentTxtData.rwystj, rwjl = _personAssessmentTxtData.rwjl
        };
        _personAssessmentTxtData.rwys.qsd = new List<string>();
        _personAssessmentTxtData.rwys.bjd = new List<string>();
        _personAssessmentTxtData.rwys.bjc = new List<string>();
        for (int j = 0; j < sceneAllzy.Count; j++)
        {
            switch (sceneAllzy[j].ZiYuanType)
            {
                case ZiYuanType.Waters:
                    _personAssessmentTxtData.rwys.qsd.Add(sceneAllzy[j].ziYuanName);
                    break;
                case ZiYuanType.Supply:
                    _personAssessmentTxtData.rwys.bjd.Add(sceneAllzy[j].ziYuanName);
                    break;
                case ZiYuanType.Airport:
                    if (!string.Equals(sceneAllzy[j].ziYuanName, "机场"))
                        _personAssessmentTxtData.rwys.bjc.Add(sceneAllzy[j].ziYuanName);
                    break;
            }
        }

        List<TaskAllocation> rwfpData = new List<TaskAllocation>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            var itemEquip = MyDataInfo.sceneAllEquips[i];
            if (!itemEquip.gameObject.activeSelf || string.IsNullOrEmpty(itemEquip.textInfo)) continue;
            var strs = itemEquip.textInfo.Split('_');
            List<string> szds = new List<string>();
            var itemData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.SourceOfAFire);
            for (int j = 0; j < itemData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemData[j].BobjectId))
                    szds.Add(itemData[j].ziYuanName);
            }

            List<string> qsds = new List<string>();
            var itemQsdsData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.Waters);
            for (int j = 0; j < itemQsdsData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemQsdsData[j].BobjectId))
                    qsds.Add(itemQsdsData[j].ziYuanName);
            }

            List<string> bjds = new List<string>();
            var itemBjdsData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.Supply);
            for (int j = 0; j < itemBjdsData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemBjdsData[j].BobjectId))
                    bjds.Add(itemBjdsData[j].ziYuanName);
            }

            List<string> bjcs = new List<string>();
            var itemBjcsData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.Airport);
            for (int j = 0; j < itemBjcsData.Count; j++)
            {
                if (itemBjcsData[j].ziYuanName == "机场") continue;
                if (itemEquip.currentBindingZy.Contains(itemBjcsData[j].BobjectId))
                    bjcs.Add(itemBjcsData[j].ziYuanName);
            }

            rwfpData.Add(new TaskAllocation() { jzName = MyDataInfo.BeUsedJizus[int.Parse(strs[5])], szd = szds, qsd = qsds, bjd = bjds, bjc = bjcs });
        }

        parfw_level2 level2 = new parfw_level2()
        {
            jzxx = _personAssessmentTxtData.jzxx, rwys = _personAssessmentTxtData.rwys, rwfp = rwfpData
        };
        List<parfw_level3> level3s = new List<parfw_level3>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            if (!MyDataInfo.sceneAllEquips[i].gameObject.activeSelf) continue;
            var itemEquip = MyDataInfo.sceneAllEquips[i];
            if (string.IsNullOrEmpty(itemEquip.textInfo)) continue;
            var strs = itemEquip.textInfo.Split('_');
            (itemEquip as IDqChangePart).GetUsableOilAndLoad(out float oilNum, out float loadNum);
            parfw_level3 level3 = new parfw_level3()
            {
                jzname = MyDataInfo.BeUsedJizus[int.Parse(strs[5])], jzId = itemEquip.BeLongToCommanderId, zyl = oilNum, zzl = loadNum, rybz = itemEquip.OilWarnNum, cwzl = itemEquip.LandErrorNum, zbgzbg = itemEquip.isReportZbgz,
                zbgzzl = itemEquip.isReturnBack, tqbhbg = itemEquip.isReportTqbh, tqbhfh = itemEquip.isReturnBack, xfxzq = itemEquip.isReportXfxzq, tianqiStr = tianqiInfoStr,
                guzhangStr = guzhangInfoStr.ContainsKey(itemEquip.BObjectId) ? guzhangInfoStr[itemEquip.BObjectId] : "正常", bindingZy = itemEquip.currentBindingZy
            };
            level3.zScore = 100 - level3.rybz - level3.cwzl - (level3.zbgzbg ? 0 : 10) - (level3.zbgzzl ? 0 : 10) - (level3.tqbhbg ? 0 : 10) - (level3.tqbhfh ? 0 : 10);
            level3s.Add(level3);
        }

        PersonAssessment_ResultFireWater personAssData = new PersonAssessment_ResultFireWater() { yjzhy = level1, ejzhy = level2, sjzhy = level3s };

        #endregion

        EvalManage em = new EvalManage();
        sender.LogError(JsonConvert.SerializeObject(rfout));
        sender.LogError(JsonConvert.SerializeObject(rfsystem));
        ResultFireWaterData rfwd = em.EvalWaterCompute(rfout, rfsystem);

        if (showAllOperatorInfos == null) Debug.LogError("showAllOperatorInfos");
        if (playerEquips == null) Debug.LogError("playerEquips");
        if (playerZiyuans == null) Debug.LogError("playerZiyuans");
        report.CreateWaterMissionReport(DateTime.Now.ToString("HH_mm_ss"), $"{misName}评估报告", mName, mId, mAbstract, rfwd, rfout, showAllOperatorInfos, heliWaterMegList, playerEquips, playerZiyuans, reportPlayers.Count, personAssData,
            scoreData, PeculiarDatas);
    }

    private void GenerateRescueReport()
    {
        if (_personAssessmentTxtData == null) _personAssessmentTxtData = new PersonAssessment_TxtData();
        PDFReport report = new PDFReport();
        var playerInfo = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, MyDataInfo.leadId));
        string mName = playerInfo.PlayerName, mId = playerInfo.RoleId, mAbstract = misDescription;
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[30]).CompareTo(float.Parse(b.AttributeInfos[30])));
        float minRyxhl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[30]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(a.AttributeInfos[29]).CompareTo(float.Parse(b.AttributeInfos[29])));
        float minDj = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[29]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(b.AttributeInfos[9]).CompareTo(float.Parse(a.AttributeInfos[9])));
        float maxSpeed = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[9]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(b.AttributeInfos[6]).CompareTo(float.Parse(a.AttributeInfos[6])));
        float maxYzRs = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[6]);
        MyDataInfo.sceneAllEquips.Sort((a, b) => float.Parse(b.AttributeInfos[4]).CompareTo(float.Parse(a.AttributeInfos[4])));
        float maxYzzl = float.Parse(MyDataInfo.sceneAllEquips[0].AttributeInfos[4]);
        //灾区需转运总人数
        float zqxzyzrs = 0;
        int totalNoGoodsPerson = 0;

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is IDisasterArea)
            {
                (sceneAllzy[i] as IDisasterArea).getTaskProgress(out int currentNuma, out int maxNuma);
                zqxzyzrs += maxNuma;
                if ((sceneAllzy[i] as IDisasterArea).getWoundedPersonnelType() == 2)
                    totalNoGoodsPerson += maxNuma;
            }
        }

        float minPersonTime = float.MaxValue, minGoodsTime = float.MaxValue;
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            float itemPersonTime = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[22]) * float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[6]) +
                                   float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[26]) * float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[6]);
            if (itemPersonTime < minPersonTime) minPersonTime = itemPersonTime;
            float itemGoodsTime = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[19]) + float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[20]);
            if (itemGoodsTime < minGoodsTime) minGoodsTime = itemGoodsTime;
        }

        sender.LogError($"救人最小：{minPersonTime}投物资最小:{minGoodsTime}");


        ResultRescueSystemData rfsystem = new ResultRescueSystemData
        {
            人均救援物资需求 = cdata.dwrsmjtsxq,
            受灾需转运总人数 = zqxzyzrs,
            所有不需要物资的人员数量 = totalNoGoodsPerson,
            最大巡航速度 = maxSpeed,
            单次人员吊救时间 = minPersonTime,
            单次物资投放时间 = minGoodsTime,
            直升机单次最大运载人数 = maxYzRs,
            直升机单次最大运载物资重量 = maxYzzl,
            最低小时燃油消耗率 = minRyxhl,
            最低直升机单价 = minDj
        };
        float twzzl = 0;
        float rwjssk = float.MinValue;
        float zhc = 0;
        int zjc = 0;
        float firstTimea = float.MaxValue, totalWeighta = 0;
        int totalPersona = 0;
        float minGoodsPoint2RescueStationDis = float.MaxValue;
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
                if (firstTime > 1 && firstTime < firstTimea) firstTimea = firstTime;
                totalWeighta += totalWeight;
                for (int j = 0; j < sceneAllzy.Count; j++)
                {
                    if (sceneAllzy[j].ZiYuanType == ZiYuanType.GoodsPoint)
                    {
                        if (Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position) < minGoodsPoint2RescueStationDis)
                        {
                            minGoodsPoint2RescueStationDis = Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position);
                        }
                    }

                    if (sceneAllzy[j].ZiYuanType == ZiYuanType.DisasterArea)
                    {
                        if (Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position) < minDisasterArea2RescueStationDis)
                        {
                            minDisasterArea2RescueStationDis = Vector3.Distance(sceneAllzy[i].transform.position, sceneAllzy[j].transform.position);
                        }
                    }
                }
            }

            if (sceneAllzy[i] is IDisasterArea)
            {
                (sceneAllzy[i] as IDisasterArea).getTaskProgress(out int currentNum, out int maxNum);
                totalPersona += currentNum;
            }
        }

        sender.LogError("首批物资到达时刻" + firstTimea + "判断一下" + (firstTimea > float.MaxValue / 2));
        sender.LogError("给传过去的数是：" + (firstTimea > float.MaxValue / 2 ? 0 : firstTimea / 3600f));
        ResultMaterialPersonOutData cfout = new ResultMaterialPersonOutData
        {
            任务结束时救援物资投放总重量 = twzzl,
            任务结束时各救援安置点物资投放重量 = totalWeighta,
            首批救援物资到达安置点时刻 = ConvertSecondsToHHMMSS(firstTimea > float.MaxValue / 2 ? 0 : firstTimea),
            物资装载起降点到安置点的最短路径 = minGoodsPoint2RescueStationDis / 1000f,
            任务结束时刻 = rwjssk < 0 ? MyDataInfo.gameStartTime / 3600 : rwjssk / 3600f,
            总航程 = zhc / 1000,
            所有飞机总架次 = zjc,
            受灾地点数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.DisasterArea).Count,
            临时安置点数量 = sceneAllzy.FindAll(a => a.ZiYuanType == ZiYuanType.RescueStation).Count,
            任务结束时转运总人数 = zqxzyzrs - totalPersona,
            救援点到安置点的最短路径 = minDisasterArea2RescueStationDis / 1000
        };
        cfout.任务结束时各安置点数据 = new List<MaterialData>();
        cfout.任务结束时各灾区数据 = new List<DisasterAreaData>();

        for (int i = 0; i < sceneAllzy.Count; i++)
        {
            if (sceneAllzy[i] is IRescueStation)
            {
                (sceneAllzy[i] as IRescueStation).getResData(out float firstTime, out float totalWeight, out int totalPerson);
                MaterialData fd1 = new MaterialData
                {
                    Id = sceneAllzy[i].ZiYuanType == ZiYuanType.Hospital ? "-1" : sceneAllzy[i].BobjectId,
                    Name = sceneAllzy[i].ziYuanName,
                    MaterialWeight = totalWeight,
                    PersonCount = totalPerson
                };
                cfout.任务结束时各安置点数据.Add(fd1);
            }

            if (sceneAllzy[i] is IDisasterArea)
            {
                (sceneAllzy[i] as IDisasterArea).getTaskProgress(out int currentNum, out int maxNum);
                DisasterAreaData dad = new DisasterAreaData()
                {
                    Id = sceneAllzy[i].BobjectId, zyrs = maxNum - currentNum, personCount = maxNum
                };
                cfout.任务结束时各灾区数据.Add(dad);
            }
        }

        float personMinTime = float.MaxValue, goodsMinTime = float.MaxValue;
        Dictionary<string, List<MaterialPersonMegData>> heliMegList = new Dictionary<string, List<MaterialPersonMegData>>();
        cfout.HeliSortieMaterialPersonDataList = new Dictionary<HeliData, List<HeliSortieData>>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            float endMissonTime = 0;
            if (MyDataInfo.sceneAllEquips[i].GetRecordedData().takeOffTime > 1)
                endMissonTime = MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime < 1 ? MyDataInfo.gameStartTime : MyDataInfo.sceneAllEquips[i].GetRecordedData().endTaskTime;
            HeliData hd1 = new HeliData
            {
                Id = MyDataInfo.sceneAllEquips[i].BObjectId,
                Name = MyDataInfo.sceneAllEquips[i].name,
                Price = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[29]),
                Consumption = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[30]),
                Speed = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[9]),
                TakeOffTime = MyDataInfo.sceneAllEquips[i].GetRecordedData().takeOffTime / 3600f,
                EndMissonTime = endMissonTime / 3600f,
                PersonTime = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[22]) + float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[26]),
                PersonMaxCount = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[6]),
                MaterialMaxCount = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[4]),
                MaterialTime = float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[19]) + float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[20]),
                IsCrash = MyDataInfo.sceneAllEquips[i].isCrash
            };
            List<HeliSortieData> hsd1List = new List<HeliSortieData>();
            List<MaterialPersonMegData> nwmdList = new List<MaterialPersonMegData>();
            var itemDatas = MyDataInfo.sceneAllEquips[i].GetRecordedData().eachSortieData;
            for (int j = 0; j < itemDatas.Count; j++)
            {
                HeliSortieData hsd1 = new HeliSortieData
                {
                    MaterialLoadingTime = itemDatas[j].firstLoadingGoodsTime / 3600f,
                    MaterialWeight = itemDatas[j].totalWeight,
                    MaterialPointTime = itemDatas[j].lastOperationTime / 3600f,
                    PersonCount = itemDatas[j].numberOfRescues,
                    PersonFirstTime = itemDatas[j].firstRescuePersonTime / 3600f, PersonEndTime = itemDatas[j].placementOfPersonTime / 3600f,
                    MaterialAToBDistance = itemDatas[j].goodsDistance / 1000, PersonAToBDistance = itemDatas[j].personDistance / 1000
                };
                hsd1List.Add(hsd1);
                MaterialPersonMegData mpmd = new MaterialPersonMegData
                {
                    sortieIndex = j + 1,
                    TakeOffTime = ConvertSecondsToHHMMSS(MyDataInfo.sceneAllEquips[i].GetRecordedData().takeOffTime),
                    EndMissionTime = ConvertSecondsToHHMMSS(endMissonTime),
                    MaterialTime = ConvertSecondsToHHMMSS(itemDatas[j].lastOperationTime),
                    MaterialWeight = itemDatas[j].totalWeight,
                    PersonCount = itemDatas[j].numberOfRescues
                };
                nwmdList.Add(mpmd);

                if (itemDatas[j].personDistance > 1)
                {
                    float itempersonMinTime = (float)((itemDatas[j].personDistance / 1000f / float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[8]) + hd1.PersonTime * float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[6])) *
                                                      (zqxzyzrs / hd1.PersonMaxCount));
                    if (itempersonMinTime < personMinTime) personMinTime = itempersonMinTime;
                    sender.LogError("itemPerson值：" + itempersonMinTime);
                }

                if (itemDatas[j].goodsDistance > 1)
                {
                    float itemgoodsMinTime = (float)((itemDatas[j].goodsDistance / 1000f / float.Parse(MyDataInfo.sceneAllEquips[i].AttributeInfos[8]) + hd1.MaterialTime) * (zqxzyzrs * rfsystem.人均救援物资需求 / hd1.MaterialMaxCount));
                    if (itemgoodsMinTime < goodsMinTime) goodsMinTime = itemgoodsMinTime;
                    sender.LogError("itemGoods值：" + itemgoodsMinTime);
                }
            }

            cfout.HeliSortieMaterialPersonDataList.Add(hd1, hsd1List);
            heliMegList.Add(hd1.Name, nwmdList);
        }

        if (personMinTime > float.MaxValue / 2) personMinTime = 0;
        if (goodsMinTime > float.MaxValue / 2) goodsMinTime = 0;

        sender.LogError($"最小救援{personMinTime}最小物资{goodsMinTime}");

        #region 岗位职责能力评估

        List<RescueForces> itemRescueForcesList = new List<RescueForces>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            if (MyDataInfo.sceneAllEquips[i].gameObject.activeSelf)
            {
                var itemEquip = MyDataInfo.sceneAllEquips[i];
                if (!string.IsNullOrEmpty(itemEquip.textInfo))
                {
                    var strs = itemEquip.textInfo.Split('_');
                    itemRescueForcesList.Add(new RescueForces()
                    {
                        jx = itemEquip.name, bh = itemEquip.name, jz = MyDataInfo.BeUsedJizus[int.Parse(strs[5])],
                        jzrs = "--", bzz = MyDataInfo.BeUsedBaozhangs[int.Parse(strs[6])], nun = "--"
                    });
                }
            }
        }

        parfw_level1 level1 = new parfw_level1()
        {
            zhlx = _personAssessmentTxtData.zhlx, zqgm = _personAssessmentTxtData.zqgm, wztfzl = float.Parse(string.IsNullOrEmpty(_personAssessmentTxtData.wztfzl) ? "0" : _personAssessmentTxtData.wztfzl),
            dzyrs = int.Parse(string.IsNullOrEmpty(_personAssessmentTxtData.dzyrs) ? "0" : _personAssessmentTxtData.dzyrs), cdjyll = itemRescueForcesList, hxgh = _personAssessmentTxtData.hxgh, 
            hlds = _personAssessmentTxtData.hlds, rwystj = _personAssessmentTxtData.rwystj, rwjl = _personAssessmentTxtData.rwjl
        };
        _personAssessmentTxtData.rwys = new TaskElements2();
        _personAssessmentTxtData.rwys.sdz = new List<string>();
        _personAssessmentTxtData.rwys.bjd = new List<string>();
        _personAssessmentTxtData.rwys.bjc = new List<string>();
        _personAssessmentTxtData.rwys.dzyry = new List<string>();
        _personAssessmentTxtData.rwys.azd = new List<string>();
        _personAssessmentTxtData.rwys.yy = new List<string>();
        for (int j = 0; j < sceneAllzy.Count; j++)
        {
            switch (sceneAllzy[j].ZiYuanType)
            {
                case ZiYuanType.Supply:
                    _personAssessmentTxtData.rwys.bjd.Add(sceneAllzy[j].ziYuanName);
                    break;
                case ZiYuanType.Airport:
                    if (!string.Equals(sceneAllzy[j].ziYuanName, "机场"))
                        _personAssessmentTxtData.rwys.bjc.Add(sceneAllzy[j].ziYuanName);
                    break;
                case ZiYuanType.DisasterArea:
                    (sceneAllzy[j] as IDisasterArea).getTaskProgress(out int currentNuma, out int maxNuma);
                    _personAssessmentTxtData.rwys.sdz.Add(sceneAllzy[j].ziYuanName);
                    _personAssessmentTxtData.rwys.dzyry.Add(maxNuma.ToString());
                    break;
                case ZiYuanType.RescueStation:
                    _personAssessmentTxtData.rwys.azd.Add(sceneAllzy[j].ziYuanName);
                    break;
                case ZiYuanType.Hospital:
                    _personAssessmentTxtData.rwys.yy.Add(sceneAllzy[j].ziYuanName);
                    break;
            }
        }

        List<TaskAllocation> rwfpData = new List<TaskAllocation>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            var itemEquip = MyDataInfo.sceneAllEquips[i];
            if (!itemEquip.gameObject.activeSelf || string.IsNullOrEmpty(itemEquip.textInfo)) continue;
            var strs = itemEquip.textInfo.Split('_');
            List<string> szds = new List<string>();
            var itemData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.DisasterArea);
            for (int j = 0; j < itemData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemData[j].BobjectId))
                    szds.Add(itemData[j].ziYuanName);
            }

            List<string> azds = new List<string>();
            var itemAzdsData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.RescueStation);
            for (int j = 0; j < itemAzdsData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemAzdsData[j].BobjectId))
                    azds.Add(itemAzdsData[j].ziYuanName);
            }

            List<string> yys = new List<string>();
            var itemyysData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.Hospital);
            for (int j = 0; j < itemyysData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemyysData[j].BobjectId))
                    yys.Add(itemyysData[j].ziYuanName);
            }

            List<string> bjds = new List<string>();
            var itemBjdsData = sceneAllzy.FindAll(x => x.ZiYuanType == ZiYuanType.Supply);
            for (int j = 0; j < itemBjdsData.Count; j++)
            {
                if (itemEquip.currentBindingZy.Contains(itemBjdsData[j].BobjectId))
                    bjds.Add(itemBjdsData[j].ziYuanName);
            }

            rwfpData.Add(new TaskAllocation() { jzName = MyDataInfo.BeUsedJizus[int.Parse(strs[5])], szd = szds, azd = azds, yy = yys, bjd = bjds });
        }

        parfw_level2 level2 = new parfw_level2()
        {
            jzxx = _personAssessmentTxtData.jzxx, rwys = _personAssessmentTxtData.rwys, rwfp = rwfpData
        };
        List<parfw_level3> level3s = new List<parfw_level3>();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            if (!MyDataInfo.sceneAllEquips[i].gameObject.activeSelf) continue;
            var itemEquip = MyDataInfo.sceneAllEquips[i];
            if (string.IsNullOrEmpty(itemEquip.textInfo)) continue;
            var strs = itemEquip.textInfo.Split('_');
            (itemEquip as IDqChangePart).GetUsableOilAndLoad(out float oilNum, out float loadNum);
            parfw_level3 level3 = new parfw_level3()
            {
                jzname = MyDataInfo.BeUsedJizus[int.Parse(strs[5])], jzId = itemEquip.BeLongToCommanderId, zyl = oilNum, zzl = loadNum, rybz = itemEquip.OilWarnNum, cwzl = itemEquip.LandErrorNum, zbgzbg = itemEquip.isReportZbgz,
                zbgzzl = itemEquip.isReturnBack, tqbhbg = itemEquip.isReportTqbh, tqbhfh = itemEquip.isReturnBack, xfxzq = itemEquip.isReportXfxzq, tianqiStr = tianqiInfoStr,
                guzhangStr = guzhangInfoStr.ContainsKey(itemEquip.BObjectId) ? guzhangInfoStr[itemEquip.BObjectId] : "正常", bindingZy = itemEquip.currentBindingZy
            };
            level3.zScore = 100 - level3.rybz - level3.cwzl - (level3.zbgzbg ? 0 : 5) - (level3.zbgzzl ? 0 : 5) - (level3.tqbhbg ? 0 : 5) - (level3.tqbhfh ? 0 : 5) - (level3.xfxzq ? 0 : 10);
            level3s.Add(level3);
        }

        PersonAssessment_ResultFireWater personAssData = new PersonAssessment_ResultFireWater() { yjzhy = level1, ejzhy = level2, sjzhy = level3s };

        #endregion

        EvalManage em = new EvalManage();
        sender.LogError(JsonConvert.SerializeObject(cfout));
        sender.LogError(JsonConvert.SerializeObject(rfsystem));
        ResultMaterialPersonData rfwd = em.EvalMaterialCompute(cfout, rfsystem, personMinTime, goodsMinTime);

        report.CreateRescueMissionReport(DateTime.Now.ToString("HH_mm_ss"), $"{misName}评估报告", mName, mId, mAbstract, rfwd, cfout, rfsystem, showAllOperatorInfos, heliMegList, playerEquips, playerZiyuans, reportPlayers.Count,
            personAssData, scoreData, PeculiarDatas);
    }


    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }
}

public class ComanderData
{
    public float dwrsmjtsxq;
}