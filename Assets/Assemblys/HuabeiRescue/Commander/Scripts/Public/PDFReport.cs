using DM.IFS;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
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
            Dictionary<string, List<WaterMegData>> heliMegList)
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
            string info = "用户名:" + userName + "                                    日期:" + DateTime.Now.ToLongDateString();
            Paragraph date = new Paragraph(info, fontText)
            {
                Alignment = Rectangle.ALIGN_CENTER
            };
            doc.Add(date);
            doc.Add(nullString);

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = 480, //表格总宽度
                LockedWidth = true //锁定宽度
            };
            table.SetWidths(new int[] { 450, 450, 450, 450 });

            Paragraph mesAbstract = new Paragraph(Abstract, fontText);
            mesAbstract.FirstLineIndent = 28; //设置段落的首行缩进
            doc.Add(mesAbstract);
            doc.Add(nullString);

            Paragraph messageTrainData = new Paragraph("1.训练数据", fontSub);
            doc.Add(messageTrainData);
            doc.Add(nullString);
            for (int i = 0; i < trainData.Count; i++)
            {
                Paragraph mesItem = new Paragraph(trainData[i], fontText);
                mesItem.IndentationLeft = 30f;
                doc.Add(mesItem);
                doc.Add(nullString);
            }

            Paragraph messageWater = new Paragraph("2.投水数据", fontSub);
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

            Paragraph messageEval = new Paragraph("3.评估结果", fontSub);
            doc.Add(messageEval);
            doc.Add(nullString);

            double WaterMissonDegree = resultData.灭火任务完成度 * 100;
            if (Double.IsNaN(WaterMissonDegree) || Double.IsInfinity(WaterMissonDegree)) WaterMissonDegree = 0;

            double WaterEval = resultData.协同指挥效能;
            if (Double.IsNaN(WaterEval) || Double.IsInfinity(WaterEval)) WaterEval = 0;

            double ZongMisson = resultData.总体任务效率;
            if (Double.IsNaN(ZongMisson) || Double.IsInfinity(ZongMisson)) ZongMisson = 0;

            PdfPTable tableResult = new PdfPTable(4);
            tableResult.AddCell(MyCell("任务完成度", 2, 1));
            tableResult.AddCell(MyCell(WaterMissonDegree.ToString("0.00000") + " %", 2, 1));
            tableResult.AddCell(MyCell("协同指挥训练得分", 2, 1));
            tableResult.AddCell(MyCell(WaterEval.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("总体任务效率", 2, 1));
            tableResult.AddCell(MyCell(ZongMisson.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("单机任务效率", 2, 1));
            tableResult.AddCell(MyCell(resultData.单机任务效率.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("灭火任务效率", 2, 1));
            tableResult.AddCell(MyCell(resultData.灭火任务效率.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("灭火任务总时间效率", 2, 1));
            tableResult.AddCell(MyCell(resultData.灭火任务总时间效率.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("任务总成本效率", 2, 1));
            tableResult.AddCell(MyCell(resultData.任务总成本效率.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("过火面积控制率", 2, 1));
            tableResult.AddCell(MyCell(resultData.过火面积控制率.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("开始投水时刻", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.开始投水时刻, 2, 1));
            tableResult.AddCell(MyCell("投水总需求（千克）", 2, 1));
            tableResult.AddCell(MyCell(resultData.任务结束时过火面积对应的投水总需求.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时投水总量（千克）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时投水总量.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("总航程（公里）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.总航程.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("飞行架次", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.直升机总架次.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("初始总燃烧面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务初始燃烧面积.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时总过火面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时过火总面积.ToString("0.00"), 2, 1));
            tableResult.AddCell(MyCell("任务结束时燃烧面积（平方米）", 2, 1));
            tableResult.AddCell(MyCell(resultOutData.任务结束时燃烧面积.ToString("0.00"), 2, 1));
            doc.Add(tableResult);
            doc.Add(nullString);

            Paragraph mesFire = new Paragraph("任务结束时各火场数据", fontSub);
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
            doc.Close();
            writer.Close();
            Debug.LogError("生成PDF");
            System.Diagnostics.Process.Start("Explorer", dirPath.Replace('/', '\\'));
        }

        /// <summary>
        /// 物资和人员任务报告
        /// </summary>
        public void CreateRescueMissionReport(string reportId, string reportName, string userName, string Id, string Abstract, ResultMaterialPersonData resultData, ResultMaterialPersonOutData resultOutData,
            ResultRescueSystemData resultSysData, List<string> trainData, Dictionary<string, List<MaterialPersonMegData>> heliMegList)
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
            string info = "用户名:" + userName + "                                    日期:" + DateTime.Now.ToLongDateString();
            Paragraph date = new Paragraph(info, fontText)
            {
                Alignment = Rectangle.ALIGN_CENTER
            };
            doc.Add(date);
            doc.Add(nullString);

            PdfPTable table = new PdfPTable(4)
            {
                TotalWidth = 480, //表格总宽度
                LockedWidth = true //锁定宽度
            };
            table.SetWidths(new int[] { 450, 450, 450, 450 });

            Paragraph mesAbstract = new Paragraph(Abstract, fontText);
            mesAbstract.FirstLineIndent = 28; //设置段落的首行缩进
            doc.Add(mesAbstract);
            doc.Add(nullString);

            Paragraph messageTrainData = new Paragraph("1.训练数据", fontSub);
            doc.Add(messageTrainData);
            doc.Add(nullString);
            for (int i = 0; i < trainData.Count; i++)
            {
                Paragraph mesItem = new Paragraph(trainData[i], fontText);
                mesItem.IndentationLeft = 30f;
                doc.Add(mesItem);
                doc.Add(nullString);
            }

            Paragraph messageWater = new Paragraph("2.救援数据", fontSub);
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

            Paragraph messageEval = new Paragraph("3.评估结果", fontSub);
            doc.Add(messageEval);
            doc.Add(nullString);

            double RescueEval = resultData.协同指挥效能;
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
            tableResult.AddCell(MyCell("协同指挥训练得分", 2, 1));
            tableResult.AddCell(MyCell(RescueEval.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("人员转运任务完成度", 2, 1));
            tableResult.AddCell(MyCell(PersonDegree.ToString("0.00000") + " %", 2, 1));
            tableResult.AddCell(MyCell("物资投放任务完成度", 2, 1));
            tableResult.AddCell(MyCell(MaterialDegree.ToString("0.00000") + " %", 2, 1));
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
            tableResult.AddCell(MyCell("人员转运单机任务效率", 2, 1));
            tableResult.AddCell(MyCell(PersonEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("物资投放单机任务效率", 2, 1));
            tableResult.AddCell(MyCell(MaterialEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("人员转运总体任务效率", 2, 1));
            tableResult.AddCell(MyCell(PersonZongEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("物资投放总体任务效率", 2, 1));
            tableResult.AddCell(MyCell(MaterialZongEfficiency.ToString("0.00000"), 2, 1));

            tableResult.AddCell(MyCell("人员转运任务时间效率", 2, 1));
            tableResult.AddCell(MyCell(PersonTimeEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("物资投放任务时间效率", 2, 1));
            tableResult.AddCell(MyCell(MaterialTimeEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("人员转运任务总成本效率", 2, 1));
            tableResult.AddCell(MyCell(PersonTotalCostEfficiency.ToString("0.00000"), 2, 1));
            tableResult.AddCell(MyCell("物资投放任务总成本效率", 2, 1));
            tableResult.AddCell(MyCell(MaterialTotalCostEfficiency.ToString("0.00000"), 2, 1));

            doc.Add(tableResult);
            doc.Add(nullString);

            Paragraph mesFire = new Paragraph("任务结束时各安置点数据", fontSub);
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
                tableFire.AddCell(MyCell(item.Name));
                tableFire.AddCell(MyCell(item.PersonCount.ToString("0.00")));
                tableFire.AddCell(MyCell(item.PersonMaterialNeed.ToString("0.00")));
                tableFire.AddCell(MyCell(item.MaterialWeight.ToString("0.00")));
                tableFire.AddCell(MyCell(item.MaterialDegree.ToString("0.00000")));
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