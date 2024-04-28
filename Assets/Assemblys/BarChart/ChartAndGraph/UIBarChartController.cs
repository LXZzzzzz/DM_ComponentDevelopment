using System;
using DM.IFS;
using UnityEngine;
using UiManager;
using Random = UnityEngine.Random;

/*
 * 需要有的功能
 * 新增数据组，新增数据元素（包括增删改查）
 * 数据初始化
 * 刷新图表展示
 */
public class UIBarChartController : BasePanel
{
    // private BarChart barChart;
    private Component barChartReflex;
    public Material mat;
    public bool isUseReflex=true;


    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        // barChart = GetComponentInChildren<BarChart>();
        barChartReflex = transform.Find("BarCanvasSimple").gameObject.HGetScript("CanvasBarChart");
        sender.LogError("获取图表组件" + barChartReflex);
    }

    private void Start()
    {
#if UNITY_EDITOR

        barChartReflex = transform.Find("BarCanvasSimple").gameObject.HGetScript("CanvasBarChart");
        Debug.LogError("获取图表组件" + barChartReflex);

        var itemTypes = transform.Find("BarCanvasSimple").GetComponents<MonoBehaviour>();
        for (int i = 0; i < itemTypes.Length; i++)
        {
            Debug.LogError("挂载的脚本：" + itemTypes[i].GetType());
        }
#endif
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            var barChartReflexitem = transform.Find("BarCanvasSimple").GetComponent("ChartAndGraph.BarChart");
            sender.LogError("GetComponent获取图表组件" + barChartReflexitem);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            barChartReflex = transform.Find("BarCanvasSimple").gameObject.HGetScript("CanvasBarChart");
            sender.LogError("HGetScript获取图表组件" + barChartReflex);
            var itemTypes = transform.Find("BarCanvasSimple").GetComponents<MonoBehaviour>();
            for (int i = 0; i < itemTypes.Length; i++)
            {
                sender.LogError("挂载的脚本：" + itemTypes[i].GetType().Name);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            testSetValue();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // barChart.DataSource.ClearCategories();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // barChart.DataSource.ClearGroups();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            testInitData();
        }
    }

    private void testSetValue()
    {
        if (!isUseReflex)
        {
            // if (barChart != null)
            // {
            //     barChart.DataSource.SetValue("Player1", "All", Random.value * barChart.DataSource.MaxValue);
            //     barChart.DataSource.SlideValue("Player2", "All", Random.value * barChart.DataSource.MaxValue, 3);
            //     ChartDynamicMaterial itemMat = barChart.DataSource.GetMaterial("Player2");
            //     mat = itemMat.Normal;
            //     barChart.DataSource.SetMaterial("Player2", new ChartDynamicMaterial(itemMat.Normal, Color.black, Color.blue));
            // }
        }
        else
        {
            if (barChartReflex != null)
            {
                // sender.LogError("进入了反射图表逻辑内");
                object mDataSource = barChartReflex.GetType().GetProperty("DataSource").GetValue(barChartReflex);
                double maxValue = (double)mDataSource.GetType().GetProperty("MaxValue").GetValue(mDataSource);
                Debug.LogError("maxValue" + maxValue);
                mDataSource.GetType().GetMethod("SetValue").Invoke(mDataSource, new object[3] { "Player1", "All", Random.value * maxValue });
                //SlideValue函数存在重载，需要指定调用函数的具体参数
                Type[] parameterTypes = { typeof(string), typeof(string), typeof(double), typeof(float) };
                mDataSource.GetType().GetMethod("SlideValue", parameterTypes).Invoke(mDataSource, new object[4] { "Player2", "All", Random.value * maxValue, 3f });

                object player2ChartMat = mDataSource.GetType().GetMethod("GetMaterial").Invoke(mDataSource, new object[1] { "Player2" });
                mat = (Material)player2ChartMat.GetType().GetField("Normal").GetValue(player2ChartMat);
                object chartMat = Activator.CreateInstance(player2ChartMat.GetType(), mat, Color.green, Color.yellow);
                Type[] setMaterialTypes = { typeof(string), player2ChartMat.GetType() };
                mDataSource.GetType().GetMethod("SetMaterial", setMaterialTypes).Invoke(mDataSource, new object[2] { "Player2", chartMat });
            }
        }
    }

    private void testInitData()
    {
        // barChart.DataSource.AddGroup("All");
        // barChart.DataSource.AddCategory("Player2", mat);
    }
}