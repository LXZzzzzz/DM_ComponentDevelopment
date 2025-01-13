using System;
using System.Collections.Generic;
using UnityEngine;

namespace ToolsLibrary.EquipPart
{
    public abstract class ZiYuanBase : DMonoBehaviour
    {
        private List<string> _beUsedCommanderIds;

        private ZiYuanType ziYuanType;

        private string bobjectId;

        private float detectionRange;

        private Color myColor, chooseColor;

        private ZyVariableDataBase variableData;

        [HideInInspector] public bool isChooseMe;

        public List<string> beUsedCommanderIds => _beUsedCommanderIds;

        public string ziYuanName, ziYuanDescribe; //记录资源名和资源描述（后面如果要拓展，就写一个抽象方法返回list<string>）

        public Vector2 latAndLon; //资源经纬度

        public Sprite ZiyuanIcon;

        public void Init(string id, float dr, string colorCode, string chooseColorCode)
        {
            bobjectId = id;
            detectionRange = dr;

            Debug.LogError(colorCode + "颜色值" + chooseColorCode);
            if (string.IsNullOrEmpty(colorCode) && string.IsNullOrEmpty(chooseColorCode))
            {
                Debug.LogError("原色：" + myColor + "+" + chooseColor);
                return;
            }

            if (ColorUtility.TryParseHtmlString(colorCode, out Color color))
            {
                myColor = color;
            }

            if (ColorUtility.TryParseHtmlString(chooseColorCode, out Color cColor))
            {
                chooseColor = cColor;
            }
        }

        //可被作用的检测范围
        public float DetectionRange
        {
            get => detectionRange;
            private set => detectionRange = value;
        }

        public string BobjectId
        {
            get => bobjectId;
            private set => bobjectId = value;
        }

        public ZiYuanType ZiYuanType
        {
            get { return ziYuanType; }

            protected set { ziYuanType = value; }
        }

        public Color MyColor => myColor;

        public Color ChooseColor => chooseColor;

        public abstract void OnStart();

        public void Reset()
        {
            SetBeUsedComs(null);
            OnReset();
        }

        protected abstract void OnReset();

        public void SetBeUsedComs(List<string> data)
        {
            _beUsedCommanderIds = data;
        }

        public void AddBeUsdCom(string comId)
        {
            if (_beUsedCommanderIds == null) _beUsedCommanderIds = new List<string>();
            _beUsedCommanderIds.Add(comId);
        }

        public void RemoveBeUsedCom(string comId)
        {
            if (_beUsedCommanderIds.Contains(comId))
            {
                _beUsedCommanderIds.Remove(comId);
            }
        }

        public void SetVariableData(ZyVariableDataBase data)
        {
            variableData = data;
            OnSetVariableData();
        }

        protected abstract void OnSetVariableData();

        public ZyVariableDataBase GetVariableData()
        {
            return variableData;
        }
    }

    public enum ZiYuanType
    {
        // 火源点
        SourceOfAFire,

        // 水源点
        Waters,

        // 机场
        Airport,

        // 医院
        Hospital,

        // 救助站
        RescueStation,

        // 灾区点
        DisasterArea,

        // 补给点
        Supply,

        // 物资点
        GoodsPoint,

        // 任务点
        TaskPoint
    }

    public abstract class ZyVariableDataBase
    {
        public string ZyName;
        public ZiYuanType ZyType;
    }

    public class FireVariableData : ZyVariableDataBase
    {
        //风速、坡度、初始燃烧面积
        public float fs;
        public float pd;
        public float csrsmj;
    }

    public class DisasterVariableData : ZyVariableDataBase
    {
        public int personNum; //需救助人数
        public int type; //人员类型
    }

    public class SupplyVariableData : ZyVariableDataBase
    {
        public float oilNum; //油量
    }

    public class GoodsPointVariableData : ZyVariableDataBase
    {
        public float goodsNum; //物资量
    }


    //展示信息的结构父类
    public abstract class ShowViewInfoBase
    {
        public int showType;
    }

    //无输入参数
    public class ShowNoInputData : ShowViewInfoBase
    {
        public ShowNoInputData(int st)
        {
            showType = st;
        }
    }

    //展示需要传递字符串信息的页面
    public class ShowStrInputData : ShowViewInfoBase
    {
        public string strInfo;

        public ShowStrInputData(int st, string str)
        {
            showType = st;
            strInfo = str;
        }
    }
    //展示需要传递字符串信息的页面
    public class ShowStrInputData_Daojiao : ShowViewInfoBase
    {
        public string strInfo;

        public ShowStrInputData_Daojiao(int st, string str)
        {
            showType = st;
            strInfo = str;
        }
    }
}