using DM.Core.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    public class UIFixedObj : DMonoBehaviour
    {
        public System.Action<bool> Action_Selected = null;

        public BObjectModel Model { get; set; } = null;

        public GameObject DMObj { get; set; } = null;

        public RectTransform m_rect;

        [HideInInspector]
        public float m_objY = 0.1f;
        private GameObject m_uiFixedObj;
        private Toggle m_toggle;
        private Image m_comIcon;
        //private Text m_name;

        private RectTransform canvasRect;

        private Camera m_camera;
        private void Awake()
        {
            if (null == m_uiFixedObj) m_uiFixedObj = transform.Find("UIFixedObj").gameObject;
            if (null == m_comIcon) m_comIcon = m_uiFixedObj.transform.Find("ComIcon").GetComponent<Image>();
        }
        private void Start()
        {
            m_rect = transform as RectTransform;
            Canvas canvas = transform.GetComponentInParent<Canvas>();
            canvasRect = canvas.transform as RectTransform;
            
            m_camera = ((ZhiHuiDuanMain)main).m_Camera;
            m_toggle = GetComponent<Toggle>();
            m_toggle.isOn = false;
            m_toggle.onValueChanged.AddListener(OnValueChanged);
            DMObj = Model?.gameObject;
            SetIcon(DirectDeductionMgr.GetInstance.GetComIcon(Model.BObject.Id));
        }

        private void SetIcon(Sprite sprite)
        {
            if (sprite != null) m_comIcon.sprite = sprite;
        }

        public void SetToggle(bool isOn)
        {
            if (m_toggle != null) m_toggle.isOn = isOn;
        }

        private void OnValueChanged(bool isOn)
        {
            Action_Selected?.Invoke(isOn);
        }

        private void ComIconEnabled(Vector3 dmComPos)
        {
            Vector3 dir1 = dmComPos - m_camera.transform.position;
            Vector3 dir2 = m_camera.transform.forward;
            if (Vector3.Dot(dir1, dir2) > 0)
                m_uiFixedObj.SetActive(true);
            else
                m_uiFixedObj.SetActive(false);
        }

        private void Update()
        {
            if (m_camera == null || DMObj == null) return;
            Vector3 point = DMObj.transform.position;
            point.y += m_objY;
            ComIconEnabled(point);
            Vector2 screenPoint = ((ZhiHuiDuanMain)main).m_Camera.WorldToScreenPoint(point);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out Vector2 pos);
            {
                m_rect.anchoredPosition = pos;
            }
        }
    }
}
