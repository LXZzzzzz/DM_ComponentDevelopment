using DM.IFS;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using ToolsLibrary;
using UnityEngine;

namespace 导教端_WRJ
{
    public class PDFReport
    {
        public string dirPath;
        public string reportPath;
        iTextSharp.text.Font fontTitle, fontSub, fontTextBold, fontText, fontTable, fontcellB, fontNull;

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
            fontTable = new iTextSharp.text.Font(font, 10);
            fontNull = new iTextSharp.text.Font(font, 5);
        }
        public void CreateReport(string reportId,string reportName,string userName,string userId,List<string> infos,bool isFormat1=true)
        {
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            //string fileName = DateTime.Now.ToLongDateString() + (Directory.GetFiles(dirPath).Length + 1);
            //string fileDate = DateTime.Now.ToLongDateString()+ DateTime.Now.Hour+"-"+ DateTime.Now.Minute+"-"+DateTime.Now.Second;
            reportPath = dirPath + "/" + userName+"("+ reportId + ").pdf";
            string fontPath = Application.dataPath + "/Font/wryh.ttf";
            Document doc = new Document();
            FileStream fi = new FileStream(reportPath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fi);
            doc.Open();

            Paragraph nullString = new Paragraph("  ", fontNull);
            Paragraph title = new Paragraph(reportName, fontTitle);
            title.Alignment = Rectangle.ALIGN_CENTER;
            doc.Add(title); doc.Add(nullString);
            string info = "姓名:" + userName + "         学号:" + userId + "                     日期:" + DateTime.Now.ToLongDateString();
            Paragraph date = new Paragraph(info, fontText);
            date.Alignment = Rectangle.ALIGN_CENTER;
            doc.Add(date); doc.Add(nullString);

            PdfPTable table = new PdfPTable(1);
            table.TotalWidth = 480;//表格总宽度
            table.LockedWidth = true;//锁定宽度
            table.SetWidths(new int[] { 450 });

            enterFormat(table,infos);
            
            doc.Add(table); doc.Add(nullString);
            doc.Close();
            writer.Close();
            Debug.LogError("生成PDF");
            //sender.RunSend(SendType.System,null,(int)MainSystemType.UI,"")
        }


        private void enterFormat(PdfPTable table,List<string> infos)
        {
            Debug.LogError("指令数据有几条"+infos.Count);
            for (int i = 0; i < infos.Count; i++)
            {
                table.AddCell(new PdfPCell(new Phrase(infos[i], fontTable)));
            }
        }
        
        private void HelicopterFormat(PdfPTable table, MonoBehaviour mono,bool isFirstReport=true)
        {
            table.AddCell(MyCell(mono.name+"性能数据", 4, 1));
            table.AddCell(MyCell("最大航程"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsDistance")).ToString()));
            table.AddCell(MyCell("载荷重量"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsLoad")).ToString()));
            table.AddCell(MyCell("最大飞行速度"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsSpeed")).ToString()));
            table.AddCell(MyCell("空空导弹携带数量"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsAirMissileCount")).ToString()));
            if (!isFirstReport)
            {
                table.AddCell(MyCell("空面导弹携带数量", 2, 1));
                table.AddCell(MyCell(((int)mono.HGetField("SettingsLandMissileCount")).ToString(), 2, 1));
            }       
            table.AddCell(MyCell("携带干扰弹数量"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsJamBombCount")).ToString()));
            table.AddCell(MyCell("机炮子弹数目"));
            table.AddCell(MyCell(((int)mono.HGetField("SettingsGunCount")).ToString()));
        }
        private PdfPCell MyCell(string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText));
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell.MinimumHeight = 35;//设置行高
            return cell;
        }
        private PdfPCell MyCell(string text, int col, int row)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText));
            cell.Colspan = col;
            cell.Rowspan = row;
            cell.MinimumHeight = 35;//设置行高

            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            return cell;
        }
        private PdfPCell MyCell1(string text, int col, int row)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, fontText));
            cell.Colspan = col;
            cell.Rowspan = row;
            cell.MinimumHeight = 35;//设置行高
            cell.BackgroundColor = new BaseColor(0.5f, 0.5f, 0.5f);
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            return cell;
        }
    }
}
