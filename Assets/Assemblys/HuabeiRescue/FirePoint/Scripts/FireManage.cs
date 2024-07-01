using System;
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
    private double Wcount;

    private double SprayingArea = 0f; //喷洒面积
    private double WaterTimeOld; //记录每次浇水后
    private double WaterTimeNew; //记录每次浇水时
    private double WTime; //修改过后的时间
    private int index = 0; //第几次浇水

    public double burnArea => _burnArea;

    public double burnedArea => _burnedArea;

    public void Init(float wind, float slope, float squareMeasure)
    {
        fireTran = transform.Find("BigFire");
        this.wind = wind;
        this.slope = slope > 10 ? 10 : slope;
        _burnArea = 0;
        _burnedArea = 0;
        IsFire = false;
        isGaming = true;
        index = 0;
        fireTran.gameObject.SetActive(true);

        gameTimer = ComputeTime(squareMeasure / 400);
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

    public double ComputeBurnCount(double time)
    {
        double burnCount = -24.889674335 * slope + 3.646092538 * Math.Pow(slope, 2) - 0.088770151 * Math.Pow(slope, 3) - 0.000209294 * Math.Pow(wind, 4) + 0.000048718 * Math.Pow(wind, 3) * slope +
            0.000071368 * Math.Pow(wind, 2) * Math.Pow(slope, 2) - 0.000004081 * wind * Math.Pow(slope, 3) + 0.000693082 * Math.Pow(slope, 4) + 0.0138951 * time + 14;

        return burnCount;
    }

    public double ComputeBurnedCount(double time)
    {
        double burnedCount = -0.001177737 * wind + 0.030355707 * slope - 0.215432836 * Math.Pow(wind, 2) + 2.581114293 * wind * slope - 0.492540312 * Math.Pow(wind, 3)
            + 0.332922535 * Math.Pow(wind, 2) * slope - 0.062450523 * wind * Math.Pow(slope, 2) - 0.007056557 * Math.Pow(slope, 2) + 0.6554745 * time;


        return burnedCount;
    }

    public double ComputeTime(double count)
    {
        double WTime = -(-24.889674335 * slope + 3.646092538 * Math.Pow(slope, 2) - 0.088770151 * Math.Pow(slope, 3) - 0.000209294 * Math.Pow(wind, 4) + 0.000048718 * Math.Pow(wind, 3) * slope +
            0.000071368 * Math.Pow(wind, 2) * Math.Pow(slope, 2) - 0.000004081 * wind * Math.Pow(slope, 3) + 0.000693082 * Math.Pow(slope, 4) + 14 - count) / 0.0138951;
        return WTime;
    }
}