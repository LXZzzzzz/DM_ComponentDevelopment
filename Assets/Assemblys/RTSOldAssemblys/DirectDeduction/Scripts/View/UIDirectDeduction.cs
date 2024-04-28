using DM.Core;
using DM.Core.Map;
using DM.Entity;
using DM.IFS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace 指挥端
{
    public class UIDirectDeduction : DMonoBehaviour, IPointerClickHandler
    {
        public float Multiple => float.Parse(m_multiple.captionText.text);

        public bool IsMask { get; private set; } = false;

        public SelectBox selectBox;

        private Transform m_bobjectImages;
        private GameObject m_uiMask;//UI遮罩
        private Text m_textTitle;
        private Dropdown m_multiple;
        private Toggle m_toggleStop;
        private Transform m_globalUI;
        private Transform m_weatherInfoParent;

        #region 自定义类

        //New
        [Header("组件列表")]
        public MissionElementPanel missionElementPanel = new MissionElementPanel();
        [Header("元素信息")]
        public ElementInfoPanel elementInfoPanel = new ElementInfoPanel();
        [Header("救援过程")]
        public RescueProcessPanel rescueProcessPanel = new RescueProcessPanel();
        [Header("小地图")]
        public MiniMap miniMap = new MiniMap();
        [Header("控制指令")]
        public ControlInstructionPanel ctrlInstructionPanel = new ControlInstructionPanel();
        [Header("指令参数")]
        public InstructionParamPanel instructionParamPanel = new InstructionParamPanel();
        [Header("任务列表")]
        public MissionListPanel missionListPanel;

        [Header("预制体")]
        public PropertyPrefabs propertyPrefabs = new PropertyPrefabs();
        #endregion

        #region Prefabs
        private GameObject m_imageBobject;//组件图标

        #endregion

        private SceneInfo sceneInfo;

        public static UIDirectDeduction Instance { get; private set; }

        private void Awake()
        {
            if (null == Instance) Instance = this;
            m_bobjectImages = transform.Find("BobjectImages");
            m_imageBobject = transform.Find("Prefabs/ImageBobject").gameObject;
            m_textTitle = transform.Find("TopMenu/Title").GetComponent<Text>();
            
            //全局UI
            m_globalUI = transform.Find("GlobalUI");
            m_uiMask = m_globalUI.Find("Mask").gameObject;
            m_multiple = m_globalUI.Find("Operation").GetComponentInChildren<Dropdown>();
            //天气信息挂点
            m_weatherInfoParent = m_globalUI.Find("WeatherlInfo");
            //New
            propertyPrefabs.Init(transform.Find("Propertys"));
            //组件列表
            missionElementPanel.InitUI(m_globalUI.Find("MissionElementPanel"));
            //元素信息
            elementInfoPanel.InitUI(m_globalUI.Find("ElementInfoPanel"));
            //救援过程
            rescueProcessPanel.InitUI(m_globalUI.Find("RescueProcessPanel"));
            //小地图
            miniMap.InitUI(m_globalUI.Find("MiniMap"));
            //控制指令
            ctrlInstructionPanel.InitUI(m_globalUI.Find("ControlInstructionPanel"));
            //指令参数
            instructionParamPanel.InitUI(m_globalUI.Find("InstructionParamPanel"));
            //任务列表
            missionListPanel = m_globalUI.Find("MissionListPanel").gameObject.AddComponent<MissionListPanel>();
        }

        private void OnEnable()
        {
            if (selectBox != null) selectBox.enabled = true;
        }

        private void OnDisable()
        {
            if (selectBox != null) selectBox.enabled = false;
        }

        private void Start()
        {
            if (null == selectBox) selectBox = gameObject.AddComponent<SelectBox>();
            InitUI();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                elementInfoPanel.HiddleSetting();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                CreateInstructLabelItem("测试组件名", "指令名", "执行任务");
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                CreateInstructLabelItem("测试组件名", "指令名", "执行任务", "系统时间");
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                string text2 = DMTools.SetRichText("测试组件名", RichTextType.Blod);
                string text3 = DMTools.SetRichText("执行任务测试测试测试测试执行任务测试测试测试测试", RichTextType.Font, 15);
                string text1 = DMTools.SetRichText("测试测试执行任务测试测试测试测试", RichTextType.Color, 12, "red");
                CreateInstructLabelItem(text2, "指令名", string.Format("{0}{1}",text1, text3));
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                CreateNormalLabelItem("测试测试：组件名 执行任务");
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                CreateNormalLabelItem("测试测试：组件名 执行任务", "系统时间");
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                string text1 = DMTools.SetRichText("测试测试：", RichTextType.Color, 12, "red");
                string text2 = DMTools.SetRichText("组件名", RichTextType.Blod);
                string text3 = DMTools.SetRichText("执行任务", RichTextType.Font, 15);
                string text4 = DMTools.SetRichText("执行任务执行任务执行任务执行任务执行任务执行任务执行任务执行任务执行任务执行任务");
                CreateNormalLabelItem(string.Format("{0}{1}{2}{3}", text1, text2, text3, text4));
            }
        }

        private void InitUI()
        {
            //弹窗
            Transform popupWindows = transform.Find("PopupWindows");
            Button[] btns = popupWindows.GetComponentsInChildren<Button>(true);
            foreach (var item in btns)
            {
                item.onClick.AddListener(() => { ClickCallBack(item.name); });
            }
            Toggle[] toggles = popupWindows.GetComponentsInChildren<Toggle>(true);
            foreach (var item in toggles)
            {
                item.onValueChanged.AddListener((isOn) => { OnValueChanged(item); });
            }
            //顶部UI
            Transform topMenu = transform.Find("TopMenu");
            btns = topMenu.GetComponentsInChildren<Button>(true);
            foreach (var item in btns)
            {
                if (item.name.Equals("Btn_Setting"))
                {
                    GameObject subBtns = item.transform.Find("SubBtns").gameObject;
                    subBtns.AddComponent<UIHover>().onlyHiddleObj = subBtns;
                }
                item.onClick.AddListener(() => { ClickCallBack(item.name); });
            }
            GameObject operation = m_globalUI.Find("Operation").gameObject;
            btns = operation.GetComponentsInChildren<Button>(true);
            foreach (var item in btns)
            {
                item.onClick.AddListener(() => { ClickCallBack(item.name); });
            }
            m_multiple.onValueChanged.AddListener((value) =>
            {
                Debug.Log("--------------Multiple=== " + Multiple);
            });
        }

        public void InitData(BObjectModel selectedRole, SceneInfo info)
        {
            if (sceneInfo != null) return;
            sceneInfo = info;

            elementInfoPanel.SetMain((ZhiHuiDuanMain)main);

            InputStringProperty inputStr = (InputStringProperty)DirectDeductionMgr.GetInstance.GetProperties("全局属性", "任务名");
            SetTitle(inputStr.Value);
            ToggleProperty toggle = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("全局属性", "显示全局环境");
            m_weatherInfoParent.gameObject.SetActive(toggle.Value);
            OpenFileDialogProperty filePro = (OpenFileDialogProperty)DirectDeductionMgr.GetInstance.GetProperties("任务属性", "数据源XML路径");
            missionListPanel.SetData(filePro.Path);
            InputStringProperty inputStr2 = (InputStringProperty)DirectDeductionMgr.GetInstance.GetProperties("控制属性", "倍速集合");
            InitMultiple(inputStr2.Value);
            
            // 救援力量:8_1,8_2;遇险事件:9_5;
            InputStringProperty inputStr3 = (InputStringProperty)DirectDeductionMgr.GetInstance.GetProperties("分组属性", "类型分组配置");
            Dictionary<string, List<BObjectModel>> dic1 = new Dictionary<string, List<BObjectModel>>();
            string[] strArr = inputStr3.Value.Split(';');
            foreach (var item in strArr)
            {
                if (string.IsNullOrEmpty(item)) continue;
                string[] strArr2 = item.Split(':');
                if (string.IsNullOrEmpty(strArr2[1])) continue;
                string[] tagStrs = strArr2[1].Split(',');
                List<BObjectModel> bObjectModels = new List<BObjectModel>();
                foreach (var item2 in tagStrs)
                {
                    if (string.IsNullOrEmpty(item2)) continue;
                    string[] tags = item2.Split('_');
                    List<BObjectModel> tempList = allBObjects.FindBObjectsWithTag(tags[0], tags[1]);
                    Debug.Log(string.Format("tag= {0}---subTag= {1}---tempList= {2}", tags[0], tags[1], tempList.Count));
                    foreach (var item3 in tempList)
                    {
                        Debug.Log("-------------item3=== " + item3);
                        bObjectModels.Add(item3);
                    }
                }
                dic1.Add(strArr2[0], bObjectModels);
            }
            missionElementPanel.SetData(dic1);
            
            Debug.Log("------------------------sceneInfo.PicMap=== " + sceneInfo.PicMap);
            miniMap.SetData(sceneInfo.PicMap, "地图名称测试测试");
            //救援力量,Red,救援力量;蓝方,Blue,遇险事件;
            InputStringProperty inputStr4 = (InputStringProperty)DirectDeductionMgr.GetInstance.GetProperties("小地图", "分组列表配置");
            Dictionary<string, List<BObjectModel>> dic2 = new Dictionary<string, List<BObjectModel>>();
            string[] inputStr4Arr = inputStr4.Value.Split(';');
            foreach (var item in inputStr4Arr)
            {
                Debug.Log("--------------------inputStr4Arr.item---= " + item);
                if (string.IsNullOrEmpty(item)) continue;
                string[] str4s = item.Split(',');
                if (dic1.ContainsKey(str4s[2]))
                {
                    dic2.Add(item, dic1[str4s[2]]);
                }
                else
                {
                    //提示错误
                }
            }
            miniMap.SetData(dic2);
        }

        private void ClickCallBack(string btnName)
        {
            Debug.Log("ClickCallBack---btnName--->" + btnName);
            switch (btnName)
            {
                case "Btn_Play":
                    PlayOrStop(true);
                    break;
                case "Btn_Stop":
                    PlayOrStop(false);
                    break;
                case "Btn_Add":
                    SetMultiple(true);
                    break;
                case "Btn_Sub":
                    SetMultiple(false);
                    break;
                case "Btn_File":
                    break;
                case "Btn_Report":

                    break; 
                case "Btn_Setting":

                    break;
                case "Btn_Interface"://界面

                    break;
                case "Btn_Sound"://声音

                    break;
                case "Btn_Func"://功能

                    break;
                case "Btn_ConfirmInterface"://确认界面设置

                    break;
                case "Btn_ConfirmFunc"://确认功能

                    break;
                case "Btn_Quit":
                    transform.parent.Find("Camera").gameObject.SetActive(false);
                    gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }

        private void OnValueChanged(Toggle toggle)
        {
            Debug.Log(string.Format("OnValueChanged---toggle= {0}---> {1}", toggle.name, toggle.isOn));
            switch (toggle.name)
            {
                case "Toggle_ShowSelectedCom"://显示选中组件
                    
                    break;
                case "Toggle_ShowStatusInfo"://显示状态信息

                    break;
                case "Toggle_ShowSelectedEquipEffect"://显示选中装备效果

                    break;
                case "Toggle_ShowAllEquipEffect"://显示全部装备效果

                    break;
                case "Toggle_ShowTerrainAreaMark"://显示地形区域标识

                    break;
                case "Toggle_ShowTerrainAuxiliaryLine"://显示地形辅助线

                    break;
                case "Toggle_EnableEconomicSystem"://启用经济系统

                    break;
                default:
                    break;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount == 1)
            {
                if (!eventData.pointerPressRaycast.gameObject.name.Equals("Setting"))
                {
                    elementInfoPanel.HiddleSetting();
                }
            }
        }

        /// <summary>
        /// 设置任务名
        /// </summary>
        /// <param name="name"></param>
        public void SetTitle(string name)
        {
            m_textTitle.text = name;
        }

        /// <summary>
        /// 播放\暂停
        /// </summary>
        public void PlayOrStop(bool isPlay)
        {
            if (isPlay)
            {
                Debug.Log("播放");
            }
            else
            {
                Debug.Log("暂停");
            }
        }

        private void InitMultiple(string str)
        {
            InputIntProperty inputInt = (InputIntProperty)DirectDeductionMgr.GetInstance.GetProperties("控制属性", "初始倍速");
            int indexValue = 0;
            m_multiple.options.Clear();
            string[] values = str.Split(';');
            int[] numValues = new int[values.Length];
            //转数据
            for (int i = 0; i < values.Length; i++)
            {
                if (!int.TryParse(values[i], out numValues[i]))
                {
                    Debug.LogError("倍速集合：输入数据错误！！！");
                    return;
                }
            }
            //排序
            for (int j = 1; j <= numValues.Length - 1; j++)
            {
                for (int i = 0; i < numValues.Length - j; i++)
                {
                    if (numValues[i] > numValues[i + 1])
                    {
                        int temp = numValues[i];
                        numValues[i] = numValues[i + 1];
                        numValues[i + 1] = temp;
                    }
                }
            }
            //赋值
            for (int i = 0; i < numValues.Length; i++)
            {
                Dropdown.OptionData optionData = new Dropdown.OptionData()
                {
                    text = numValues[i].ToString(),
                };
                m_multiple.options.Add(optionData);
            }
            //对比初始倍速
            for (int i = 0; i < numValues.Length; i++)
            {
                if (inputInt.Value >= numValues[numValues.Length - 1])
                {
                    indexValue = numValues.Length - 1;
                    break;
                }
                else if (inputInt.Value == numValues[i])
                {
                    indexValue = i;
                    break;
                }
                else if (inputInt.Value < numValues[i])
                {
                    indexValue = i - 1;
                    break;
                }
            }

            indexValue = Mathf.Clamp(indexValue, 0, numValues.Length - 1);
            m_multiple.value = indexValue;
        }

        /// <summary>
        /// 设置倍数
        /// </summary>
        public void SetMultiple(bool isAdd)
        {
            int value = m_multiple.value;
            value += isAdd ? 1 : -1;
            value = Mathf.Clamp(value, 0, m_multiple.options.Count - 1);
            m_multiple.value = value;
        }

        public UIFixedObj SetBobjectIcons(BObjectModel bObjectModel)
        {
            GameObject comImageObj = GameObject.Instantiate<GameObject>(m_imageBobject, m_bobjectImages);
            UIFixedObj uiFixedObj = comImageObj.GetComponent<UIFixedObj>();
            if (uiFixedObj == null) uiFixedObj = comImageObj.AddComponent<UIFixedObj>();
            uiFixedObj.Model = bObjectModel;

            return uiFixedObj;
        }

        public void SetElementInfo(BObjectModel bobjModel)
        {
            elementInfoPanel.SetData(bobjModel);
            ctrlInstructionPanel.SetData(bobjModel);
        }

        /// <summary>
        /// 设置UI遮罩
        /// </summary>
        public void SetMask(bool isMask)
        {
            IsMask = isMask;
            m_uiMask.SetActive(isMask);
        }

        /// <summary>
        /// 救援过程普通标签
        /// </summary>
        public void CreateNormalLabelItem(string message, string timeStr = "任务时间")
        {
            ToggleProperty toggle = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("消息属性", "显示通用消息");
            if (!toggle.Value) return;
            DropDownProperty dropDown = (DropDownProperty)DirectDeductionMgr.GetInstance.GetProperties("控制属性", "时间参数");
            if (dropDown.Selected.Enum == 0)
            {
                timeStr = System.DateTime.Now.ToString("HH:mm:ss");
            }

            rescueProcessPanel.CreateNormalLabelItem(timeStr, message);
        }

        /// <summary>
        /// 救援过程指令标签
        /// </summary>
        public void CreateInstructLabelItem(string text1, string insName, string text2, string timeStr = "任务时间")
        {
            ToggleProperty toggle = (ToggleProperty)DirectDeductionMgr.GetInstance.GetProperties("消息属性", "显示指令消息");
            if (!toggle.Value) return;
            DropDownProperty dropDown = (DropDownProperty)DirectDeductionMgr.GetInstance.GetProperties("控制属性", "时间参数");
            if (dropDown.Selected.Enum == 0)
            {
                timeStr = System.DateTime.Now.ToString("HH:mm:ss");
            }
            
            rescueProcessPanel.CreateInstructLabelItem(timeStr, text1, insName, text2);
        }

        /// <summary>
        /// 设置全局UI显\隐
        /// </summary>
        public void SetActiveGlobalUI(bool active)
        {
            m_globalUI.gameObject.SetActive(!active);
            m_bobjectImages.gameObject.SetActive(!active);
        }

        private void OnDestroy()
        {
            DirectDeductionMgr.GetInstance.Clear();
            missionElementPanel.OnDestroy();
            elementInfoPanel.OnDestroy();
            miniMap.OnDestory();
            ctrlInstructionPanel.OnDestroy();
        }
    }
}
