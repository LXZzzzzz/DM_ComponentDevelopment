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
    }

    public enum ZiYuanType
    {
        /// <summary>
        /// 火源点
        /// </summary>
        SourceOfAFire,

        /// <summary>
        /// 水源点
        /// </summary>
        Waters,

        /// <summary>
        /// 机场
        /// </summary>
        Airport,

        /// <summary>
        /// 医院
        /// </summary>
        Hospital,

        /// <summary>
        /// 救助站
        /// </summary>
        RescueStation,

        /// <summary>
        /// 灾区点
        /// </summary>
        DisasterArea,

        /// <summary>
        /// 补给点
        /// </summary>
        Supply,

        /// <summary>
        /// 物资点
        /// </summary>
        GoodsPoint,

        /// <summary>
        /// 任务点
        /// </summary>
        TaskPoint
    }
}