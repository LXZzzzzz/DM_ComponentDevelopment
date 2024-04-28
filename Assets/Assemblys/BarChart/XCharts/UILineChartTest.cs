using System;
using System.Reflection;
using DM.IFS;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UiManager
{
    public class UILineChartTest : BasePanel
    {
        public bool isReflex = true;

        // private LineChart lineChart;
        private Component lineChartReflex;

        public override void ShowMe(object userData)
        {
            base.ShowMe(userData);
            lineChartReflex = transform.Find("LineChart").gameObject.HGetScript("LineChart");
            if (lineChartReflex != null)
            {
                Debug.Log("走进图标show"+lineChartReflex.GetType());
                lineChartReflex.GetType().GetMethod("Init").Invoke(lineChartReflex, new object[] { true });
                LineShowReflex();
            }
        }

        void Start()
        {
            Debug.LogError("查看是否挂上UILineChartTest");
#if UNITY_EDITOR
            if (!isReflex)
            {
                // lineChart = transform.Find("LineChart").GetComponent<LineChart>();
                // if (lineChart != null)
                // {
                //     lineChart.Init();
                //     LineShow();
                // }
            }

            else
            {
                lineChartReflex = transform.Find("LineChart").GetComponent("XCharts.Runtime.LineChart");
                if (lineChartReflex != null)
                {
                    Debug.Log(lineChartReflex.GetType());
                    lineChartReflex.GetType().GetMethod("Init").Invoke(lineChartReflex, new object[] { true });
                    LineShowReflex();
                }
            }
#endif
        }

        private void LineShow()
        {
            // //标题
            // var title = lineChart.EnsureChartComponent<Title>();
            // title.text = "测试折线图";
            // //提示框
            // var tooltip = lineChart.EnsureChartComponent<Tooltip>();
            // tooltip.show = true;
            // //图例
            // var legend = lineChart.EnsureChartComponent<Legend>();
            // legend.show = true;
            // //设置坐标轴
            // var xAxis = lineChart.EnsureChartComponent<XAxis>();
            // xAxis.splitNumber = 10;
            // xAxis.boundaryGap = true;
            // xAxis.type = Axis.AxisType.Category;
            //
            // var yAxis = lineChart.EnsureChartComponent<YAxis>();
            // yAxis.type = Axis.AxisType.Value;
            //
            // lineChart.RemoveData();
            // lineChart.AddSerie<Line>("测试的系列数据");
            //
            // for (int i = 0; i < 10; i++)
            // {
            //     lineChart.AddData(0, Random.Range(10, 20));
            //     lineChart.AddXAxisData(lineChart.GetSerie(0).data.Count.ToString());
            // }
        }

        private void LineShowReflex()
        {
            Assembly assembly = Assembly.Load("XCharts.Runtime");
            //标题
            MethodInfo EnsureComTitle = lineChartReflex.GetType().GetMethod("EnsureChartComponent").MakeGenericMethod(assembly.GetType("XCharts.Runtime.Title"));
            object title = EnsureComTitle.Invoke(lineChartReflex, null);
            title.GetType().GetProperty("text").SetValue(title,"测试折线图");
            //提示框
            MethodInfo EnsureComTooltip = lineChartReflex.GetType().GetMethod("EnsureChartComponent").MakeGenericMethod(assembly.GetType("XCharts.Runtime.Tooltip"));
            object tooltip = EnsureComTooltip.Invoke(lineChartReflex, null);
            tooltip.GetType().GetProperty("show").SetValue(tooltip,true);
            //图例
            MethodInfo EnsureComLegend = lineChartReflex.GetType().GetMethod("EnsureChartComponent").MakeGenericMethod(assembly.GetType("XCharts.Runtime.Legend"));
            object Legend = EnsureComLegend.Invoke(lineChartReflex, null);
            Legend.GetType().GetProperty("show").SetValue(Legend,true);
            //设置坐标轴
            MethodInfo EnsureComXAxis = lineChartReflex.GetType().GetMethod("EnsureChartComponent").MakeGenericMethod(assembly.GetType("XCharts.Runtime.XAxis"));
            object XAxis = EnsureComXAxis.Invoke(lineChartReflex, null);
            XAxis.GetType().GetProperty("splitNumber").SetValue(XAxis,10);
            XAxis.GetType().GetProperty("boundaryGap").SetValue(XAxis,true);
            //通过反射获取嵌套枚举类型
            var types = assembly.GetType("XCharts.Runtime.Axis").GetNestedTypes();
            object enumCategory = FindEnumValue(types, "AxisType").GetField("Category").GetValue(null);
            XAxis.GetType().GetProperty("type").SetValue(XAxis, enumCategory);
            MethodInfo EnsureComYAxis = lineChartReflex.GetType().GetMethod("EnsureChartComponent").MakeGenericMethod(assembly.GetType("XCharts.Runtime.YAxis"));
            object yAxis = EnsureComXAxis.Invoke(lineChartReflex, null);
            object enumValue = FindEnumValue(types, "AxisType").GetField("Value").GetValue(null);
            yAxis.GetType().GetProperty("type").SetValue(XAxis, enumValue);
            //设置数据
            lineChartReflex.GetType().GetMethod("RemoveData",new Type[]{}).Invoke(lineChartReflex, null);
            MethodInfo EnsureComLine = lineChartReflex.GetType().GetMethod("AddSerie").MakeGenericMethod(assembly.GetType("XCharts.Runtime.Line"));
            EnsureComLine.Invoke(lineChartReflex, new object[] { "测试的系列数据" ,true,false});
            for (int i = 0; i < 10; i++)
            {
                lineChartReflex.GetType().GetMethod("AddData", new Type[] { typeof(int), typeof(double), typeof(string), typeof(string) })
                    .Invoke(lineChartReflex, new object[] { 0, (double)Random.Range(30f, 50f), null, null });
                lineChartReflex.GetType().GetMethod("AddXAxisData").Invoke(lineChartReflex, new object[] { (++xValue).ToString(), 0 });
                
                object serie = lineChartReflex.GetType().GetMethod("GetSerie", new Type[] { typeof(int) }).Invoke(lineChartReflex, new object[] { 0 });
                object data = serie.GetType().GetProperty("data").GetValue(serie);
                
                lineChartReflex.GetType().GetMethod("AddXAxisData").Invoke(lineChartReflex,
                    new object[] { data.GetType().GetProperty("Count").GetValue(data).ToString(), 0 });
            }
        }

        private Type FindEnumValue(Type[] types,string enumName)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsEnum&& string.Equals(enumName,types[i].Name))
                {
                    return types[i];
                }
            }

            return null;
        }
        
        private int xValue = 0;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                var lineChartReflexitem = transform.Find("LineChart").GetComponent("XCharts.Runtime.LineChart");
                sender.LogError("GetComponent获取图表组件" + lineChartReflexitem);
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                lineChartReflex = transform.Find("LineChart").gameObject.HGetScript("LineChart");
                sender.LogError("HGetScript获取图表组件" + lineChartReflex);
                var itemTypes = transform.Find("LineChart").GetComponents<MonoBehaviour>();
                for (int i = 0; i < itemTypes.Length; i++)
                {
                    sender.LogError("挂载的脚本：" + itemTypes[i].GetType().Name);
                }
            } 
            if (Input.GetKeyDown(KeyCode.A))
            {
                lineChartReflex.GetType().GetMethod("AddData", new Type[] { typeof(int), typeof(double), typeof(string), typeof(string) })
                    .Invoke(lineChartReflex, new object[] { 0, (double)Random.Range(0f, 50f), null, null });
                lineChartReflex.GetType().GetMethod("AddXAxisData").Invoke(lineChartReflex, new object[] { (++xValue).ToString(), 0 });
                // lineChart.AddData("line", Random.Range(0, 50));
                // lineChart.AddXAxisData(lineChart.GetSerie(0).data.Count.ToString());
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                xValue = 0;
                lineChartReflex.GetType().GetMethod("ClearData").Invoke(lineChartReflex, null);
            }
        }
    }
}