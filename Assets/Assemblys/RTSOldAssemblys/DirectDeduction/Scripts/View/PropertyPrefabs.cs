using DM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace 指挥端
{
    [System.Serializable]
    public class PropertyPrefabs
    {
        private GameObject m_proDefault;
        private GameObject m_proLabel;
        private GameObject m_proInputString;
        private GameObject m_proInputFloat;
        private GameObject m_proInputFloatUnit;
        private GameObject m_proInputFloatUnitLimit;
        private GameObject m_proInputInt;
        private GameObject m_proInputIntUnit;
        private GameObject m_proInputIntUnitLimit;
        private GameObject m_proSlider;
        private GameObject m_proSliderInt;
        private GameObject m_proDropdown;
        private GameObject m_proToggle;

        public void Init(Transform propertys)
        {
            m_proDefault = propertys.Find("Property_Default").gameObject;
            m_proLabel = propertys.Find("Property_Label").gameObject;
            m_proInputString = propertys.Find("Property_InputString").gameObject;
            m_proInputFloat = propertys.Find("Property_InputFloat").gameObject;
            m_proInputFloatUnit = propertys.Find("Property_InputFloatUnit").gameObject;
            m_proInputFloatUnitLimit = propertys.Find("Property_InputFloatUnitLimit").gameObject;
            m_proInputInt = propertys.Find("Property_InputInt").gameObject;
            m_proInputIntUnit = propertys.Find("Property_InputIntUnit").gameObject;
            m_proInputIntUnitLimit = propertys.Find("Property_InputIntUnitLimit").gameObject;
            m_proSlider = propertys.Find("Property_Slider").gameObject;
            m_proSliderInt = propertys.Find("Property_SliderInt").gameObject;
            m_proDropdown = propertys.Find("Property_Dropdown").gameObject;
            m_proToggle = propertys.Find("Property_Toggle").gameObject;

            DynamicPropertyItem dy0 = m_proDefault.AddComponent<DynamicPropertyItem>();
            dy0.m_dataPropertyType = PropertyType.Label;
            DynamicPropertyItem dy1 = m_proLabel.AddComponent<DynamicPropertyItem>();
            dy1.m_dataPropertyType = PropertyType.Label;
            DynamicPropertyItem dy2 = m_proInputString.AddComponent<DynamicPropertyItem>();
            dy2.m_dataPropertyType = PropertyType.InputString;
            DynamicPropertyItem dy3 = m_proInputFloat.AddComponent<DynamicPropertyItem>();
            dy3.m_dataPropertyType = PropertyType.InputFloat;
            DynamicPropertyItem dy4 = m_proInputFloatUnit.AddComponent<DynamicPropertyItem>();
            dy4.m_dataPropertyType = PropertyType.InputFloatUnit;
            DynamicPropertyUnitLimitItem dy5 = m_proInputFloatUnitLimit.AddComponent<DynamicPropertyUnitLimitItem>();
            dy5.m_dataPropertyType = PropertyType.InputFloatUnitLimit;
            DynamicPropertyItem dy6 = m_proInputInt.AddComponent<DynamicPropertyItem>();
            dy6.m_dataPropertyType = PropertyType.InputInt;
            DynamicPropertyItem dy7 = m_proInputIntUnit.AddComponent<DynamicPropertyItem>();
            dy7.m_dataPropertyType = PropertyType.InputIntUnit;
            DynamicPropertyUnitLimitItem dy8 = m_proInputIntUnitLimit.AddComponent<DynamicPropertyUnitLimitItem>();
            dy8.m_dataPropertyType = PropertyType.InputIntUnitLimit;
            DynamicPropertyItem dy9 = m_proSlider.AddComponent<DynamicPropertyItem>();
            dy9.m_dataPropertyType = PropertyType.Slider;
            DynamicPropertyItem dy10 = m_proSliderInt.AddComponent<DynamicPropertyItem>();
            dy10.m_dataPropertyType = PropertyType.IntSlider;
            DynamicPropertyItem dy11 = m_proDropdown.AddComponent<DynamicPropertyItem>();
            dy11.m_dataPropertyType = PropertyType.DropDown;
            DynamicPropertyItem dy12 = m_proToggle.AddComponent<DynamicPropertyItem>();
            dy12.m_dataPropertyType = PropertyType.Toggle;
        }

        /// <summary>
        /// 获取属性预制体
        /// </summary>
        public GameObject GetPropertyPrefab(PropertyType propertyType)
        {
            GameObject obj = m_proDefault;
            switch (propertyType)
            {
                case PropertyType.Label:
                    obj = m_proLabel;
                    break;
                case PropertyType.InputString:
                    obj = m_proInputString;
                    break;
                case PropertyType.InputFloat:
                    obj = m_proInputFloat;
                    break;
                case PropertyType.InputFloatUnit:
                    obj = m_proInputFloatUnit;
                    break;
                case PropertyType.InputFloatUnitLimit:
                    obj = m_proInputFloatUnitLimit;
                    break;
                case PropertyType.InputInt:
                    obj = m_proInputInt;
                    break;
                case PropertyType.InputIntUnit:
                    obj = m_proInputIntUnit;
                    break;
                case PropertyType.InputIntUnitLimit:
                    obj = m_proInputIntUnitLimit;
                    break;
                case PropertyType.Slider:
                    obj = m_proSlider;
                    break;
                case PropertyType.IntSlider:
                    obj = m_proSliderInt;
                    break;
                case PropertyType.DropDown:
                    obj = m_proDropdown;
                    break;
                case PropertyType.Toggle:
                    obj = m_proToggle;
                    break;
                default:
                    break;
            }

            return obj;
        }

    }
}
