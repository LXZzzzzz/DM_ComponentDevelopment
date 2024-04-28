using DM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    public class DirectDeductionMgr
    {

        private static DirectDeductionMgr instance;

        public static DirectDeductionMgr GetInstance
        {
            get
            {
                if (instance == null)
                    instance = new DirectDeductionMgr();
                return instance;
            }
        }

        private Dictionary<string, Dictionary<string, DynamicProperty>> dicDynamicGroups = new Dictionary<string, Dictionary<string, DynamicProperty>>();

        Dictionary<string, Sprite> picBobjects = new Dictionary<string, Sprite>();

        public void InitProperties(DynamicProperty[] properties)
        {
            AddProperties("全局属性","任务名", properties[1]);
            AddProperties("全局属性", "显示全局环境", properties[2]);
            AddProperties("全局属性", "关联环境组件", properties[3]);
            AddProperties("全局属性", "评估报告路径", properties[4]);
            AddProperties("全局属性", "启用经济系统", properties[5]);
            AddProperties("小地图", "数据源", properties[7]);
            AddProperties("小地图", "分组依据", properties[8]);
            AddProperties("小地图", "分组列表配置", properties[9]);
            AddProperties("小地图", "自定义类型配置", properties[10]);
            AddProperties("小地图", "组件图标设置", properties[11]);
            AddProperties("小地图", "通信消息配置", properties[12]);
            AddProperties("小地图", "显示组件名称", properties[13]);
            AddProperties("小地图", "显示选中状态", properties[14]);
            AddProperties("小地图", "是否显示朝向", properties[15]);
            AddProperties("消息属性", "通信消息配置", properties[17]);
            AddProperties("消息属性", "显示通用消息", properties[18]);
            AddProperties("消息属性", "显示指令消息", properties[19]);
            AddProperties("任务属性", "数据源XML路径", properties[21]);
            AddProperties("控制属性", "初始倍速", properties[23]);
            AddProperties("控制属性", "倍速集合", properties[24]);
            AddProperties("控制属性", "时间参数", properties[25]);
            //AddProperties("指令属性", "", properties[33]);
            AddProperties("分组属性", "数据源", properties[28]);
            AddProperties("分组属性", "分组依据", properties[29]);
            AddProperties("分组属性", "类型分组配置", properties[30]);
            AddProperties("分组属性", "场景分组配置", properties[31]);
            AddProperties("分组属性", "是否显示隐藏组件", properties[32]);
            AddProperties("组件属性", "显示状态信息", properties[34]);
            AddProperties("组件属性", "显示选中装备效果", properties[35]);
            AddProperties("组件属性", "显示全部装备效果", properties[36]);
        }

        private void AddProperties(string groupKey, string propertyKey, DynamicProperty dProperty)
        {
            if (dicDynamicGroups.ContainsKey(groupKey))
            {
                if (dicDynamicGroups[groupKey].ContainsKey(propertyKey))
                {
                    dicDynamicGroups[groupKey][propertyKey] = dProperty;
                }
                else
                {
                    dicDynamicGroups[groupKey].Add(propertyKey, dProperty);
                }
            }
            else
            {
                Dictionary<string, DynamicProperty> dic2 = new Dictionary<string, DynamicProperty>();
                dic2.Add(propertyKey, dProperty);
                dicDynamicGroups.Add(groupKey, dic2);
            }
        }

        public DynamicProperty GetProperties(string groupKey, string propertiesKey)
        {
            if (dicDynamicGroups.ContainsKey(groupKey))
            {
                if (dicDynamicGroups[groupKey].ContainsKey(propertiesKey))
                {
                    return dicDynamicGroups[groupKey][propertiesKey];
                }
            }
            return null;
        }

        public void InitComIcon(Dictionary<string, Sprite> picBobjects)
        {
            this.picBobjects = picBobjects;
        }

        public Sprite GetComIcon(string comId)
        {
            picBobjects.TryGetValue(comId, out Sprite sprite);
            Debug.Log("--------------------sprite---= " + sprite);
            return sprite;
        }

        /// <summary>
        /// 添加动态属性
        /// </summary>
        /// <param name="dProperty">动态属性</param>
        /// <param name="parent">父级对象</param>
        /// <returns></returns>
        public GameObject AddDynamicProperty(DynamicProperty dProperty, Transform parent)
        {
            Debug.Log(dProperty.Name + "--->" + dProperty.Type);

            GameObject obj = UIDirectDeduction.Instance.propertyPrefabs.GetPropertyPrefab(dProperty.Type);
            GameObject propertyObj = GameObject.Instantiate(obj, parent);
            DynamicPropertyItem propertyItem = propertyObj.GetComponent<DynamicPropertyItem>();
            propertyItem.m_dProperty = dProperty;
            propertyItem.m_dataLabel.text = dProperty.Name;
            if (obj.name == "Property_Default")
            {
                propertyItem.m_dataLabel.text = string.Format("暂不支持“{0}”类型", dProperty.Type.ToString());
            }
            switch (propertyItem.DataPropertyType)
            {
                case PropertyType.DropDown:
                    propertyItem.m_dataDropdown.options.Clear();
                    DropDownProperty ddProperty = (DropDownProperty)dProperty;
                    foreach (var item in ddProperty.Context)
                    {
                        Dropdown.OptionData option = new Dropdown.OptionData();
                        option.text = item.Description;
                        propertyItem.m_dataDropdown.options.Add(option);
                        propertyItem.dropdownIdCache.Add(propertyItem.m_dataDropdown.options.Count - 1, item.Enum);
                    }
                    foreach (var item in propertyItem.dropdownIdCache)
                    {
                        if (item.Value.Equals(ddProperty.Selected.Enum))
                        {
                            propertyItem.m_dataDropdown.value = item.Key;
                            break;
                        }
                    }
                    break;
                case PropertyType.Toggle:
                    ToggleProperty togProperty = (ToggleProperty)dProperty;
                    propertyItem.m_dataToggle.isOn = togProperty.Value;
                    break;
                case PropertyType.Slider:
                    SliderProperty sliProperty = (SliderProperty)dProperty;
                    propertyItem.m_dataSlider.minValue = sliProperty.Min;
                    propertyItem.m_dataSlider.maxValue = sliProperty.Max;
                    propertyItem.m_dataSlider.value = sliProperty.Value;
                    propertyItem.m_dataSliderLabel.text = string.Format("{0}/{1}", sliProperty.Value, sliProperty.Max);
                    break;
                case PropertyType.IntSlider:
                    IntSliderProperty intSliProperty = (IntSliderProperty)dProperty;
                    propertyItem.m_dataSlider.minValue = intSliProperty.Min;
                    propertyItem.m_dataSlider.maxValue = intSliProperty.Max;
                    propertyItem.m_dataSlider.value = intSliProperty.Value;
                    propertyItem.m_dataSliderLabel.text = string.Format("{0}/{1}", intSliProperty.Value, intSliProperty.Max);
                    break;
                case PropertyType.InputInt:
                    InputIntProperty iIntProperty = (InputIntProperty)dProperty;
                    propertyItem.m_dataInputBox.text = iIntProperty.Value.ToString();
                    break;
                case PropertyType.InputFloat:
                    InputFloatProperty iFloatProperty = (InputFloatProperty)dProperty;
                    propertyItem.m_dataInputBox.text = iFloatProperty.Value.ToString();
                    break;
                case PropertyType.InputIntUnit:
                    InputIntUnitProperty iIntUnitProperty = (InputIntUnitProperty)dProperty;
                    propertyItem.m_dataInputBox.text = iIntUnitProperty.Value.ToString();
                    propertyItem.m_unitLabel.text = iIntUnitProperty.Unit;
                    break;
                case PropertyType.InputFloatUnit:
                    InputFloatUnitProperty iFloatUnitProperty = (InputFloatUnitProperty)dProperty;
                    propertyItem.m_dataInputBox.text = iFloatUnitProperty.Value.ToString();
                    propertyItem.m_unitLabel.text = iFloatUnitProperty.Unit;
                    break;
                case PropertyType.InputIntUnitLimit:
                    DynamicPropertyUnitLimitItem intLimitItem = (DynamicPropertyUnitLimitItem)propertyItem;
                    InputIntUnitLimitProperty iIntUnitLimitProperty = (InputIntUnitLimitProperty)dProperty;
                    intLimitItem.m_dataInputBox.text = iIntUnitLimitProperty.Value.ToString();
                    string intLimitStr = string.Format("({0}-{1})", iIntUnitLimitProperty.Min.ToString(), iIntUnitLimitProperty.Max.ToString());
                    intLimitItem.m_textLimit.text = intLimitStr;
                    intLimitItem.m_unitLabel.text = iIntUnitLimitProperty.Unit;
                    break;
                case PropertyType.InputFloatUnitLimit:
                    DynamicPropertyUnitLimitItem floatLimitItem = (DynamicPropertyUnitLimitItem)propertyItem;
                    InputFloatUnitLimitProperty iFloatUnitLimitProperty = (InputFloatUnitLimitProperty)dProperty;
                    floatLimitItem.m_dataInputBox.text = iFloatUnitLimitProperty.Value.ToString();
                    string floatLimitStr = string.Format("({0}-{1})", iFloatUnitLimitProperty.Min.ToString(), iFloatUnitLimitProperty.Max.ToString());
                    floatLimitItem.m_textLimit.text = floatLimitStr;
                    floatLimitItem.m_unitLabel.text = iFloatUnitLimitProperty.Unit;
                    break;
                case PropertyType.InputString:
                    InputStringProperty iStringProperty = (InputStringProperty)dProperty;
                    propertyItem.m_dataInputBox.text = iStringProperty.Value;
                    break;
                default:
                    break;
            }
            return propertyObj;
        }

        public void Clear()
        {
            dicDynamicGroups.Clear();
            picBobjects.Clear();
        }
    }

    #region 枚举类型

    /// <summary>
    /// 要素类型
    /// </summary>
    public enum ElementType
    {
        /// <summary>
        /// 救援力量
        /// </summary>
        RescueForces = 0,
        /// <summary>
        /// 遇险事件
        /// </summary>
        DistressIncident,
        /// <summary>
        /// 人在环干预
        /// </summary>
        Intervention,
        /// <summary>
        /// 框选要素
        /// </summary>
        BoxSelectedElements,
        /// <summary>
        /// 装备信息
        /// </summary>
        EquipmentInfo,
    }

    #endregion
}
