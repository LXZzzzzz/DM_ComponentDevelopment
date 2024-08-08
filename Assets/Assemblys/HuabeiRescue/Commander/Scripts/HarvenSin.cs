using UnityEngine;
using System;

/// <summary>
/// 经纬度转换
/// </summary>
public class HarvenSin
{
    static double EARTH_RADIUS = 6371393; //地球半径  

    public static double DisLat(double lat1, double lat2)
    {
        double L = 2 * Math.PI * EARTH_RADIUS;
        return L / 360 * (lat2 - lat1);
    }

    public static double DisLon(double lon1, double lon2, double lat)
    {
        double L = 2 * Math.PI * EARTH_RADIUS;
        return L / 360 * Math.Cos(Mathf.Deg2Rad * lat) * (lon2 - lon1);
    }

    public static double GetLatByDis(double lat1, double dis)
    {
        double L = 2 * Math.PI * EARTH_RADIUS;
        return dis / L * 360 + lat1;
    }

    public static double GetLonByDis(double lon1, double dis, double lat)
    {
        double L = 2 * Math.PI * EARTH_RADIUS;
        return dis / L * 360 / Math.Cos(Mathf.Deg2Rad * lat) + lon1;
    }

    //度分秒格式转度格式
    public static double ToNormal(string str)
    {
        str = str.Replace("°", "|");
        //支持中文格式
        str = str.Replace("′", "|");
        str = str.Replace("″", "|");
        //支持英文格式
        str = str.Replace("'", "|");
        str = str.Replace("\"", "|");
        string[] strs = str.Split('|');
        int intiger = int.Parse(strs[0]);
        float fen = float.Parse(strs[1]);
        float miao = float.Parse(strs[2]);
        return intiger + fen / 60 + miao / 3600;
    }

    //度格式转度分秒格式
    public static string ToDegree(double deg)
    {
        int intiger = (int)deg;
        double temp = (deg - intiger) * 60;
        int fen = (int)temp;
        string miao = ((temp - fen) * 60).ToString("00.00");
        return intiger + "°" + fen + "'" + miao + "\"";
    }
}