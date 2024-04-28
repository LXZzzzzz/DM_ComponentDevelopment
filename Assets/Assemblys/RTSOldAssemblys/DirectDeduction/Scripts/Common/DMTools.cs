using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DM.Entity;
using DM.IFS;
using System.Text.RegularExpressions;
using UnityEngine;

namespace 指挥端
{
    public enum RichTextType
    {
        Nomal = 0,
        Color,
        Font,
        Blod,
    }

    public class DMTools
    {

        public static string GetTimeStr(float time)
        {
            int sec = (int)time % 60;
            int min = (int)time / 60 % 60;
            int hour = (int)time / 60 / 60 % 24;
            return hour.ToString("00") + ":" + min.ToString("00") + ":" + sec.ToString("00");
        }

        /// <summary>
        /// 富文本显示
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="richTextType">富文本类型</param>
        /// <param name="font">字体大小[默认12]</param>
        /// <param name="color">颜色字符串</param>
        /// <returns>富文本</returns>
        public static string SetRichText(string text, RichTextType richTextType = RichTextType.Nomal, int font = 12, string color = "#d9d9ea")
        {
            string str = text;
            switch (richTextType)
            {
                case RichTextType.Nomal:
                    break;
                case RichTextType.Color:
                    str = string.Format("<color={0}>{1}</color>", color, text);
                    break;
                case RichTextType.Font:
                    str = string.Format("<size={0}>{1}</size>", font, text);
                    break;
                case RichTextType.Blod:
                    str = string.Format("<b>{0}</b>", text);
                    break;
                default:
                    break;
            }
            return str;
        }

        #region 位置、经纬度相关接口

        public static DMVector3 ToDMVector3(Vector3 pos)
        {
            DMVector3 dmV3 = new DMVector3();
            dmV3.X = pos.x;
            dmV3.Y = pos.y;
            dmV3.Z = pos.z;
            return dmV3;
        }
        public static Vector3 ToVector3(DMVector3 dmV3)
        {
            Vector3 pos = new Vector3();
            pos.x = dmV3.X;
            pos.y = dmV3.Y;
            pos.z = dmV3.Z;
            return pos;
        }

        /// <summary>
        /// 经纬度转换Vector3
        /// </summary>
        /// <param name="coordinate">[000.000000,000.000000]</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(string coordinate, LonLatType lonLatType = LonLatType.Normal)
        {
            string[] strs = ToLonLats(coordinate);
            if (strs.Length != 2) return default(Vector3);//确保格式
            double targetLat, targetLon;
            LonLatZero lonLatZero = LonLatHelper.GetBasePoint();
            if (lonLatType == LonLatType.Normal)
            {
                targetLat = double.Parse(strs[1]);
                targetLon = double.Parse(strs[0]);
            }
            else
            {
                targetLat = LonLatHelper.ToNormal(strs[1]);
                targetLon = LonLatHelper.ToNormal(strs[0]);
            }
            double lat = LonLatHelper.DisLat(double.Parse(lonLatZero.Latitude), targetLat);
            double lon = LonLatHelper.DisLon(double.Parse(lonLatZero.Longitude), targetLon, lat);

            Vector3 pos = new Vector3();
            pos.x = (float)lon;
            pos.y = 0;
            pos.z = (float)lat;

            return pos;
        }

        /// <summary>
        /// 转换为经纬度
        /// </summary>
        /// <param name="pos">Vector3</param>
        /// <returns>[000.000000E(W),000.000000N(S)]</returns>
        public static string ToLonLat(Vector3 pos, LonLatType lonLatType = LonLatType.Normal)
        {
            LonLatZero lonLatZero = LonLatHelper.GetBasePoint();
            double zeroLat = double.Parse(lonLatZero.Latitude);
            double zeroLon = double.Parse(lonLatZero.Longitude);
            double lat = LonLatHelper.GetLatByDis(zeroLat, pos.z);
            double lon = LonLatHelper.GetLonByDis(zeroLon, pos.x, lat);
            string value;
            if (lonLatType == LonLatType.Normal)
            {
                value = string.Format("{0},{1}", lon.ToString("F6"), lat.ToString("F6"));
            }
            else
            {
                //判断东西经度E(W)，南北纬度S(N)
                string lonSymbol = lonLatZero.LonType.ToString();
                string latSymbol = lonLatZero.LatType.ToString();
                value = string.Format("{0}{1},{2}{3}", LonLatHelper.ToDegree(lon), lonSymbol, LonLatHelper.ToDegree(lat), latSymbol);
            }
            return value;
        }

        public static string[] ToLonLats(Vector3 pos, LonLatType lonLatType = LonLatType.Normal)
        {
            return ToLonLats(ToLonLat(pos, lonLatType));
        }

        /// <summary>
        /// 将经纬度转成string[2]带有E(W)、N(S)的数据
        /// </summary>
        public static string[] ToLonLats(string coordinate)
        {
            if (coordinate.Contains("E")) coordinate = coordinate.Replace("E", "");
            if (coordinate.Contains("W")) coordinate = coordinate.Replace("W", "");
            if (coordinate.Contains("S")) coordinate = coordinate.Replace("S", "");
            if (coordinate.Contains("N")) coordinate = coordinate.Replace("N", "");

            return coordinate.Split(',');
        }

        /// <summary>
        /// 将Vector3的string(x,z)格式转为Vector3格式
        /// </summary>
        public static Vector3 StringToVector3(string posStr)
        {
            string[] strs = posStr.Split(',');
            if (strs.Length != 2) return default(Vector3);//确保格式
            Vector3 pos = new Vector3();
            pos.x = float.Parse(strs[0]);
            pos.y = 0;
            pos.z = float.Parse(strs[1]);

            return pos;
        }

        /// <summary>
        /// 将经纬度转为Vector3的string(x,z)格式
        /// </summary>
        public static string ToVector3String(string coordinate, LonLatType lonLatType = LonLatType.Normal)
        {
            Vector3 pos = ToVector3(coordinate, lonLatType);
            return ToVector3String(pos);
        }

        /// <summary>
        /// 将Vector3转为Vector3的string(x,z)格式
        /// </summary>
        public static string ToVector3String(Vector3 pos)
        {
            return string.Format("{0},{1}", pos.x.ToString("F6"), pos.z.ToString("F6"));
        }

        /// <summary>
        /// //判断经纬度格式是否正确
        /// </summary>
        public static bool IsMatchLonLat(string text)
        {
            text = text.Trim();
            if (text.Contains("E")) text = text.Replace("E", "");
            if (text.Contains("W")) text = text.Replace("W", "");
            if (text.Contains("N")) text = text.Replace("N", "");
            if (text.Contains("S")) text = text.Replace("S", "");
            return Regex.IsMatch(text, @"^[0-9]+°[0-9]+′[0-9]+\.?[0-9]+″$");
        }

        #endregion

    }
}
