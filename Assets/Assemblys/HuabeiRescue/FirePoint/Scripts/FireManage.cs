using System;
using System.Collections;
using UnityEngine;

public class FireManage : DMonoBehaviour
{
    public Transform fireTran;
    public float slope;
    public float wind;
    public bool isGaming = false;
    private bool isWater = false;
    public bool IsFire = false;

    private double gameTimer = 0f; //系统运行开始后开始计时
    private double burnCount = 0;
    private double burnedCount = 0;
    private double _burnArea = 0; //燃烧面积
    private double _burnedArea = 0f; //过火面积
    private double _csBurnedArea = 0;//初始过火面积
    private double Wcount;

    private double SprayingArea = 0f; //喷洒面积
    private double WaterTimeOld; //记录每次浇水后
    private double WaterTimeNew; //记录每次浇水时
    private double WTime; //修改过后的时间
    private int index = 0; //第几次浇水

    public double burnArea => _burnArea;

    public double burnedArea => _burnedArea;

    public double csBurnedArea => _csBurnedArea;

    public void Init(float wind, float slope, float squareMeasure)
    {
        fireTran = transform.Find("BigFire");
        this.wind = wind <= 1 ? 1 : wind;
        this.slope = slope <= 16 ? 16 : slope;
        _burnArea = 0;
        _burnedArea = 0;
        IsFire = false;
        isGaming = true;
        index = 0;
        fireTran.gameObject.SetActive(true);
        Debug.LogError("初始燃烧面积：" + squareMeasure);
        gameTimer = ComputeTime(squareMeasure / 400);
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return 1;
        _csBurnedArea = _burnedArea;
        Debug.LogError("初始过火燃烧面积：" + _csBurnedArea);
    }
    

    public void Update()
    {
        if (IsFire) return;

        if (isGaming)
        {
            gameTimer += Time.fixedDeltaTime;

            burnCount = ComputeBurnCount(gameTimer);
            burnedCount = ComputeBurnedCount(gameTimer);

            _burnArea = burnCount * 400;
            _burnedArea = burnedCount * 400;

            // dm.Point.BurnedArea = burnArea;
            // dm.Point.BurningArea = burnedArea;
        }

        if (isWater)
        {
            Debug.LogError("第几次浇水：" + index + "     当前时间：" + gameTimer + "     燃烧面积：" + burnArea + "       过火面积：" + burnedArea);
            if (index == 1)
            {
                _burnArea = _burnArea - SprayingArea;
                // dm.Point.BurnedArea = burnArea;

                if (_burnArea <= 0)
                {
                    burnedCount = ComputeBurnedCount(gameTimer);
                    _burnedArea = burnedCount * 400;
                    // dm.Point.BurningArea = burnedArea;

                    fireTran.gameObject.SetActive(false); //火灭了
                    IsFire = true;
                    Debug.LogError("火灭了");
                }
                else
                {
                    Wcount = _burnArea / 400;
                    WTime = ComputeTime(Wcount);
                }
            }
            else
            {
                double TimeInterval = WaterTimeNew - WaterTimeOld;
                WTime = TimeInterval + WTime;
                burnCount = ComputeBurnCount(WTime);
                _burnArea = (burnCount * 400) - SprayingArea;
                // dm.Point.BurnedArea = burnArea;

                if (_burnArea <= 0)
                {
                    burnedCount = ComputeBurnedCount(gameTimer);
                    _burnedArea = burnedCount * 400;
                    // dm.Point.BurningArea = burnedArea;

                    fireTran.gameObject.SetActive(false); //火灭了
                    IsFire = true;
                    Debug.LogError("火灭了");
                }
                else
                {
                    Wcount = _burnArea / 400;
                    WTime = ComputeTime(Wcount);
                }
            }

            WaterTimeOld = WaterTimeNew;
            isWater = false;
        }
    }

    /// <summary>
    /// 开始浇水
    /// </summary>
    /// <param name="sprayingArea">喷洒面积</param>
    /// <param name="time">喷洒时的时间</param>
    public void SetDrowning(double sprayingArea, double waterTime)
    {
        index++;
        SprayingArea = sprayingArea;
        WaterTimeNew = waterTime;
        isGaming = false;
        isWater = true;
    }

    /// <summary>
    /// 燃烧面积公式
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public double ComputeBurnCount(double time)
    {
        double newSlope = slope * Math.PI / 180;
        double a = 0.1127 * Math.Pow(Math.Tan(newSlope), 2);
        double b = 0.422 * (Math.Pow((3.284 * wind), 0.381));
        double c = 0.305 * time - 9.06;
        double burnCount = a * b * c;
        return burnCount;
    }

    /// <summary>
    /// 过火面积公式
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public double ComputeBurnedCount(double time)
    {
        double newSlope = slope * Math.PI / 180;
        double a = 0.0648 * Math.Pow(Math.Tan(newSlope), 2);
        double b = 0.438 * (Math.Pow((3.284 * wind), 0.381));
        double c = 2.546 * time - 2999;
        double burnedCount = a * b * c;
        return burnedCount;
    }

    /// <summary>
    /// 时间公式
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public double ComputeTime(double count)
    {
        double newSlope = slope * Math.PI / 180;
        double a = 0.1127 * Math.Pow(Math.Tan(newSlope), 2);
        double b = 0.422 * (Math.Pow((3.284 * wind), 0.381));
        double count1 = a * b;
        double WTime = (9.06 + (count / count1)) / 0.305;
        return WTime;
    }
}