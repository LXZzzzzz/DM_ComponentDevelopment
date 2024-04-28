using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [System.Serializable]
    public class ElementInfoPanel : InfoBase
    {
        private BObjectModel bObjectModel;

        private GameObject openObj;
        private Transform infoPanel;
        private Image icon;
        private Text title;
        private Text type;
        private Text state;
        private Text coordsX;
        private Text coordsY;
        
        private PropertyInfo propertyInfo = new PropertyInfo();
        private EquipInfo equipInfo = new EquipInfo();
        private Setting setting = new Setting();
        private ZhiHuiDuanMain zhiHuiDuanMain;

        private List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
        //性能
        private List<DynamicProperty> dProperties = new List<DynamicProperty>();

        bool isTrue = false;

        protected override void OnInitUI()
        {
            base.OnInitUI();
            openObj = transform.Find("Btn_Open").gameObject;
            infoPanel = transform.Find("InfoPanel");
            icon = infoPanel.Find("Icon").GetComponent<Image>();
            title = infoPanel.Find("Title").GetComponent<Text>();
            type = infoPanel.Find("Type/Text").GetComponent<Text>();
            state = infoPanel.Find("State").GetComponent<Text>();
            coordsX = infoPanel.Find("Coords/X").GetComponent<Text>();
            coordsY = infoPanel.Find("Coords/Y").GetComponent<Text>();

            Transform windowInfos = transform.Find("WindowInfos");
            GameObject property = windowInfos.Find("Property").gameObject;
            GameObject equipInfoObj = windowInfos.Find("EquipInfo").gameObject;
            GameObject settingObj = windowInfos.Find("Setting").gameObject;
            propertyInfo.InitUI(property.transform);
            equipInfo.InitUI(equipInfoObj.transform);
            setting.InitUI(settingObj.transform);
            Button[] funcBtns = infoPanel.Find("Funcs").GetComponentsInChildren<Button>(true);         
            foreach (var item in funcBtns)
            {
                switch (item.name)
                {
                    case "Btn_Property":
                    case "Btn_Performance":
                    case "Btn_State":
                        UIHover uiHover1 = item.gameObject.AddComponent<UIHover>();
                        uiHover1.hoverObj = property;
                        uiHover1.OnEnterAction = SetPropertyInfo;
                        break;
                    case "Btn_Setting":
                        item.onClick.AddListener(()=> {
                            HiddleSetting(true);
                        });
                        break;
                    default:
                        break;
                }
            }

            Toggle[] toggles = infoPanel.Find("EquipmentItems").GetComponentsInChildren<Toggle>(true);
            foreach (var item in toggles)
            {
                UIHover uiHover = item.gameObject.AddComponent<UIHover>();
                uiHover.hoverObj = equipInfoObj;
                EquipmentItem equipmentItem = new EquipmentItem(item.transform);
                uiHover.OnEnterAction = equipmentItem.Show;
                equipmentItem.OnEnterAction = SetEquipmentItem;
                equipmentItems.Add(equipmentItem);
            }
            Open(false);
        }

        private void Open(bool isOpen)
        {
            openObj.SetActive(!isOpen);
            infoPanel.gameObject.SetActive(isOpen);
        }

        public void SetMain(ZhiHuiDuanMain zhiHuiDuanMain)
        {
            this.zhiHuiDuanMain = zhiHuiDuanMain;
        }

        public void SetData(BObjectModel model)
        {
            if (model != null)
            {
                this.bObjectModel = model;
                this.title.text = model.BObject.Info.Name;
                this.icon.sprite = DirectDeductionMgr.GetInstance.GetComIcon(model.BObject.Id);
                //this.type.text = model.BObject.Info.;
                string state = "";
                Debug.Log("CurStatus=== " + this.zhiHuiDuanMain.CurStatus);
                foreach (var item in this.zhiHuiDuanMain.StatusEnums)
                {
                    Debug.Log(string.Format("Enum=== {0}-----Description=== {1}", item.Enum, item.Description));
                    if (item.Enum == this.zhiHuiDuanMain.CurStatus)
                    {
                        state = item.Description;
                        break;
                    }
                }
                this.state.text = state;
                string[] strArr = DMTools.ToLonLat(model.transform.position, LonLatType.Degree).Split(',');
                this.coordsX.text = strArr[0];
                this.coordsY.text = strArr[1];

                //equipmentItems
                //model.BObject.
                Open(true);
            }
        }

        public void SetEquipmentItem()//此处需要传值
        {
            //equipInfo.Set()
        }

        private void SetPropertyInfo(string btnName)
        {
            switch (btnName)
            {
                case "Btn_Property":
                    SetProperty();
                    break;
                case "Btn_Performance":
                    SetPerformance();
                    break;
                case "Btn_State":
                    SetState();
                    break;
                default:
                    break;
            }
        }

        private void SetProperty()
        {
            List<DynamicProperty> dProperties = new List<DynamicProperty>();
            if (bObjectModel != null) 
            {
                dProperties = bObjectModel.GetEquipmentProperty();
            }
            propertyInfo.Set("基本属性", dProperties);
        }
        private void SetPerformance()
        {
            List<DynamicProperty> dProperties = new List<DynamicProperty>();
            if (bObjectModel != null)
            {
                dProperties = bObjectModel.GetEquipmentProperty();
            }
            propertyInfo.Set("性能", dProperties);
        }

        private void SetState()
        {
            List<DynamicProperty> dProperties = new List<DynamicProperty>()
            {
            //全局属性[0]
            new LabelProperty("全局属性"),
            new InputStringProperty("任务名", "测试任务"),
            new ToggleProperty("显示全局环境", false),
            new DropDownSceneSelectProperty("关联环境组件"),
            new OpenFileDialogProperty("评估报告路径","路径"),
            new ToggleProperty("启用经济系统", false),
            //组件属性[33]
            new LabelProperty("组件属性"),
            new ToggleProperty("显示状态信息", true),
            new ToggleProperty("显示选中装备效果", true),
            new ToggleProperty("显示全部装备效果", false),
            };

            if (bObjectModel != null)
            {
                
            }
            propertyInfo.Set("状态", dProperties);
        }

        public void HiddleSetting(bool isBtn = false)
        {
            if (setting.gameObject != null)
            {
                if (!isBtn && !setting.gameObject.activeInHierarchy) return;
                isTrue = !isTrue;
                setting.gameObject.SetActive(isBtn && isTrue);
            }
        }

        public void OnDestroy()
        {
            equipmentItems.Clear();
            dProperties.Clear();
        }

        [System.Serializable]
        public class EquipmentItem
        {
            public System.Action OnEnterAction;

            public int Num => int.Parse(num.text);

            private Image icon;
            private Text num;

            public EquipmentItem(Transform trans)
            {
                this.icon = trans.Find("Icon").GetComponent<Image>();
                this.num = trans.Find("Num").GetComponent<Text>();
            }

            public void Set(Sprite icon, string num)
            {
                this.icon.sprite = icon;
                this.num.text = num;
            }

            public void Show(string str)
            {
                OnEnterAction?.Invoke();
            }
        }

        [System.Serializable]
        public class PropertyInfo : InfoBase
        {
            private Text title;
            private RectTransform content;
            private RectTransform rect;
            private Vector2 rectStartSize = new Vector2();

            protected override void OnInitUI()
            {
                base.OnInitUI();
                rect = (RectTransform)transform;
                rectStartSize = rect.sizeDelta;
                title = transform.Find("Bg/Title").GetComponent<Text>();
                content = transform.Find("Content").GetComponent<RectTransform>();
            }

            public void Set(string title, List<DynamicProperty> dProperty)
            {
                this.title.text = title;
                ClearContent();
                foreach (var item in dProperty)
                {
                    DirectDeductionMgr.GetInstance.AddDynamicProperty(item, content);
                }

                if (content.childCount > 8)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                    rect.sizeDelta = new Vector2(rectStartSize.x, content.sizeDelta.y + 45);
                }
                else
                {
                    rect.sizeDelta = rectStartSize;
                }
            }

            public void ClearContent()
            {
                int count = content.childCount;
                for (int i = 0; i < count; i++)
                {
                    GameObject.DestroyImmediate(content.GetChild(0).gameObject);
                }
            }
        }
        
        [System.Serializable]
        public class EquipInfo : InfoBase
        {
            public System.Action OnEnterAction;

            private Text title;
            private Image icon;
            private RectTransform content;
            private RectTransform rect;
            private Vector2 rectStartSize = new Vector2();

            protected override void OnInitUI()
            {
                base.OnInitUI();
                rect = (RectTransform)transform;
                rectStartSize = rect.sizeDelta;
                title = transform.Find("Bg/Title").GetComponent<Text>();
                icon = transform.Find("Bg/IconBg/Icon").GetComponent<Image>();
                content = transform.Find("Content").GetComponent<RectTransform>();
            }

            public void Show()
            {
                OnEnterAction?.Invoke();
            }

            public void Set(string title, Sprite icon, List<DynamicProperty> dProperty)
            {
                this.title.text = title;
                this.icon.sprite = icon;
                ClearContent();
                foreach (var item in dProperty)
                {
                    DirectDeductionMgr.GetInstance.AddDynamicProperty(item, content);
                }

                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
                if (content.childCount > 8)
                {
                    rect.sizeDelta = new Vector2(rectStartSize.x, content.sizeDelta.y + 5);
                }
                else
                {
                    rect.sizeDelta = rectStartSize;
                }
            }

            public void ClearContent()
            {
                int count = content.childCount;
                for (int i = 0; i < count; i++)
                {
                    GameObject.Destroy(content.GetChild(0).gameObject);
                }
            }
        }

        [System.Serializable]
        public class Setting : InfoBase
        {
            public bool ShowStatusInfo { get; set; } = false;
            public bool ShowSelectedEquipEffect { get; set; } = false;
            public bool ShowAllEquipEffect { get; set; } = false;

            protected override void OnInitUI()
            {
                base.OnInitUI();
                Transform content = transform.Find("Content");
                ToggleProperty togglePro1 = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("组件属性", "显示状态信息");
                ToggleProperty togglePro2 = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("组件属性", "显示选中装备效果");
                ToggleProperty togglePro3 = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("组件属性", "显示全部装备效果");
                Toggle[] toggles = content.GetComponentsInChildren<Toggle>();
                foreach (var item in toggles)
                {
                    item.onValueChanged.AddListener((isOn)=> {
                        ToggleOnValueChanged(item);
                    });
                    switch (item.name)
                    {
                        case "Toggle_ShowStatusInfo":
                            item.isOn = togglePro1.Value;
                            break;
                        case "Toggle_ShowSelectedEquipEffect":
                            item.isOn = togglePro2.Value;
                            break;
                        case "Toggle_ShowAllEquipEffect":
                            item.isOn = togglePro3.Value;
                            break;
                        default:
                            break;
                    }
                }
            }

            private void ToggleOnValueChanged(Toggle toggle)
            {
                Debug.Log(string.Format("OnValueChanged---toggle= {0}---> {1}", toggle.name, toggle.isOn));
                switch (toggle.name)
                {
                    case "Toggle_ShowStatusInfo":
                        ShowStatusInfo = toggle.isOn;
                        break;
                    case "Toggle_ShowSelectedEquipEffect":
                        ShowSelectedEquipEffect = toggle.isOn;
                        break;
                    case "Toggle_ShowAllEquipEffect":
                        ShowAllEquipEffect = toggle.isOn;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
