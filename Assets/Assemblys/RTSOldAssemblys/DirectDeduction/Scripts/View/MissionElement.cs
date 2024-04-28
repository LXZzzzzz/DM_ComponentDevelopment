using DM.Core.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    /// <summary>
    /// 任务要素
    /// </summary>
    public class MissionElement : DMonoBehaviour
    {
        public UIFixedObj uiFixedObj = null;

        public System.Action<bool> Action_Selected = null;
        public System.Action<string> Action_SetTitle = null;
        public System.Action<BObjectModel> Action_Tracking = null;
        public System.Action<BObjectModel> Action_Location = null;

        private string groupName = "";

        public string Name
        {
            get { return m_name.text; }
            set { m_name.text = value; }
        }

        public BObjectModel bobjModel;

        public GameObject m_dmCompotent;//绑定组件

        private Text m_name;
        private Toggle m_toggle;

        private void Awake()
        {
            m_name = transform.Find("Name").GetComponent<Text>();
        }

        private void Start()
        {
            m_toggle = gameObject.GetComponentInChildren<Toggle>();
            m_toggle.onValueChanged.AddListener(OnValueChanged);
            Button[] btns = gameObject.GetComponentsInChildren<Button>();
            foreach (var item in btns)
            {
                item.onClick.AddListener(()=> {
                    ClickCallBack(item.name);
                });
            }
        }

        private void OnValueChanged(bool isOn)
        {
            MiniMap.ComPointData comPointData = UIDirectDeduction.Instance.miniMap.GetComPointData(bobjModel.BObject.Id);

            if (isOn)
            {
                UIDirectDeduction.Instance.SetElementInfo(bobjModel);
                if (comPointData != null)
                {
                    comPointData.ShowSelected(true);
                }
            }
            else
            {
                if (comPointData != null)
                {
                    comPointData.ShowSelected(false);
                }
                
            }
            Action_Selected?.Invoke(isOn);
            Action_SetTitle?.Invoke(groupName);
            if (!m_toggle.group.AnyTogglesOn())
            {
                UIDirectDeduction.Instance.SetElementInfo(null);
                ((ZhiHuiDuanMain)main).thirdCameraControl.Target = null;
                ((ZhiHuiDuanMain)main).thirdCameraControl.EnableControls = true;
            }
        }

        private void ClickCallBack(string btnName)
        {
            if (btnName.Equals("Btn_Tracking"))
            {
                Action_Tracking?.Invoke(bobjModel);
                ((ZhiHuiDuanMain)main).thirdCameraControl.Target = bobjModel.gameObject;
                ((ZhiHuiDuanMain)main).thirdCameraControl.EnableControls = true;
            }
            else if (btnName.Equals("Btn_Location"))
            {
                Action_Location?.Invoke(bobjModel);
                //组件的Location
                Transform location = bobjModel.gameObject.GetComponentInChildren<ScriptManager>().main.Location;
                Vector3 locationPos = location.position;
                locationPos.y = 200f;
                ((ZhiHuiDuanMain)main).m_Camera.transform.position = locationPos;
                ((ZhiHuiDuanMain)main).m_Camera.transform.LookAt(main.Location);
            }
        }
        
        public void SetToggle(bool isOn)
        {
            if (m_toggle != null) m_toggle.isOn = isOn;
        }

        public void SetInfo(BObjectModel bobjModel, string groupName, bool isBoxSelected = false, UIFixedObj uiFixedObj = null)
        {
            m_name = transform.Find("Name").GetComponent<Text>();
            if (bobjModel != null)
            {
                this.bobjModel = bobjModel;
                Name = bobjModel.BObject.Info.Name;
                m_dmCompotent = bobjModel.gameObject;
                this.groupName = groupName;
                if (!isBoxSelected)
                {
                    uiFixedObj = UIDirectDeduction.Instance.SetBobjectIcons(bobjModel);
                    uiFixedObj.Action_Selected = SetToggle;
                } 
                Action_Selected = uiFixedObj.SetToggle;
                this.uiFixedObj = uiFixedObj;
            }
        }
    }
}
