using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [System.Serializable]
    public class RescueProcessPanel : InfoBase
    {
        private Transform infoContent;
        //预制体
        private GameObject prefabNormalLabelItem;
        private GameObject prefabInstructLabelItem;

        protected override void OnInitUI()
        {
            base.OnInitUI();
            infoContent = transform.Find("InfoPanel/Scroll View/Viewport/Content");
            prefabNormalLabelItem = transform.Find("Prefabs/NormalLabelItem").gameObject;
            prefabInstructLabelItem = transform.Find("Prefabs/InstructLabelItem").gameObject;
        }

        public void CreateNormalLabelItem(string timeStr, string text1)
        {
            LabelItem labelItem = CreateLabelItem(LabelType.NormalLabel);
            labelItem.SetNormalLabel(timeStr, text1);
        }

        public void CreateInstructLabelItem(string timeStr, string text1, string insName, string text2)
        {
            LabelItem labelItem = CreateLabelItem(LabelType.InstructLabel);
            labelItem.SetInstructLabel(timeStr, text1, insName, text2);
        }

        private LabelItem CreateLabelItem(LabelType labelType)
        {
            GameObject prefab = null;
            if (labelType == LabelType.NormalLabel)
            {
                prefab = prefabNormalLabelItem;
            }
            else if (labelType == LabelType.InstructLabel)
            {
                prefab = prefabInstructLabelItem;
            }
            GameObject obj = GameObject.Instantiate<GameObject>(prefab, infoContent);
            LabelItem labelItem = new LabelItem(labelType, obj.transform);
            return labelItem;
        }

        private enum LabelType
        {
            NormalLabel,
            InstructLabel,
        }

        private class LabelItem
        {
            public LabelType labelType;

            private Text textTime;
            private Text text1;
            private Text insName;
            private Text text2;

            private RectTransform rectTrans;

            public LabelItem(LabelType labelType, Transform trans)
            {
                rectTrans = (RectTransform)trans;
                this.labelType = labelType;
                textTime = trans.Find("Time").GetComponent<Text>();
                text1 = trans.Find("Text1").GetComponent<Text>();
                if (labelType == LabelType.InstructLabel)
                {
                    insName = trans.Find("Instruct/Text").GetComponent<Text>();
                    text2 = trans.Find("Text2").GetComponent<Text>();
                }
            }

            public void SetNormalLabel(string timeStr, string text1)
            {
                textTime.text = timeStr;
                this.text1.text = text1;
                SetLabelItemHeight();
            }

            public void SetInstructLabel(string timeStr, string text1, string insName, string text2)
            {
                textTime.text = timeStr;
                this.text1.text = text1;
                this.insName.text = insName;
                this.text2.text = text2;
                SetLabelItemHeight();
            }

            private void SetLabelItemHeight()
            {
                if (labelType == LabelType.NormalLabel)
                {
                    if (rectTrans.sizeDelta.y < text1.preferredHeight)
                    {
                        rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, text1.preferredHeight);
                    }
                }
                else if (labelType == LabelType.InstructLabel)
                {
                    float offsetH = rectTrans.sizeDelta.y;
                    if (offsetH < text2.preferredHeight) offsetH = text2.preferredHeight;
                    float height = rectTrans.sizeDelta.y + offsetH;
                    rectTrans.sizeDelta = new Vector2(rectTrans.sizeDelta.x, height);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTrans);
                LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)rectTrans.parent);
                rectTrans.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
            }
        }
    }
}
