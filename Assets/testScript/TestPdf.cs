using System;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using UnityEngine;

public class TestPdf : MonoBehaviour
{
    public string dirPath;
    public string reportPath;
    iTextSharp.text.Font fontTitle, fontSub, fontTextBold, fontText, fontcellB, fontNull;

    private void Start()
    {
        
        dirPath = "D:/DM/DM2.2.6D/DM_Data/MapLib/Report";
        string fontPath = "D:/DM/DM2.2.6D/DM_Data/Font/wryh.ttf";
        BaseFont font = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        fontTitle = new iTextSharp.text.Font(font, 24);
        fontSub = new iTextSharp.text.Font(font, 15, iTextSharp.text.Font.BOLD);
        fontTextBold = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.BOLD);
        fontText = new iTextSharp.text.Font(font, 13);
        fontcellB = new iTextSharp.text.Font(font, 12, iTextSharp.text.Font.BOLD);
        fontNull = new iTextSharp.text.Font(font, 5);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ShowPDF();
        }
    }

    private void ShowPDF()
    {
        if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            //string fileName = DateTime.Now.ToLongDateString() + (Directory.GetFiles(dirPath).Length + 1);
            //string fileDate = DateTime.Now.ToLongDateString()+ DateTime.Now.Hour+"-"+ DateTime.Now.Minute+"-"+DateTime.Now.Second;
            reportPath = dirPath + "/" + "userName" + "(" + 66 + ").pdf";
            string fontPath = Application.dataPath + "/Font/wryh.ttf";
            Document doc = new Document();
            FileStream fi = new FileStream(reportPath, FileMode.Create);
            PdfWriter writer = PdfWriter.GetInstance(doc, fi);
            doc.Open();

            Paragraph nullString = new Paragraph("  ", fontNull);
            Paragraph title = new Paragraph("reportName", fontTitle)
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
                TotalWidth = 580, //表格总宽度
                LockedWidth = true //锁定宽度
            };
            table.SetWidths(new int[] { 450, 450, 450, 450 });

            // Paragraph mesAbstract = new Paragraph(Abstract, fontText);
            // mesAbstract.FirstLineIndent = 28; //设置段落的首行缩进
            // doc.Add(mesAbstract);
            doc.Add(nullString);


            Paragraph messagePersonAssessment = new Paragraph("一、岗位职责能力评估", fontSub);
            doc.Add(messagePersonAssessment);
            doc.Add(nullString);
            Paragraph paLevel1;
            paLevel1 = new Paragraph("1.一级指挥员    ", fontSub);
            paLevel1.IndentationLeft = 20f;
            doc.Add(paLevel1);
            doc.Add(nullString);
            //数据：personAss

            //一级指挥员
            PdfPTable commander1 = new PdfPTable(12);

            commander1.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 12, 1));
            commander1.AddCell(MyCell1("评分项", 2, 1));
            commander1.AddCell(MyCell1("评分内容细节", 8, 1));
            commander1.AddCell(MyCell1("分值", 1, 1));
            commander1.AddCell(MyCell1("主观打分", 1, 1));
            commander1.AddCell(MyCell($"确认灾情信息", 2, 2));
            // else commander1.AddCell(MyCell($"确认灾情信息: {scores.qrzqxx}分    (主观：{scores.qrzqxx_zg})", 6, 1));
            commander1.AddCell(MyCell("灾害类型", 2, 1));
            commander1.AddCell(MyCell("灾情规模", 2, 1));
            commander1.AddCell(MyCell("火场面积(平方米)", 2, 1));
            commander1.AddCell(MyCell("", 2, 1));
            commander1.AddCell(MyCell("10", 1, 2));
            commander1.AddCell(MyCell("8", 1, 2));
            //值
            commander1.AddCell(MyCell("personAss.yjzhy.zhlx", 2, 1));
            commander1.AddCell(MyCell("personAss.yjzhy.zqgm", 2, 1));
            commander1.AddCell(MyCell("personAss.yjzhy.hcmj", 2, 1));
            commander1.AddCell(MyCell("", 2, 1));

            commander1.AddCell(MyCell($"确认出动装备信息", 8, 1));
            // else commander1.AddCell(MyCell($"确认出动装备信息： {scores.cdzbxxqr}分    (主观：{scores.cdzbxxqr_zg})", 6, 1));
            commander1.AddCell(MyCell("机型", 2, 1));
            commander1.AddCell(MyCell("编号", 2, 1));
            commander1.AddCell(MyCell("数量", 2, 1));
            //值
            for (int i = 0; i < 1; i++)
            {
                int index = i;
                commander1.AddCell(MyCell("机型", 2, 1));
                commander1.AddCell(MyCell("编号", 2, 1));
                commander1.AddCell(MyCell("1", 2, 1));
            }

            commander1.AddCell(MyCell($"确认出动人员信息", 6, 1));
            // else commander1.AddCell(MyCell($"确认出动人员信息： {scores.cdryxxqr}分    (主观：{scores.cdryxxqr_zg})", 6, 1));
            commander1.AddCell(MyCell("机型编号", 2, 1));
            commander1.AddCell(MyCell("机组", 2, 1));
            commander1.AddCell(MyCell("保障组", 2, 1));
            //值
            for (int i = 0; i < 1; i++)
            {
                int index = i;
                commander1.AddCell(MyCell("personAss.yjzhy.cdjyll[index].jx", 2, 1));
                commander1.AddCell(MyCell("personAss.yjzhy.cdjyll[index].jz", 2, 1));
                commander1.AddCell(MyCell("personAss.yjzhy.cdjyll[index].bzz", 2, 1));
            }

            commander1.AddCell(MyCell($"申报转场航线", 6, 1));
            // else commander1.AddCell(MyCell($"申报转场航线： {scores.zchxsb}分    (主观：{scores.zchxsb_zg})", 6, 1));
            commander1.AddCell(MyCell("航线名称", 1, 1));
            commander1.AddCell(MyCell("personAss.yjzhy.hxgh", 5, 1));
            commander1.AddCell(MyCell("航路点", 1, 1));
            commander1.AddCell(MyCell("string.Join(\"、\", personAss.yjzhy.hlds)", 5, 1));

            commander1.AddCell(MyCell($"下达任务", 6, 1));
            // else commander1.AddCell(MyCell($"下达任务： {scores.xdrw}分    (主观：{scores.xdrw_zg})", 6, 1));
            commander1.AddCell(MyCell("任务简令", 6, 1));
            commander1.AddCell(MyCell("personAss.yjzhy.rwjl", 6, 2));

            doc.Add(commander1);
            doc.Add(nullString);

            Paragraph paLevel2;
            paLevel2 = new Paragraph($"2.二级指挥员", fontSub);
            // else paLevel2 = new Paragraph($"2.二级指挥员    得分:{scores.secondZhyTotalScore:F2}", fontSub);
            paLevel2.IndentationLeft = 20f;
            doc.Add(paLevel2);
            doc.Add(nullString);

            PdfPTable commander2 = new PdfPTable(6);
            commander2.AddCell(MyCell("评分规则：每项训练点满分10分，未完成：0分，较差：1~3分，一般：4~6分，较好：7~9分", 6, 1));
            commander2.AddCell(MyCell($"领受任务", 6, 1));
            // else commander2.AddCell(MyCell($"领受任务： {scores.lsrw}分    (主观：{scores.lsrw_zg})", 6, 1));
            commander2.AddCell(MyCell($"确认出动装备信息", 6, 1));
            // else commander2.AddCell(MyCell($"确认出动装备信息： {scores.qrzbztxx}分    (主观：{scores.qrzbztxx_zg})", 6, 1));
            commander2.AddCell(MyCell("机组信息", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("机型", 1, 1));
            commander2.AddCell(MyCell("装载设备", 1, 1));
            commander2.AddCell(MyCell("载油量（千克）", 1, 1));
            commander2.AddCell(MyCell("可用载重（千克）", 1, 1));
            commander2.AddCell(MyCell("地面维护时间间隔（小 时）", 1, 1));

            for (int i = 0; i < 1; i++)
            {
                int index = i;
                commander2.AddCell(MyCell("personAss.ejzhy.jzxx[index].jzName", 1, 1));
                commander2.AddCell(MyCell("personAss.ejzhy.jzxx[index].jx", 1, 1));
                commander2.AddCell(MyCell("string.Join(\" \", personAss.ejzhy.jzxx[index].zzsb)", 1, 1));
                commander2.AddCell(MyCell("personAss.ejzhy.jzxx[index].zyl.ToString()", 1, 1));
                commander2.AddCell(MyCell("personAss.ejzhy.jzxx[index].zzl.ToString()", 1, 1));
                commander2.AddCell(MyCell("personAss.ejzhy.jzxx[index].dmwhTime.ToString()", 1, 1));
            }

            commander2.AddCell(MyCell($"完成任务分配并下达任务", 6, 1));
            // else commander2.AddCell(MyCell($"完成任务分配并下达任务： {scores.fprwbxdrw}分    (主观：{scores.fprwbxdrw_zg})", 6, 1));
            commander2.AddCell(MyCell("机组", 1, 1));
            commander2.AddCell(MyCell("受灾点", 1, 1));
            commander2.AddCell(MyCell("取水点", 1, 1));
            commander2.AddCell(MyCell("补给站", 1, 1));
            commander2.AddCell(MyCell("备降场", 1, 1));
            commander2.AddCell(MyCell("", 1, 1));
            for (int i = 0; i < 1; i++)
            {
                int index = i;
                commander2.AddCell(MyCell("personAss.ejzhy.rwfp[index].jzName", 1, 1));
                commander2.AddCell(MyCell("string.Join(personAss.ejzhy.rwfp[index].szd)", 1, 1));
                commander2.AddCell(MyCell("string.Join(personAss.ejzhy.rwfp[index].qsd)", 1, 1));
                commander2.AddCell(MyCell("string.Join(personAss.ejzhy.rwfp[index].bjd)", 1, 1));
                commander2.AddCell(MyCell("string.Join(personAss.ejzhy.rwfp[index].bjc)", 1, 1));
                commander2.AddCell(MyCell("", 1, 1));
            }

            commander2.AddCell(MyCell($"确认机长的特情处置报告", 6, 1));
            // else commander2.AddCell(MyCell($"确认机长的特情处置报告： {scores.qrtqczbg}分    (主观：{scores.qrtqczbg_zg})", 6, 1));
            commander2.AddCell(MyCell("报告次数", 3, 1));
            commander2.AddCell(MyCell("确认次数", 3, 1));

            doc.Add(commander2);
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
