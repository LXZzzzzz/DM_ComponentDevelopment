using DM.Core.Map;
using DM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [System.Serializable]
    public class MiniMap : InfoBase
    {
        private Image map;
        private Text name;
        private Transform comPoints;
        private Transform groupLabels;

        //预制体
        private GameObject prefabGroupLabelItem;
        private GameObject prefabComPointItem;

        //<分组名, 分组数据>
        public Dictionary<string, GroupLabelData> DicGroupLabelData = new Dictionary<string, GroupLabelData>();
        //<分组名, 对应分组组件点数据列表>
        public Dictionary<string, List<ComPointData>> DicComPointData = new Dictionary<string, List<ComPointData>>();
        //<组件Id, 组件点数据>
        public Dictionary<string, ComPointData> DicAllComPointData = new Dictionary<string, ComPointData>();

        protected override void OnInitUI()
        {
            base.OnInitUI();
            Transform mapInfo = transform.Find("MapInfo");
            map = mapInfo.Find("Map").GetComponent<Image>();
            name = mapInfo.Find("Name").GetComponent<Text>();
            comPoints = mapInfo.Find("ComPoints");
            groupLabels = mapInfo.Find("GroupLabels");
            Transform prefabs = transform.Find("Prefabs");
            prefabGroupLabelItem = prefabs.Find("GroupLabelItem").gameObject;
            prefabComPointItem = prefabs.Find("ComPointItem").gameObject;
        }

        public void SetData(Sprite map, string name)
        {
            this.map.sprite = map;
            this.name.text = name;
        }

        public void SetData(Dictionary<string, List<BObjectModel>> dic)
        {
            //格式:分组名,颜色,关联分组;例如:救援力量,Red,救援力量; 蓝方,Blue,遇险事件;
            foreach (var item in dic)
            {
                string[] groupStrs = item.Key.Split(',');
                CreateGroupLabelItem(groupStrs[0], groupStrs[1], item.Value);
            }
        }

        public ComPointData GetComPointData(string bObjectId)
        {
            if (DicAllComPointData.ContainsKey(bObjectId))
            {
                return DicAllComPointData[bObjectId];
            }
            return null;
        }

        public void OnDestory()
        {
            DicGroupLabelData.Clear();
            foreach (var item in DicComPointData)
            {
                item.Value.Clear();
            }
            DicComPointData.Clear();
            DicAllComPointData.Clear();
        }

        private void CreateGroupLabelItem(string groupName, string color, List<BObjectModel> bObjectModels)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(prefabGroupLabelItem, groupLabels);
            GroupLabelData groupLabelData = new GroupLabelData(obj.transform, this);
            groupLabelData.Set(groupName, color);
            DicGroupLabelData.Add(groupName, groupLabelData);
            List<ComPointData> comPointDatas = new List<ComPointData>();
            foreach (var item in bObjectModels)
            {
                comPointDatas.Add(CreateComPointItem(groupName, color, item));
            }
            DicComPointData.Add(groupName, comPointDatas);
        }

        private ComPointData CreateComPointItem(string groupName, string color, BObjectModel bObjectModel)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(prefabComPointItem, comPoints);
            ComPointData comPointData = new ComPointData(obj.transform, this, bObjectModel);
            comPointData.SetColor(color);
            DicAllComPointData.Add(bObjectModel.BObject.Id, comPointData);
            return comPointData;
        }

        private Color GetColor(string colorStr)
        {
            Color color = Color.white;
            switch (colorStr)
            {
                case "White":
                    color = Color.white;
                    break;
                case "Red":
                    color = Color.red;
                    break;
                case "Blue":
                    color = new Color(57 / 255f, 166 / 255f, 247 / 255f, 1);
                    break;
                case "Yellow":
                    color = new Color(1, 186 / 255f, 0, 1);
                    break;
                default:
                    break;
            }
            return color;
        }

        public class GroupLabelData
        {
            private Image mark;
            private Text groupName;

            private MiniMap miniMap;

            public GroupLabelData(Transform trans, MiniMap miniMap)
            {
                mark = trans.Find("Mark").GetComponent<Image>();
                groupName = trans.Find("Name").GetComponent<Text>();
                this.miniMap = miniMap;
            }

            public void Set(string groupName, string color)
            {
                mark.color = miniMap.GetColor(color);
                this.groupName.text = groupName;
            }
        }

        public class ComPointData
        {
            public BObjectModel bObjectModel = null;

            private Image mark;
            private Image comIcon;
            private Text comName;
            private GameObject selected;
            private GameObject direction;
            private MiniMap miniMap;

            //private bool showName = false;
            private bool showIcon = false;
            private bool showSelected = false;
            private int showDirection = 0;

            public ComPointData(Transform trans, MiniMap miniMap, BObjectModel bObjectModel)
            {
                mark = trans.Find("Mark").GetComponent<Image>();
                comIcon = trans.Find("Icon").GetComponent<Image>();
                comName = trans.Find("Name").GetComponent<Text>();
                selected = trans.Find("Selected").gameObject;
                direction = trans.Find("Direction").gameObject;
                this.miniMap = miniMap;
                this.bObjectModel = bObjectModel;
                this.comName.text = bObjectModel.BObject.Info.Name;
                comIcon.sprite = DirectDeductionMgr.GetInstance.GetComIcon(bObjectModel.BObject.Id);

                ToggleProperty togglePro1 = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("小地图", "显示组件名称");
                //showName = toggleProperty1.Value;
                this.comName.gameObject.SetActive(togglePro1.Value);
                ToggleProperty togglePro2 = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("小地图", "显示选中状态");
                showSelected = togglePro2.Value;
                DropDownProperty dropDownPro = (DropDownProperty)DirectDeductionMgr.GetInstance.GetProperties("小地图", "是否显示朝向");
                showDirection = dropDownPro.Selected.Enum;
                if (showDirection == 2) ShowDirection(true);
                ShowIcon(false);
            }

            public void SetColor(string color)
            {
                mark.color = miniMap.GetColor(color);
            }

            public void ShowIcon(bool show)
            {
                comIcon.gameObject.SetActive(show);
            }

            public void ShowSelected(bool active)
            {
                selected.SetActive(active && showSelected);
                if (showDirection == 1) 
                {
                    ShowDirection(active);
                }
            }

            public void ShowDirection(bool active)
            {
                if (showDirection == 0) return;
                direction.SetActive(active);
            }
        }
    }
}
