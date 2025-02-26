using DM.IFS;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using DataTranfsers;
using ToolsLibrary;
using UnityEngine;

namespace ReportGenerate
{
    public class PDFReport
    {
        public string dirPath;
        public string reportPath;
        iTextSharp.text.Font fontTitle, fontSub, fontTextBold, fontText, fontcellB, fontNull;

        public PDFReport()
        {
            dirPath = Application.dataPath + "/MapLib/Report";
            string fontPath = Application.dataPath + "/Font/wryh.ttf";
            BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            fontTitle = new iTextSharp.text.Font(font, 24);
            fontSub = new iTextSharp.text.Font(font, 15, iTextSharp.text.Font.BOLD);
            fontTextBold = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.BOLD);
            fontText = new iTextSharp.text.Font(font, 14);
            fontcellB = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.BOLD);
            fontNull = new iTextSharp.text.Font(font, 5);
        }

        /// <summary>
        /// 灭火任务报告
        /// </summary>
        public void CreateWaterMissionReport(string reportId, string reportName, string userName, string Id, string Abstract, ResultFireWaterData resultData, ResultFireWaterOutData resultOutData, List<string> trainData,
            Dictionary<string, List<WaterMegData>> heliMegList, Dictionary<string, List<string>> usersEquips, Dictionary<string, List<string>> usersZiyuans, int reports, PersonAssessment_ResultFireWater personAss, ScoreStatistics scores,
            Dictionary<int, List<string>> tqData)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            //string fileName = DateTime.Now.ToLongDateString() + (Directory.GetFiles(dirPath).Length + 1);
            //string fileDate = DateTime.Now.ToLongDateString()+ DateTime.Now.Hour+"-"+ DateTime.Now.Minute+"-"+DateTime.Now.Second;
            reportPath = dirPath + "/" + userName + "(" + reportId + ").pdf";
            string fontPath = Application.dataPath + "/Font/wryh.ttf";
            Document doc = new Document();
            FileStream fi = new FileStream(reportPath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fi);
            doc.Open();

            Paragraph nullString = new Paragraph("  ", fontNull);
            Paragraph title = new Paragraph(reportName, fontTitle)
            {
                Alignment = Rectangle.ALIGN_CENTER
            };
            doc.Add(title);
            doc.Add(nullString);
            // string info = "用户名:" + userName + "                                    日期:" + DateTime.Now.ToLongDateString();
            // Paragraph date = new Paragraph(info, fontText)
            // {
            //     Alignment = Rectangle.ALIGN_CENTER
            // };
            // doc.Add(date);
            // doc.Add(nullString);

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = 480, //表格总宽度
                LockedWidth = true //锁定宽度
            };
            table.SetWidths(new int[] { 450, 450, 450, 450 });

            // Paragraph mesAbstract = new Paragraph(Abstract, fontText);
            // mesAbstract.FirstLineIndent = 28; //设置段落的首行缩进
            // doc.Add(mesAbstract);
            doc.Add(nullString);

            #region 救援力量配置不用了

            //
            // Paragraph messageUserInfo = new Paragraph("1.救援力量配置", fontSub);
            // doc.Add(messageUserInfo);
            // doc.Add(nullString);
            // PdfPTable tableUserInfos = new PdfPTable(5);
            // tableUserInfos.AddCell(MyCell("指挥员", 1, 1));
            // tableUserInfos.AddCell(MyCell("装备", 2, 1));
            // tableUserInfos.AddCell(MyCell("资源", 2, 1));
            //
            // Dictionary<string, string[]> userInfos = new Dictionary<string, string[]>();
            // foreach (var equip in usersEquips)
            // {
            //     if (!userInfos.ContainsKey(equip.Key)) userInfos.Add(equip.Key, new string[2]);
            //     for (int i = 0; i < equip.Value.Count; i++)
            //     {
            //         userInfos[equip.Key][0] += equip.Value[i] + '、';
            //     }
            //
            //     if (!string.IsNullOrEmpty(userInfos[equip.Key][0]) && userInfos[equip.Key][0].Length > 1)
            //         userInfos[equip.Key][0] = userInfos[equip.Key][0].Remove(userInfos[equip.Key][0].Length - 1);
            // }
            //
            // foreach (var ziyuan in usersZiyuans)
            // {
            //     if (!userInfos.ContainsKey(ziyuan.Key)) userInfos.Add(ziyuan.Key, new string[2]);
            //     for (int i = 0; i < ziyuan.Value.Count; i++)
            //     {
            //         userInfos[ziyuan.Key][1] += ziyuan.Value[i] + '、';
            //     }
            //
            //     if (!string.IsNullOrEmpty(userInfos[ziyuan.Key][1]) && userInfos[ziyuan.Key][1].Length > 1)
            //         userInfos[ziyuan.Key][1] = userInfos[ziyuan.Key][1].Remove(userInfos[ziyuan.Key][1].Length - 1);
            // }
            //
            // foreach (var item in userInfos)
            // {
            //     tableUserInfos.AddCell(MyCell(item.Key, 1, 5));
            //     tableUserInfos.AddCell(MyCell(item.Value[0], 2, 5));
            //     tableUserInfos.AddCell(MyCell(item.Value[1], 2, 5));
            // }
            //
            // doc.Add(tableUserInfos);
            // doc.Add(nullString);

            #endregion

            Paragraph messagePersonAssessment = new Paragraph("一、岗位职责能力评估", fontSub);
            doc.Add(messagePersonAssessment);
            doc.Add(nullString);
            Paragraph paLevel1;
            if (scores == null) paLevel1 = new Paragraph("1.一级指挥员    ", fontSub);
            else paLevel1 = new Paragraph($"1.一级指挥员    得分{scores.firstZhyTotalScore}", fontSub);
            paLevel1.IndentationLeft = 20f;
            doc.Add(paLevel1);
            doc.Add(nullString);
            //数据：personAss

            //一级指挥员
            PdfPTable commander1 = new PdfPTable(6);

            commander1.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 6, 1));
            if (scores == null) commander1.AddCell(MyCell($"确认灾情信息", 6, 1));
            else commander1.AddCell(MyCell($"确认灾情信息: {scores.qrzqxx}分", 6, 1));
            commander1.AddCell(MyCell("灾害类型", 1, 1));
            commander1.AddCell(MyCell("灾情规模", 1, 1));
            commander1.AddCell(MyCell("火场面积（平方米）", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            //值
            commander1.AddCell(MyCell(personAss.yjzhy.zhlx, 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.zqgm, 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.hcmj, 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));

            if (scores == null) commander1.AddCell(MyCell($"确认出动装备信息", 6, 1));
            else commander1.AddCell(MyCell($"确认出动装备信息： {scores.cdzbxxqr}分", 6, 1));
            commander1.AddCell(MyCell("机型", 2, 1));
            commander1.AddCell(MyCell("编号", 2, 1));
            commander1.AddCell(MyCell("数量", 2, 1));
            //值
            for (int i = 0; i < personAss.yjzhy.cdjyll.Count; i++)
            {
                int index = i;
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jx, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].bh, 2, 1));
                commander1.AddCell(MyCell("1", 2, 1));
            }

            if (scores == null) commander1.AddCell(MyCell($"确认出动人员信息", 6, 1));
            else commander1.AddCell(MyCell($"确认出动人员信息： {scores.cdryxxqr}分", 6, 1));
            commander1.AddCell(MyCell("机型编号", 2, 1));
            commander1.AddCell(MyCell("机组", 2, 1));
            commander1.AddCell(MyCell("保障组", 2, 1));
            //值
            for (int i = 0; i < personAss.yjzhy.cdjyll.Count; i++)
            {
                int index = i;
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jx, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jz, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].bzz, 2, 1));
            }

            if (scores == null) commander1.AddCell(MyCell($"申报转场航线", 6, 1));
            else commander1.AddCell(MyCell($"申报转场航线： {scores.zchxsb}分", 6, 1));
            commander1.AddCell(MyCell("航线信息", 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.hxgh, 5, 1));

            if (scores == null) commander1.AddCell(MyCell($"下达任务", 6, 1));
            else commander1.AddCell(MyCell($"下达任务： {scores.xdrw}分", 6, 1));
            commander1.AddCell(MyCell("任务简令", 6, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.rwjl, 6, 2));

            #region 任务要素统计，不用了

            // commander1.AddCell(MyCell("任务要素统计", 6, 1));
            // commander1.AddCell(MyCell("机场", 1, 1));
            // commander1.AddCell(MyCell("临时起降点", 1, 1));
            // commander1.AddCell(MyCell("补给点", 1, 1));
            //
            // commander1.AddCell(MyCell("取水点", 1, 1));
            // commander1.AddCell(MyCell("", 1, 1));
            // commander1.AddCell(MyCell("", 1, 1));
            //
            // commander1.AddCell(MyCell(personAss.yjzhy.rwystj.jc.ToString(), 1, 1));
            // commander1.AddCell(MyCell(personAss.yjzhy.rwystj.lsqjd.ToString(), 1, 1));
            //
            // commander1.AddCell(MyCell(personAss.yjzhy.rwystj.bjd.ToString(), 1, 1));
            //
            //
            // commander1.AddCell(MyCell(personAss.yjzhy.rwystj.qsd.ToString(), 1, 1));
            // commander1.AddCell(MyCell("", 1, 1));
            // commander1.AddCell(MyCell("", 1, 1));

            #endregion

            doc.Add(commander1);
            doc.Add(nullString);

            Paragraph paLevel2;
            if (scores == null) paLevel2 = new Paragraph($"2.二级指挥员", fontSub);
            else paLevel2 = new Paragraph($"2.二级指挥员    得分:{scores.secondZhyTotalScore}", fontSub);
            paLevel2.IndentationLeft = 20f;
            doc.Add(paLevel2);
            doc.Add(nullString);

            PdfPTable commander2 = new PdfPTable(6);
            commander2.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 6, 1));
            if (scores == null) commander2.AddCell(MyCell($"领受任务", 6, 1));
            else commander2.AddCell(MyCell($"领受任务： {scores.lsrw}分", 6, 1));
            if (scores == null) commander2.AddCell(MyCell($"确认出动装备信息", 6, 1));
            else commander2.AddCell(MyCell($"确认出动装备信息： {scores.qrzbztxx}分", 6, 1));
            commander2.AddCell(MyCell("机组信息", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("机型", 1, 1));
            commander2.AddCell(MyCell("装载设备", 1, 1));
            commander2.AddCell(MyCell("载油量（千克）", 1, 1));
            commander2.AddCell(MyCell("可用载重（千克）", 1, 1));
            commander2.AddCell(MyCell("地面维护时间间隔（小 时）", 1, 1));

            for (int i = 0; i < personAss.ejzhy.jzxx.Count; i++)
            {
                int index = i;
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].jzName, 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].jx, 1, 1));
                commander2.AddCell(MyCell(string.Join(" ", personAss.ejzhy.jzxx[index].zzsb), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].zyl.ToString(), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].zzl.ToString(), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].dmwhTime.ToString(), 1, 1));
            }

            if (scores == null) commander2.AddCell(MyCell($"完成任务分配并下达任务", 6, 1));
            else commander2.AddCell(MyCell($"完成任务分配并下达任务： {scores.fprwbxdrw}分", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("受灾点", 1, 1));
            commander2.AddCell(MyCell("取水点", 1, 1));
            commander2.AddCell(MyCell("补给站", 1, 1));
            commander2.AddCell(MyCell("备降场", 1, 1));
            commander2.AddCell(MyCell("", 1, 1));
            for (int i = 0; i < personAss.ejzhy.rwfp.Count; i++)
            {
                int index = i;
                commander2.AddCell(MyCell(personAss.ejzhy.rwfp[index].jzName, 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].szd), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].qsd), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].bjd), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].bjc), 1, 1));
                commander2.AddCell(MyCell("", 1, 1));
            }

            if (scores == null) commander2.AddCell(MyCell($"确认机长的特情处置报告", 6, 1));
            else commander2.AddCell(MyCell($"确认机长的特情处置报告： {scores.qrtqczbg}分", 6, 1));
            commander2.AddCell(MyCell("报告次数", 3, 1));
            commander2.AddCell(MyCell($"{(tqData.ContainsKey(2) ? tqData[2].Count : 0).ToString()}", 3, 1));
            commander2.AddCell(MyCell("确认次数", 3, 1));
            commander2.AddCell(MyCell($"{(tqData.ContainsKey(3) ? tqData[3].Count : 0).ToString()}", 3, 1));

            doc.Add(commander2);
            doc.Add(nullString);

            Paragraph paLevel3 = new Paragraph($"3.三级指挥员", fontSub);
            paLevel3.IndentationLeft = 20f;
            doc.Add(paLevel3);
            doc.Add(nullString);

            for (int i = 0; i < personAss.sjzhy.Count; i++)
            {
                int index = i;
                var jzScore = scores?.thirdZhyScores?.Find(a => string.Equals(a.roleId, personAss.sjzhy[index].jzId));
                Paragraph paLevel3i;
                if (jzScore == null) paLevel3i = new Paragraph("  (" + (index + 1) + ") " + personAss.sjzhy[i].jzname, fontSub);
                else paLevel3i = new Paragraph("  (" + (index + 1) + ") " + personAss.sjzhy[i].jzname + "   得分：" + jzScore.jzZhyTotalScore, fontSub);
                paLevel3i.IndentationLeft = 20f;
                doc.Add(paLevel3i);
                doc.Add(nullString);

                PdfPTable commander3 = new PdfPTable(3);
                commander3.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 3, 1));
                if (jzScore == null) commander3.AddCell(MyCell($"确认载油量和载重信息", 3, 1));
                else commander3.AddCell(MyCell($"确认载油量和载重信息： {jzScore.qrzyl}分", 3, 1));
                commander3.AddCell(MyCell("燃油重量（千克）", 1, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].zyl.ToString(), 2, 1));
                commander3.AddCell(MyCell("可用载重（千克）", 1, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].zzl.ToString(), 2, 1));

                int aqfxnl = 60;
                int tqcznl = 40;

                if (jzScore == null) commander3.AddCell(MyCell($"完成任务区航线规划", 3, 1));
                else commander3.AddCell(MyCell($"完成任务区航线规划： {jzScore.rwqyhxgh}分", 3, 1));
                commander3.AddCell(MyCell("燃油不足报警次数（剩余燃油重量低于最大油量的 10% 千克）", 2, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].rybz.ToString(), 1, 1));
                commander3.AddCell(MyCell("错误着陆次数（着陆在未分配的补给点、备降点）", 2, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].cwzl.ToString(), 1, 1));

                if (jzScore == null) commander3.AddCell(MyCell($"向现场指挥员报告特情", 3, 1));
                else commander3.AddCell(MyCell($"向现场指挥员报告特情： {jzScore.xxczhybg}分", 3, 1));
                commander3.AddCell(MyCell("特情次数", 1, 1));
                commander3.AddCell(MyCell($"{(tqData.ContainsKey(1) ? tqData[1].Count : 0).ToString()}", 2, 1));
                commander3.AddCell(MyCell("报告次数", 1, 1));
                commander3.AddCell(MyCell($"{(tqData.ContainsKey(2) ? tqData[2].Count : 0).ToString()}", 2, 1));
                doc.Add(commander3);
                doc.Add(nullString);
            }


            Paragraph messageEval = new Paragraph("二、任务效能评估", fontSub);
            doc.Add(messageEval);
            doc.Add(nullString);

            double WaterMissonDegree = resultData.灭火任务完成度 * 100;
            if (Double.IsNaN(WaterMissonDegree) || Double.IsInfinity(WaterMissonDegree)) WaterMissonDegree = 0;

            double WaterEval = resultData.协同指挥效能 + reports;
            if (Double.IsNaN(WaterEval) || Double.IsInfinity(WaterEval)) WaterEval = 0;

            double ZongMisson = resultData.总体任务效率;
            if (Double.IsNaN(ZongMisson) || Double.IsInfinity(ZongMisson)) ZongMisson = 0;

            PdfPTable tableResult = new PdfPTable(4);
            tableResult.AddCell(MyCell($"任务效能总分 {WaterEval.ToString("0.00000")}", 4, 1));
            tableResult.AddCell(MyCell("灭火任务完成度", 2, 1));
            tableResult.AddCell(MyCell(WaterMissonDegree.ToString("0.00000") + " %", 2, 1));
            tableResult.AddCell(MyCell("灭火任务总时间效率", 2, 1));
            tableResult.AddCell(MyCell(resultData.灭火任务总时间效率.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell($"机组任务完成度", 4, 1));
            for (int i = 0; i < personAss.sjzhy.Count; i++)
            {
                tableResult.AddCell(MyCell(personAss.sjzhy[i].jzname, 2, 1));
                var rws = resultData.任务结束时各火场数据.FindAll(a => personAss.sjzhy[i].bindingZy.Contains(a.Id));
                double zongWcd = 0;
                rws.ForEach(a => zongWcd += a.Degree);
                tableResult.AddCell(MyCell((zongWcd / rws.Count * 100).ToString("0.00000") + "%", 2, 1));
            }

            tableResult.AddCell(MyCell($"任务信息", 4, 1));
            tableResult.AddCell(MyCell("灭火任务总时间(小时)", 2, 1));
            tableResult.AddCell(MyCell("总时间", 2, 1));
            tableResult.AddCell(MyCell("开始投水时刻", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.开始投水时刻, 2, 1));
            tableResult.AddCell(MyCell("投水总需求（千克）", 2, 1));
            tableResult.AddCell(MyCell(resultData.任务结束时过火面积对应的投水总需求.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时投水总量（千克）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时投水总量.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("总航程（公里）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.总航程.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("飞行架次", 2, 1));

            tableResult.AddCell(MyCell($"火场数据", 4, 1));
            tableResult.AddCell(MyCell("过火面积控制率", 2, 1));
            tableResult.AddCell(MyCell(resultData.过火面积控制率.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("初始总燃烧面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务初始燃烧面积.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时总过火面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时过火总面积.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时燃烧面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时燃烧面积.ToString("0.00"), 2, 1));

            #region 以前的任务效能，不用了

            // tableResult.AddCell(MyCell("协同指挥训练得分", 2, 1));
            // tableResult.AddCell(MyCell(WaterEval.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("总体任务效率", 2, 1));
            // tableResult.AddCell(MyCell(ZongMisson.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("单机任务效率", 2, 1));
            // tableResult.AddCell(MyCell(resultData.单机任务效率.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("灭火任务效率", 2, 1));
            // tableResult.AddCell(MyCell(resultData.灭火任务效率.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("任务总成本效率", 2, 1));
            // tableResult.AddCell(MyCell(resultData.任务总成本效率.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("过火面积控制率", 2, 1));
            // tableResult.AddCell(MyCell(resultData.过火面积控制率.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("开始投水时刻", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.开始投水时刻, 2, 1));
            // tableResult.AddCell(MyCell("投水总需求（千克）", 2, 1));
            // tableResult.AddCell(MyCell(resultData.任务结束时过火面积对应的投水总需求.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("任务结束时投水总量（千克）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.任务结束时投水总量.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("总航程（公里）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.总航程.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("飞行架次", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.直升机总架次.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("初始总燃烧面积（平方米）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.任务初始燃烧面积.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("任务结束时总过火面积（平方米）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.任务结束时过火总面积.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("任务结束时燃烧面积（平方米）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.任务结束时燃烧面积.ToString("0.00"), 2, 1));

            #endregion

            doc.Add(tableResult);
            doc.Add(nullString);


            Paragraph xlsj = new Paragraph("三、训练数据", fontSub);
            doc.Add(xlsj);
            doc.Add(nullString);

            Paragraph mesFire = new Paragraph("1.任务结束时各火场数据", fontSub);
            mesFire.IndentationLeft = 20f;
            doc.Add(mesFire);
            doc.Add(nullString);

            PdfPTable tableFire = new PdfPTable(4);
            tableFire.AddCell(MyCell("火场名称"));
            tableFire.AddCell(MyCell("投水总重量（千克）"));
            tableFire.AddCell(MyCell("投水需求（千克）"));
            tableFire.AddCell(MyCell("任务完成度"));
            foreach (FireData item in resultData.任务结束时各火场数据)
            {
                tableFire.AddCell(MyCell(item.Name));
                tableFire.AddCell(MyCell(item.WaterWeight.ToString("0.00")));
                tableFire.AddCell(MyCell(item.WaterNeed.ToString("0.00")));
                tableFire.AddCell(MyCell(item.Degree.ToString("0.00000")));
            }

            doc.Add(tableFire);
            doc.Add(nullString);

            foreach (KeyValuePair<HeliData, List<HeliSortieData>> item in resultData.机型架次数据)
            {
                Paragraph mesItem = new Paragraph(item.Key.Name, fontSub);
                mesItem.IndentationLeft = 20f;
                doc.Add(mesItem);
                doc.Add(nullString);

                double TimeWaterWeight = item.Key.单位时间单机投水重量;
                if (Double.IsNaN(TimeWaterWeight) || Double.IsInfinity(TimeWaterWeight)) TimeWaterWeight = 0;

                PdfPTable tableEffort = new PdfPTable(4);
                tableEffort.AddCell(MyCell("累计投水重量（千克）", 2, 1));
                tableEffort.AddCell(MyCell(item.Key.单机投水总重量.ToString("0.00"), 2, 1));
                tableEffort.AddCell(MyCell("单机任务成本", 2, 1));
                tableEffort.AddCell(MyCell(item.Key.IsCrash ? "已坠毁" : item.Key.单机任务成本.ToString("0.00"), 2, 1));
                tableEffort.AddCell(MyCell("单位时间内单机投水重量（千克）", 2, 1));
                tableEffort.AddCell(MyCell(TimeWaterWeight.ToString("0.00"), 2, 1));
                tableEffort.AddCell(MyCell("飞行架次", 2, 1));
                tableEffort.AddCell(MyCell(item.Value.Count.ToString(), 2, 1));
                tableEffort.AddCell(MyCell("单位架次投水重量（千克）", 4, 1));
                tableEffort.AddCell(MyCell("架次", 2, 1));
                tableEffort.AddCell(MyCell("投水重量（千克）", 2, 1));
                int sIndex = 0;
                foreach (HeliSortieData hsdItem in item.Value)
                {
                    sIndex++;
                    tableEffort.AddCell(MyCell(sIndex.ToString(), 2, 1));
                    tableEffort.AddCell(MyCell(hsdItem.WaterZongWeight.ToString("0.00"), 2, 1));
                }

                doc.Add(tableEffort);
                doc.Add(nullString);
            }

            doc.Add(table);
            doc.Add(nullString);

            Paragraph messageTrainData = new Paragraph("2.训练流程数据", fontSub);
            doc.Add(messageTrainData);
            doc.Add(nullString);
            for (int i = 0; i < trainData.Count; i++)
            {
                Paragraph mesItem = new Paragraph(trainData[i], fontText);
                mesItem.IndentationLeft = 30f;
                doc.Add(mesItem);
                doc.Add(nullString);
            }

            Paragraph messageWater = new Paragraph("3.投水数据", fontSub);
            doc.Add(messageWater);
            doc.Add(nullString);
            foreach (KeyValuePair<string, List<WaterMegData>> item in heliMegList)
            {
                Paragraph mesItem = new Paragraph(item.Key, fontSub);
                mesItem.IndentationLeft = 20f;
                doc.Add(mesItem);
                doc.Add(nullString);

                foreach (WaterMegData wmItem in item.Value)
                {
                    string ShowMeg = String.Format("第{0}架次     开始取水时间：{1}     结束投水时间：{2}     投水重量：{3}", wmItem.sortieIndex, wmItem.StartWaterTime, wmItem.EndWaterTime, wmItem.WaterWeight);
                    Paragraph mesItemWM = new Paragraph(ShowMeg, fontText);
                    mesItemWM.IndentationLeft = 30f;
                    doc.Add(mesItemWM);
                    doc.Add(nullString);
                }
            }

            doc.Close();
            writer.Close();
            Debug.LogError("生成PDF");
            System.Diagnostics.Process.Start("Explorer", dirPath.Replace('/', '\\'));
        }

        /// <summary>
        /// 物资和人员任务报告
        /// </summary>
        public void CreateRescueMissionReport(string reportId, string reportName, string userName, string Id, string Abstract, ResultMaterialPersonData resultData, ResultMaterialPersonOutData resultOutData,
            ResultRescueSystemData resultSysData, List<string> trainData, Dictionary<string, List<MaterialPersonMegData>> heliMegList, Dictionary<string, List<string>> usersEquips, Dictionary<string, List<string>> usersZiyuans, int reports,
            PersonAssessment_ResultFireWater personAss, ScoreStatistics scores,Dictionary<int, List<string>> tqData)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            //string fileName = DateTime.Now.ToLongDateString() + (Directory.GetFiles(dirPath).Length + 1);
            //string fileDate = DateTime.Now.ToLongDateString()+ DateTime.Now.Hour+"-"+ DateTime.Now.Minute+"-"+DateTime.Now.Second;
            reportPath = dirPath + "/" + userName + "(" + reportId + ").pdf";
            string fontPath = Application.dataPath + "/Font/wryh.ttf";
            Document doc = new Document();
            FileStream fi = new FileStream(reportPath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fi);
            doc.Open();

            Paragraph nullString = new Paragraph("  ", fontNull);
            Paragraph title = new Paragraph(reportName, fontTitle)
            {
                Alignment = Rectangle.ALIGN_CENTER
            };
            doc.Add(title);
            doc.Add(nullString);
            // string info = "用户名:" + userName + "                                    日期:" + DateTime.Now.ToLongDateString();
            // Paragraph date = new Paragraph(info, fontText)
            // {
            //     Alignment = Rectangle.ALIGN_CENTER
            // };
            // doc.Add(date);
            // doc.Add(nullString);

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = 480, //表格总宽度
                LockedWidth = true //锁定宽度
            };
            table.SetWidths(new int[] { 450, 450, 450, 450 });

            // Paragraph mesAbstract = new Paragraph(Abstract, fontText);
            // mesAbstract.FirstLineIndent = 28; //设置段落的首行缩进
            // doc.Add(mesAbstract);
            doc.Add(nullString);

            #region 救援力量不用了

            // Paragraph messageUserInfo = new Paragraph("1.救援力量配置", fontSub);
            // doc.Add(messageUserInfo);
            // doc.Add(nullString);
            // PdfPTable tableUserInfos = new PdfPTable(5);
            // tableUserInfos.AddCell(MyCell("指挥员", 1, 1));
            // tableUserInfos.AddCell(MyCell("装备", 2, 1));
            // tableUserInfos.AddCell(MyCell("资源", 2, 1));
            //
            // Dictionary<string, string[]> userInfos = new Dictionary<string, string[]>();
            // foreach (var equip in usersEquips)
            // {
            //     if (!userInfos.ContainsKey(equip.Key)) userInfos.Add(equip.Key, new string[2]);
            //     for (int i = 0; i < equip.Value.Count; i++)
            //     {
            //         userInfos[equip.Key][0] += equip.Value[i] + '、';
            //     }
            //
            //     if (!string.IsNullOrEmpty(userInfos[equip.Key][0]) && userInfos[equip.Key][0].Length > 1)
            //         userInfos[equip.Key][0] = userInfos[equip.Key][0].Remove(userInfos[equip.Key][0].Length - 1);
            // }
            //
            // foreach (var ziyuan in usersZiyuans)
            // {
            //     if (!userInfos.ContainsKey(ziyuan.Key)) userInfos.Add(ziyuan.Key, new string[2]);
            //     for (int i = 0; i < ziyuan.Value.Count; i++)
            //     {
            //         userInfos[ziyuan.Key][1] += ziyuan.Value[i] + '、';
            //     }
            //
            //     if (!string.IsNullOrEmpty(userInfos[ziyuan.Key][1]) && userInfos[ziyuan.Key][1].Length > 1)
            //         userInfos[ziyuan.Key][1] = userInfos[ziyuan.Key][1].Remove(userInfos[ziyuan.Key][1].Length - 1);
            // }
            //
            // foreach (var item in userInfos)
            // {
            //     tableUserInfos.AddCell(MyCell(item.Key, 1, 5));
            //     tableUserInfos.AddCell(MyCell(item.Value[0], 2, 5));
            //     tableUserInfos.AddCell(MyCell(item.Value[1], 2, 5));
            // }
            //
            // doc.Add(tableUserInfos);
            // doc.Add(nullString);

            #endregion

            Paragraph messagePersonAssessment = new Paragraph("一、岗位职责能力评估", fontSub);
            doc.Add(messagePersonAssessment);
            doc.Add(nullString);
            Paragraph paLevel1;
            if (scores == null) paLevel1 = new Paragraph($"1.一级指挥员", fontSub);
            else paLevel1 = new Paragraph($"1.一级指挥员    得分{scores.firstZhyTotalScore}", fontSub);
            paLevel1.IndentationLeft = 20f;
            doc.Add(paLevel1);
            doc.Add(nullString);
            //数据：personAss

            //一级指挥员
            PdfPTable commander1 = new PdfPTable(6);

            commander1.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 6, 1));
            if (scores == null) commander1.AddCell(MyCell($"确认灾情信息", 6, 1));
            else commander1.AddCell(MyCell($"确认灾情信息: {scores.qrzqxx}分", 6, 1));
            commander1.AddCell(MyCell("灾害类型", 1, 1));
            commander1.AddCell(MyCell("灾情规模", 1, 1));
            commander1.AddCell(MyCell("物资投放重量（千克）", 1, 1));
            commander1.AddCell(MyCell("待转运人数", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            //值
            commander1.AddCell(MyCell(personAss.yjzhy.zhlx, 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.zqgm, 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.wztfzl.ToString(), 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.dzyrs.ToString(), 1, 1));
            commander1.AddCell(MyCell("", 1, 1));
            commander1.AddCell(MyCell("", 1, 1));

            if (scores == null) commander1.AddCell(MyCell($"确认出动装备信息", 6, 1));
            else commander1.AddCell(MyCell($"确认出动装备信息： {scores.cdzbxxqr}分", 6, 1));
            commander1.AddCell(MyCell("机型", 2, 1));
            commander1.AddCell(MyCell("编号", 2, 1));
            commander1.AddCell(MyCell("数量", 2, 1));
            //值
            for (int i = 0; i < personAss.yjzhy.cdjyll.Count; i++)
            {
                int index = i;
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jx, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].bh, 2, 1));
                commander1.AddCell(MyCell("1", 2, 1));
            }

            if (scores == null) commander1.AddCell(MyCell($"确认出动人员信息", 6, 1));
            else commander1.AddCell(MyCell($"确认出动人员信息： {scores.cdryxxqr}分", 6, 1));
            commander1.AddCell(MyCell("机型编号", 2, 1));
            commander1.AddCell(MyCell("机组", 2, 1));
            commander1.AddCell(MyCell("保障组", 2, 1));
            //值
            for (int i = 0; i < personAss.yjzhy.cdjyll.Count; i++)
            {
                int index = i;
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jx, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].jz, 2, 1));
                commander1.AddCell(MyCell(personAss.yjzhy.cdjyll[index].bzz, 2, 1));
            }

            if (scores == null) commander1.AddCell(MyCell($"申报转场航线", 6, 1));
            else commander1.AddCell(MyCell($"申报转场航线： {scores.zchxsb}分", 6, 1));
            commander1.AddCell(MyCell("航线信息", 1, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.hxgh, 5, 1));

            if (scores == null) commander1.AddCell(MyCell($"下达任务", 6, 1));
            else commander1.AddCell(MyCell($"下达任务： {scores.xdrw}分", 6, 1));
            commander1.AddCell(MyCell("任务简令", 6, 1));
            commander1.AddCell(MyCell(personAss.yjzhy.rwjl, 6, 2));

            doc.Add(commander1);
            doc.Add(nullString);


            Paragraph paLevel2;
            if (scores == null) paLevel2 = new Paragraph($"2.二级指挥员", fontSub);
            else paLevel2 = new Paragraph($"2.二级指挥员    得分:{scores.secondZhyTotalScore}", fontSub);
            paLevel2.IndentationLeft = 20f;
            doc.Add(paLevel2);
            doc.Add(nullString);

            PdfPTable commander2 = new PdfPTable(6);
            commander2.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 6, 1));
            if (scores == null) commander2.AddCell(MyCell($"领受任务", 6, 1));
            else commander2.AddCell(MyCell($"领受任务： {scores.lsrw}分", 6, 1));
            if (scores == null) commander2.AddCell(MyCell($"确认出动装备信息", 6, 1));
            else commander2.AddCell(MyCell($"确认出动装备信息： {scores.qrzbztxx}分", 6, 1));
            commander2.AddCell(MyCell("机组信息", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("机型", 1, 1));
            commander2.AddCell(MyCell("装载设备", 1, 1));
            commander2.AddCell(MyCell("载油量（千克）", 1, 1));
            commander2.AddCell(MyCell("可用载重（千克）", 1, 1));
            commander2.AddCell(MyCell("地面维护时间间隔（小 时）", 1, 1));

            for (int i = 0; i < personAss.ejzhy.jzxx.Count; i++)
            {
                int index = i;
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].jzName, 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].jx, 1, 1));
                commander2.AddCell(MyCell(string.Join(" ", personAss.ejzhy.jzxx[index].zzsb), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].zyl.ToString(), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].zzl.ToString(), 1, 1));
                commander2.AddCell(MyCell(personAss.ejzhy.jzxx[index].dmwhTime.ToString(), 1, 1));
            }

            if (scores == null) commander2.AddCell(MyCell($"完成任务分配并下达任务", 6, 1));
            else commander2.AddCell(MyCell($"完成任务分配并下达任务： {scores.fprwbxdrw}分", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("受灾点", 1, 1));
            commander2.AddCell(MyCell("安置点", 1, 1));
            commander2.AddCell(MyCell("医院", 1, 1));
            commander2.AddCell(MyCell("补给站", 1, 1));
            commander2.AddCell(MyCell("", 1, 1));

            for (int i = 0; i < personAss.ejzhy.rwfp.Count; i++)
            {
                int index = i;
                commander2.AddCell(MyCell(personAss.ejzhy.rwfp[index].jzName, 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].szd), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].azd), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].yy), 1, 1));
                commander2.AddCell(MyCell(string.Join("、", personAss.ejzhy.rwfp[index].bjd), 1, 1));
                commander2.AddCell(MyCell("", 1, 1));
            }

            if (scores == null) commander2.AddCell(MyCell($"确认机长的特情处置报告", 6, 1));
            else commander2.AddCell(MyCell($"确认机长的特情处置报告： {scores.qrtqczbg}分", 6, 1));
            commander2.AddCell(MyCell("报告次数", 3, 1));
            commander2.AddCell(MyCell($"{(tqData.ContainsKey(2) ? tqData[2].Count : 0).ToString()}", 3, 1));
            commander2.AddCell(MyCell("确认次数", 3, 1));
            commander2.AddCell(MyCell($"{(tqData.ContainsKey(3) ? tqData[3].Count : 0).ToString()}", 3, 1));

            doc.Add(commander2);
            doc.Add(nullString);

            Paragraph paLevel3 = new Paragraph($"3.三级指挥员", fontSub);
            paLevel3.IndentationLeft = 20f;
            doc.Add(paLevel3);
            doc.Add(nullString);

            for (int i = 0; i < personAss.sjzhy.Count; i++)
            {
                int index = i;
                var jzScore = scores?.thirdZhyScores?.Find(a => string.Equals(a.roleId, personAss.sjzhy[index].jzId));
                Paragraph paLevel3i;
                if (jzScore == null) paLevel3i = new Paragraph("  (" + (index + 1) + ") " + personAss.sjzhy[i].jzname, fontSub);
                else paLevel3i = new Paragraph("  (" + (index + 1) + ") " + personAss.sjzhy[i].jzname + "   得分：" + jzScore.jzZhyTotalScore, fontSub);

                paLevel3i.IndentationLeft = 20f;
                doc.Add(paLevel3i);
                doc.Add(nullString);

                PdfPTable commander3 = new PdfPTable(3);
                commander3.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 3, 1));
                if(jzScore==null)commander3.AddCell(MyCell($"确认载油量和载重信息", 3, 1));
                else commander3.AddCell(MyCell($"确认载油量和载重信息： {jzScore.qrzyl}分", 3, 1));
                commander3.AddCell(MyCell("燃油重量（千克）", 1, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].zyl.ToString(), 2, 1));
                commander3.AddCell(MyCell("可用载重（千克）", 1, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].zzl.ToString(), 2, 1));

                int aqfxnl = 60;
                int tqcznl = 40;

                if(jzScore==null)commander3.AddCell(MyCell($"完成任务区航线规划", 3, 1));
                else commander3.AddCell(MyCell($"完成任务区航线规划： {jzScore.rwqyhxgh}分", 3, 1));
                commander3.AddCell(MyCell("燃油不足报警次数（剩余燃油重量低于最大油量的 10% 千克）", 2, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].rybz.ToString(), 1, 1));
                commander3.AddCell(MyCell("错误着陆次数（着陆在未分配的补给点、备降点）", 2, 1));
                commander3.AddCell(MyCell(personAss.sjzhy[index].cwzl.ToString(), 1, 1));

                if(jzScore==null)commander3.AddCell(MyCell($"向现场指挥员报告特情", 3, 1));
                else commander3.AddCell(MyCell($"向现场指挥员报告特情： {jzScore.xxczhybg}分", 3, 1));
                commander3.AddCell(MyCell("特情次数", 1, 1));
                commander3.AddCell(MyCell($"{(tqData.ContainsKey(1) ? tqData[1].Count : 0).ToString()}", 2, 1));
                commander3.AddCell(MyCell("报告次数", 1, 1));
                commander3.AddCell(MyCell($"{(tqData.ContainsKey(2) ? tqData[1].Count : 0).ToString()}", 2, 1));
                doc.Add(commander3);
                doc.Add(nullString);
            }

            Paragraph messageEval = new Paragraph("二、任务效能评估", fontSub);
            doc.Add(messageEval);
            doc.Add(nullString);

            double RescueEval = resultData.协同指挥效能 + reports;
            if (Double.IsNaN(RescueEval) || Double.IsInfinity(RescueEval)) RescueEval = 0;

            double PersonDegree = resultData.人员转运任务完成度 * 100;
            if (Double.IsNaN(PersonDegree) || Double.IsInfinity(PersonDegree)) PersonDegree = 0;

            double MaterialDegree = resultData.物资任务完成度 * 100;
            if (Double.IsNaN(MaterialDegree) || Double.IsInfinity(MaterialDegree)) MaterialDegree = 0;

            double PersonEfficiency = resultData.人员转运单机任务效率;
            if (Double.IsNaN(PersonEfficiency) || Double.IsInfinity(PersonEfficiency)) PersonEfficiency = 0;

            double MaterialEfficiency = resultData.物资投放单机任务效率;
            if (Double.IsNaN(MaterialEfficiency) || Double.IsInfinity(MaterialEfficiency)) MaterialEfficiency = 0;

            double PersonZongEfficiency = resultData.人员转运总体任务效率;
            if (Double.IsNaN(PersonZongEfficiency) || Double.IsInfinity(PersonZongEfficiency)) PersonZongEfficiency = 0;

            double MaterialZongEfficiency = resultData.物资投放总体任务效率;
            if (Double.IsNaN(MaterialZongEfficiency) || Double.IsInfinity(MaterialZongEfficiency)) MaterialZongEfficiency = 0;

            double PersonTimeEfficiency = resultData.人员转运任务时间效率;
            if (Double.IsNaN(PersonTimeEfficiency) || Double.IsInfinity(PersonTimeEfficiency)) PersonTimeEfficiency = 0;
            double MaterialTimeEfficiency = resultData.物资投放任务时间效率;
            if (Double.IsNaN(MaterialTimeEfficiency) || Double.IsInfinity(MaterialTimeEfficiency)) MaterialTimeEfficiency = 0;
            double PersonTotalCostEfficiency = resultData.人员转运任务总成本效率;
            if (Double.IsNaN(PersonTotalCostEfficiency) || Double.IsInfinity(PersonTotalCostEfficiency)) PersonTotalCostEfficiency = 0;
            double MaterialTotalCostEfficiency = resultData.物资投放任务总成本效率;
            if (Double.IsNaN(MaterialTotalCostEfficiency) || Double.IsInfinity(MaterialTotalCostEfficiency)) MaterialTotalCostEfficiency = 0;

            PdfPTable tableResult = new PdfPTable(4);
            tableResult.AddCell(MyCell($"任务效能总分 {RescueEval.ToString("0.00000")}", 4, 1));
            tableResult.AddCell(MyCell($"人员转运总体任务效率    {PersonZongEfficiency.ToString("0.00000")}", 2, 1));
            tableResult.AddCell(MyCell($"物资投放总体任务效率    {MaterialZongEfficiency.ToString("0.00000")}", 2, 1));
            tableResult.AddCell(MyCell($"人员转运总体任务效率", 4, 1));
            tableResult.AddCell(MyCell("人员转运任务完成度", 2, 1));
            tableResult.AddCell(MyCell(PersonDegree.ToString("0.00000") + " %", 2, 1));
            tableResult.AddCell(MyCell("人员转运任务时间效率", 2, 1));
            tableResult.AddCell(MyCell(PersonTimeEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("物资投放总体任务效率", 4, 1));
            tableResult.AddCell(MyCell("物资投放任务完成度", 2, 1));
            tableResult.AddCell(MyCell(MaterialDegree.ToString("0.00000") + " %", 2, 1));
            tableResult.AddCell(MyCell("物资投放任务时间效率", 2, 1));
            tableResult.AddCell(MyCell(MaterialTimeEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("机组任务完成度", 4, 1));
            for (int i = 0; i < personAss.sjzhy.Count; i++)
            {
                tableResult.AddCell(MyCell(personAss.sjzhy[i].jzname, 4, 1));
                tableResult.AddCell(MyCell("物资投放", 2, 1));
                var rws = resultOutData.任务结束时各安置点数据.FindAll(a => personAss.sjzhy[i].bindingZy.Contains(a.Id));
                double zongWcd = 0;
                rws.ForEach(a => zongWcd += a.MaterialDegree);
                tableResult.AddCell(MyCell((zongWcd / rws.Count * 100).ToString("0.00000") + "%", 2, 1));
                tableResult.AddCell(MyCell("人员转运", 2, 1));
                var rws2 = resultOutData.任务结束时各灾区数据.FindAll(a => personAss.sjzhy[i].bindingZy.Contains(a.Id));
                double zongWcd2 = 0;
                rws2.ForEach(a => zongWcd2 += a.PersonDegree);
                tableResult.AddCell(MyCell((zongWcd2 / rws2.Count * 100).ToString("0.00000") + "%", 2, 1));
            }

            tableResult.AddCell(MyCell("任务信息", 4, 1));
            tableResult.AddCell(MyCell("首批救援物资到达安置点时刻", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.首批救援物资到达安置点时刻, 2, 1));
            tableResult.AddCell(MyCell("受灾需转运总人数", 2, 1));
            tableResult.AddCell(MyCell(resultSysData.受灾需转运总人数.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时转运总人数", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时转运总人数.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时对应的物资投放总需求（千克）", 2, 1));
            tableResult.AddCell(MyCell(resultData.任务结束时对应的物资投放总需求.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("总航程（公里）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.总航程.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("所有飞机总架次", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.所有飞机总架次.ToString("0.00"), 2, 1));

            #region 老效能模板，不用了

            // tableResult.AddCell(MyCell("协同指挥训练得分", 2, 1));
            // tableResult.AddCell(MyCell(RescueEval.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("人员转运任务完成度", 2, 1));
            // tableResult.AddCell(MyCell(PersonDegree.ToString("0.00000") + " %", 2, 1));
            // tableResult.AddCell(MyCell("物资投放任务完成度", 2, 1));
            // tableResult.AddCell(MyCell(MaterialDegree.ToString("0.00000") + " %", 2, 1));
            // tableResult.AddCell(MyCell("首批救援物资到达安置点时刻", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.首批救援物资到达安置点时刻, 2, 1));
            // tableResult.AddCell(MyCell("受灾需转运总人数", 2, 1));
            // tableResult.AddCell(MyCell(resultSysData.受灾需转运总人数.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("任务结束时转运总人数", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.任务结束时转运总人数.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("任务结束时对应的物资投放总需求（千克）", 2, 1));
            // tableResult.AddCell(MyCell(resultData.任务结束时对应的物资投放总需求.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("总航程（公里）", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.总航程.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("所有飞机总架次", 2, 1));
            // tableResult.AddCell(MyCell(resultOutData.所有飞机总架次.ToString("0.00"), 2, 1));
            // tableResult.AddCell(MyCell("人员转运单机任务效率", 2, 1));
            // tableResult.AddCell(MyCell(PersonEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("物资投放单机任务效率", 2, 1));
            // tableResult.AddCell(MyCell(MaterialEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("人员转运总体任务效率", 2, 1));
            // tableResult.AddCell(MyCell(PersonZongEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("物资投放总体任务效率", 2, 1));
            // tableResult.AddCell(MyCell(MaterialZongEfficiency.ToString("0.00000"), 2, 1));
            //
            // tableResult.AddCell(MyCell("人员转运任务时间效率", 2, 1));
            // tableResult.AddCell(MyCell(PersonTimeEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("物资投放任务时间效率", 2, 1));
            // tableResult.AddCell(MyCell(MaterialTimeEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("人员转运任务总成本效率", 2, 1));
            // tableResult.AddCell(MyCell(PersonTotalCostEfficiency.ToString("0.00000"), 2, 1));
            // tableResult.AddCell(MyCell("物资投放任务总成本效率", 2, 1));
            // tableResult.AddCell(MyCell(MaterialTotalCostEfficiency.ToString("0.00000"), 2, 1));

            #endregion

            doc.Add(tableResult);
            doc.Add(nullString);


            Paragraph xlsj = new Paragraph("三、训练数据", fontSub);
            doc.Add(xlsj);
            doc.Add(nullString);

            Paragraph mesFire = new Paragraph("1.任务结束时各安置点数据", fontSub);
            mesFire.IndentationLeft = 20f;
            doc.Add(mesFire);
            doc.Add(nullString);

            PdfPTable tableFire = new PdfPTable(5);
            tableFire.AddCell(MyCell("安置点名称"));
            tableFire.AddCell(MyCell("转运人数"));
            tableFire.AddCell(MyCell("物资投放需求重量（千克）"));
            tableFire.AddCell(MyCell("物资投放重量（千克）"));
            tableFire.AddCell(MyCell("物资投放任务完成度"));
            foreach (MaterialData item in resultData.任务结束时各安置点数据)
            {
                bool isHos = string.Equals(item.Id, "-1");
                tableFire.AddCell(MyCell(item.Name));
                tableFire.AddCell(MyCell(item.PersonCount.ToString("0.00")));
                tableFire.AddCell(MyCell(isHos ? "--" : item.PersonMaterialNeed.ToString("0.00")));
                tableFire.AddCell(MyCell(isHos ? "--" : item.MaterialWeight.ToString("0.00")));
                tableFire.AddCell(MyCell(isHos ? "--" : item.MaterialDegree.ToString("0.00000")));
            }

            doc.Add(tableFire);
            doc.Add(nullString);

            foreach (KeyValuePair<HeliData, List<HeliSortieData>> item in resultData.机型架次数据)
            {
                Paragraph mesItem = new Paragraph(item.Key.Name, fontSub);
                mesItem.IndentationLeft = 20f;
                doc.Add(mesItem);
                doc.Add(nullString);

                PdfPTable tableEffort = new PdfPTable(6);
                tableEffort.AddCell(MyCell("累计转运人数", 3, 1));
                tableEffort.AddCell(MyCell(item.Key.累计转运人数.ToString("0.00"), 3, 1));
                tableEffort.AddCell(MyCell("累计投放物资重量（千克）", 3, 1));
                tableEffort.AddCell(MyCell(item.Key.累计投放物资重量.ToString("0.00"), 3, 1));
                tableEffort.AddCell(MyCell("单机任务成本", 3, 1));
                tableEffort.AddCell(MyCell(item.Key.IsCrash ? "已坠毁" : item.Key.单机任务成本.ToString("0.00"), 3, 1));
                tableEffort.AddCell(MyCell("飞行架次", 3, 1));
                tableEffort.AddCell(MyCell(item.Value.Count.ToString(), 3, 1));
                tableEffort.AddCell(MyCell("单位架次数据", 6, 1));
                tableEffort.AddCell(MyCell("架次", 2, 1));
                tableEffort.AddCell(MyCell("转运人数", 2, 1));
                tableEffort.AddCell(MyCell("投放物资重量（千克）", 2, 1));
                int sIndex = 0;
                foreach (HeliSortieData hsdItem in item.Value)
                {
                    sIndex++;
                    tableEffort.AddCell(MyCell(sIndex.ToString(), 2, 1));
                    tableEffort.AddCell(MyCell(hsdItem.PersonCount.ToString("0.00"), 2, 1));
                    tableEffort.AddCell(MyCell(hsdItem.MaterialWeight.ToString("0.00"), 2, 1));
                }

                doc.Add(tableEffort);
                doc.Add(nullString);
            }

            Paragraph messageTrainData = new Paragraph("2.训练流程数据", fontSub);
            doc.Add(messageTrainData);
            doc.Add(nullString);
            for (int i = 0; i < trainData.Count; i++)
            {
                Paragraph mesItem = new Paragraph(trainData[i], fontText);
                mesItem.IndentationLeft = 30f;
                doc.Add(mesItem);
                doc.Add(nullString);
            }

            Paragraph messageWater = new Paragraph("3.救援数据", fontSub);
            doc.Add(messageWater);
            doc.Add(nullString);
            foreach (KeyValuePair<string, List<MaterialPersonMegData>> item in heliMegList)
            {
                Paragraph mesItem = new Paragraph(item.Key, fontSub);
                mesItem.IndentationLeft = 20f;
                doc.Add(mesItem);
                doc.Add(nullString);

                foreach (MaterialPersonMegData mpmItem in item.Value)
                {
                    string ShowMeg = String.Format("第{0}架次     起飞时间：{1}     物资投放时间：{2}     投放重量：{3}     结束任务时间：{4}     转运人数：{5}", mpmItem.sortieIndex, mpmItem.TakeOffTime, mpmItem.MaterialTime, mpmItem.MaterialWeight,
                        mpmItem.EndMissionTime, mpmItem.PersonCount);
                    Paragraph mesItemWM = new Paragraph(ShowMeg, fontText);
                    mesItemWM.IndentationLeft = 30f;
                    doc.Add(mesItemWM);
                    doc.Add(nullString);
                }
            }

            doc.Add(table);
            doc.Add(nullString);
            doc.Close();
            writer.Close();
            Debug.LogError("生成PDF");
            System.Diagnostics.Process.Start("Explorer", dirPath.Replace('/', '\\'));
        }

        private PdfPCell MyCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText))
            {
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE,
                MinimumHeight = 35 //设置行高
            };
            return cell;
        }

        private PdfPCell MyCell(string text, int col, int row)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText))
            {
                Colspan = col,
                Rowspan = row,
                MinimumHeight = 35, //设置行高

                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE
            };
            return cell;
        }

        private PdfPCell MyCell1(string text, int col, int row)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText))
            {
                Colspan = col,
                Rowspan = row,
                MinimumHeight = 35, //设置行高
                BackgroundColor = new BaseColor(0.5f, 0.5f, 0.5f),
                HorizontalAlignment = PdfPCell.ALIGN_CENTER,
                VerticalAlignment = PdfPCell.ALIGN_MIDDLE
            };
            return cell;
        }
    }
}