/*********************************************
 * Author:Leo
 * Create:2023.04.25
 * Modify:
 * Func:
 * *******************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DM.RGEditor
{
    public static partial class RGFieldExtension
    {
        public static string GetStringValue(this object value, string type)
        {
            string val = GetValueType(type, value);
            if (val == null) val = GetValueTypeExt(type, value);
            if (val == null) val = GetReferenceType(type, value);
            if (val == null) val = GetReferenceTypeExt(type, value);
            if (string.IsNullOrEmpty(val)) val = "null";
            return val;
        }

        #region 值类型 object to string
        public static string GetValueType(string type, object value)
        {
            switch (type)
            {
                case "System.String":
                case "System.Boolean":
                case "System.Byte":
                case "System.Char":
                case "System.Decimal":
                case "System.Double":
                case "System.Single":
                case "System.Int32":
                case "System.Int64":
                case "System.UInt32":
                case "System.UInt16":
                    return value.ToString();
            }
            return null;
        }
        public static string GetValueTypeExt(string type, object value)
        {
            #region UnityEngine
            if (type == "UnityEngine.Color")
            {
                Color c = (Color)value;
                return c.r + "-" + c.g + "-" + c.b + "-" + c.a;
            }
            if (type == "UnityEngine.Vector3")
            {
                Vector3 vec=(Vector3)value;
                return vec.x + "-" + vec.y + "-" + vec.z;
            }
            if (type == "UnityEngine.Quaternion")
            {
                Quaternion quat = (Quaternion)value;
                return quat.x + "-" + quat.y + "-" + quat.z + "-" + quat.w;
            }
            if (type == "UnityEngine.AnimationCurve")
            {

            }
            #endregion
            return null;
        }
        #endregion

        #region 引用类型 自定义拆箱
        public static string GetReferenceType(string type, object value)
        {
            //System.Collections.Generic.List`1[System.String]
            //System.Collections.Generic.List`1[System.Collections.Generic.List`1[System.String]]
            if (type.Contains("System.Collections.Generic.List`1"))
            {
            }
            //System.Collections.Generic.Dictionary`2[System.String,System.Int32]
            //System.Collections.Generic.Dictionary`2[System.String,System.Collections.Generic.List`1[System.String]]
            if (type.Contains(""))
            //System.String[]
            if (type.Contains("[]")) //Array
            {
                //switch (type)
                //{
                //    case "System.String[]":
                //        return ((List<string>)value).ToParamStr<string>();
                //    case "System.Boolean[]":
                //        return ((List<bool>)value).ToParamStr<bool>();
                //    case "System.Byte[]":
                //        return ((List<byte>)value).ToParamStr<byte>();
                //    case "System.Char[]":
                //        return ((List<char>)value).ToParamStr<char>();
                //    case "System.Decimal[]":
                //        return ((List<decimal>)value).ToParamStr<decimal>();
                //    case "System.Double[]":
                //        return ((List<double>)value).ToParamStr<double>();
                //    case "System.Single[]":
                //        return ((List<Single>)value).ToParamStr<Single>();
                //    case "System.Int32[]":
                //        return ((List<Int32>)value).ToParamStr<Int32>();
                //    case "System.Int64[]":
                //        return ((List<Int64>)value).ToParamStr<Int64>();
                //    case "System.UInt32[]":
                //        return ((List<UInt32>)value).ToParamStr<UInt32>();
                //    case "System.UInt16[]":
                //        return ((List<UInt64>)value).ToParamStr<UInt64>();
                //}               
            }
            return null;
        }
        public static string GetReferenceTypeExt(string type, object value)
        {
            #region UnityEngine
            if (value == null||value.ToString()=="null") return null;
            if (type == "UnityEngine.Transform")
            {
                return GetRelativePath(value);
            }
            if (type == "UnityEngine.GameObject")
            {
                return GetRelativePath(((GameObject)value).transform);
            }
            #endregion

            #region UnityEngine.UI
            switch (type)
            {
                case "UnityEngine.UI.Button":
                    return GetRelativePath(((Button)value).transform);
                case "UnityEngine.UI.Text":
                    return GetRelativePath(((Text)value).transform);
                case "UnityEngine.UI.Toggle":
                    return GetRelativePath(((Toggle)value).transform);
                case "UnityEngine.UI.Slider":
                    return GetRelativePath(((Slider)value).transform);
                case "UnityEngine.UI.Dropdown":
                    return GetRelativePath(((Dropdown)value).transform);
                case "UnityEngine.UI.Image":
                    return GetRelativePath(((Image)value).transform);
                case "UnityEngine.UI.RawImage":
                    return GetRelativePath(((RawImage)value).transform);
                case "UnityEngine.UI.Scrollbar":
                    return GetRelativePath(((Scrollbar)value).transform);
                case "UnityEngine.UI.ScrollRect":
                    return GetRelativePath(((ScrollRect)value).transform);
                case "UnityEngine.UI.InputField":
                    return GetRelativePath(((InputField)value).transform);
                case "UnityEngine.Canvas":
                    return GetRelativePath(((Canvas)value).transform);
            }
            #endregion
            return null;
        }
        private static string GetRelativePath(object value)
        {
            string path = string.Empty;
            Transform current = (Transform)value;
            while (current != null)
            {
                path = string.IsNullOrEmpty(path) ? current.name : current.name + "/" + path;
                current = current.parent;
            }
            if (path == ((Transform)value).name) return "#MySelf";
            else return path.Substring(path.IndexOf("/") + 1);
        }
        #endregion
    }
}
