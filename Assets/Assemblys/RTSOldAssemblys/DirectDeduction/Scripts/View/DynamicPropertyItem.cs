using DM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    public class DynamicPropertyItem : DMonoBehaviour
    {
        public bool IsShowInputBox { get; set; } = true;

        [HideInInspector]
        public DynamicProperty m_dProperty;//该属性引用
        [HideInInspector]
        public Text m_unitLabel;
        [HideInInspector]
        public Text m_dataLabel;
        [HideInInspector]
        public Toggle m_dataToggle;
        [HideInInspector]
        public InputField m_dataInputBox;
        [HideInInspector]
        public Dropdown m_dataDropdown;
        [HideInInspector]
        public Slider m_dataSlider;
        [HideInInspector]
        public Text m_dataSliderLabel;
        [HideInInspector]
        public Dictionary<int, int> dropdownIdCache = new Dictionary<int, int>();

        public PropertyType m_dataPropertyType;
        public PropertyType DataPropertyType
        {
            get { return m_dataPropertyType; }
        }
        void Awake()
        {
            if (null == m_unitLabel && transform.Find("UnitLab"))
                m_unitLabel = transform.Find("UnitLab").GetComponent<Text>();
            if (null == m_dataLabel && transform.Find("DataLab"))
                m_dataLabel = transform.Find("DataLab").GetComponent<Text>();
            this.OnAwake();
            Init();
        }
        void Start()
        {
            if (m_unitLabel != null)
            {
                bool isShow = DataPropertyType == PropertyType.InputFloatUnit ||
                              DataPropertyType == PropertyType.InputIntUnit ||
                              DataPropertyType == PropertyType.InputFloatUnitLimit ||
                              DataPropertyType == PropertyType.InputIntUnitLimit;
                m_unitLabel.gameObject.SetActive(isShow);
            }
            if (transform.parent.name.Equals("SceneDynamicProperty"))
            {
                transform.GetChild(0).GetComponent<RectTransform>().sizeDelta += new Vector2(140, 0);
                transform.GetChild(1).localPosition += new Vector3(140, 0, 0);
            }
            this.OnStart();
            if(m_dataInputBox != null)
            {
                //m_dataInputBox.GetComponent<Image>().enabled = IsShowInputBox;
                m_dataInputBox.interactable = IsShowInputBox;
            }
        }

        private void Init()
        {
            switch (m_dataPropertyType)
            {
                case PropertyType.Label:
                    if (null == m_dataLabel)
                        m_dataLabel = GetComponentInChildren<Text>();
                    break;
                case PropertyType.Toggle:
                    if (null == m_dataToggle)
                        m_dataToggle = GetComponentInChildren<Toggle>();
                    break;
                case PropertyType.InputInt:
                case PropertyType.InputFloat:
                case PropertyType.InputString:
                case PropertyType.ButtonInputStr:
                case PropertyType.InputIntUnit:
                case PropertyType.InputFloatUnit:
                case PropertyType.InputIntUnitLimit:
                case PropertyType.InputFloatUnitLimit:
                    if (null == m_dataInputBox)
                        m_dataInputBox = GetComponentInChildren<InputField>();
                    break;
                case PropertyType.DropDown:
                case PropertyType.DropDownSceneBObjects:
                    if (null == m_dataDropdown)
                        m_dataDropdown = GetComponentInChildren<Dropdown>();
                    break;
                case PropertyType.Slider:
                case PropertyType.IntSlider:
                    if (null == m_dataSlider)
                        m_dataSlider = GetComponentInChildren<Slider>();
                    if (null == m_dataSliderLabel)
                        m_dataSliderLabel = transform.Find("Label").GetComponent<Text>();
                    if (null == m_dataInputBox)
                        m_dataInputBox = GetComponentInChildren<InputField>();
                    m_dataSlider.onValueChanged.AddListener(SliderOnValueChanged);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 设置单位
        /// </summary>
        /// <param name="unit"></param>
        public void SetUnit(string unit)
        {
            if (m_unitLabel != null)
            {
                if (!m_unitLabel.gameObject.activeInHierarchy)
                    m_unitLabel.gameObject.SetActive(true);
                m_unitLabel.text = unit;
            }
        }

        public void SliderOnValueChanged(float value)
        {
            if (m_dataSliderLabel != null && m_dataSliderLabel.text.Contains("/"))
            {
                string[] labs = m_dataSliderLabel.text.Split('/');
                m_dataSliderLabel.text = string.Format("{0}/{1}", value.ToString("F1"), labs[1]);
            }
        }

        public void ShowSliderValue(bool isSlider)
        {
            if (m_dataSlider && m_dataInputBox)
            {
                if (m_dataSlider.value == float.Parse(m_dataInputBox.text)) return;
                if (isSlider)
                    m_dataInputBox.text = (m_dataSlider.value).ToString();
                else
                {
                    m_dataSlider.value = float.Parse(m_dataInputBox.text);
                }
            }
        }

        public void OnDestroy()
        {
            dropdownIdCache.Clear();
        }

        protected virtual void OnAwake() { }
        protected virtual void OnStart() { }
    }
}
