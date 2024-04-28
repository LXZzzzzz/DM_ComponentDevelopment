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
    public class InstructionParamPanel : InfoBase
    {
        private RectTransform infoPanel;
        private RectTransform rect;
        private Vector2 rectStartSize = new Vector2();
        private System.Action<bool, bool> actionClose = null;

        protected override void OnInitUI()
        {
            base.OnInitUI();
            rect = (RectTransform)transform;
            rectStartSize = rect.sizeDelta;
            infoPanel = transform.Find("InfoPanel").GetComponent<RectTransform>();
            Button btnClose = transform.GetComponentInChildren<Button>();
            btnClose.onClick.AddListener(()=> {
                actionClose?.Invoke(false, true);
            });
        }

        public void SetData(Instruction instruction, System.Action<bool, bool> action)
        {
            Clear();
            actionClose = action;
            gameObject.SetActive(instruction != null);
            if (instruction == null) return;

            List<DynamicProperty> dynamicProperties = instruction.Properties;
            foreach (var item in dynamicProperties)
            {
                DirectDeductionMgr.GetInstance.AddDynamicProperty(item, infoPanel);
            }
            
            if (infoPanel.childCount > 6)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(infoPanel);
                rect.sizeDelta = new Vector2(infoPanel.sizeDelta.x + 10, rectStartSize.y);
            }
            else
            {
                rect.sizeDelta = rectStartSize;
            }
        }
    
        public void Clear()
        {
            actionClose = null;
            int count = infoPanel.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject.DestroyImmediate(infoPanel.GetChild(0).gameObject);
            }
        }
    }
}
