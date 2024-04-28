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
    public class ControlInstructionPanel : InfoBase
    {
        private Transform content;
        private GameObject prefabInstructionItem;

        public List<InstructionItem> instructionItems = new List<InstructionItem>();

        protected override void OnInitUI()
        {
            base.OnInitUI();
            content = transform.GetComponentInChildren<ScrollRect>().content;
            prefabInstructionItem = transform.Find("Prefabs/InstructionItem").gameObject;
        }

        public void SetData(BObjectModel bObjectModel)
        {
            OnDestroy(true);
            UIDirectDeduction.Instance.instructionParamPanel.Clear();
            if (bObjectModel == null) return;
            List<Instruction> instructions = bObjectModel.GetInstructions();
            foreach (var item in instructions)
            {
                instructionItems.Add(CreateInstructionItem(item));
            }
        }

        private InstructionItem CreateInstructionItem(Instruction instruction)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(prefabInstructionItem, content);
            InstructionItem instructionItem = new InstructionItem(obj.transform);
            instructionItem.SetData(instruction, UIDirectDeduction.Instance.instructionParamPanel);
            return instructionItem;
        }

        public void OnDestroy(bool isImmediate = false)
        {
            int count = instructionItems.Count;
            for (int i = 0; i < count; i++)
            {
                if (isImmediate)
                {
                    GameObject.DestroyImmediate(instructionItems[0].gameObject);
                }
                else
                {
                    GameObject.Destroy(instructionItems[0].gameObject);
                }
                instructionItems.RemoveAt(0);
            }
            instructionItems.Clear();
        }

        public class InstructionItem
        {
            public Instruction instruction;

            public GameObject gameObject;

            private Toggle toggleSwitch;
            private GameObject open;
            private GameObject close;
            private Text textName;

            private InstructionParamPanel instructionParamPanel;

            public InstructionItem(Transform trans)
            {
                gameObject = trans.gameObject;
                Button btnName = gameObject.GetComponentInChildren<Button>(true);
                btnName.onClick.AddListener(ClickCallBack);
                textName = btnName.transform.Find("Text").GetComponent<Text>();
                toggleSwitch = gameObject.GetComponentInChildren<Toggle>(true);
                toggleSwitch.onValueChanged.AddListener((isOn)=> { ToggleOnValueChanged(isOn); });
                open = toggleSwitch.transform.Find("Open").gameObject;
                close = toggleSwitch.transform.Find("Close").gameObject;
                close.SetActive(false);
            }

            private void ClickCallBack()
            {
                Debug.Log("执行指令");
            }

            private void ToggleOnValueChanged(bool isOn, bool isClose = false)
            {
                open.SetActive(!isOn);
                close.SetActive(isOn);

                if (isClose) return;
                //打开界面
                instructionParamPanel.SetData(isOn ? instruction : null, ToggleOnValueChanged);
            }

            public void SetData(Instruction instruction, InstructionParamPanel instructionParamPanel)
            {
                this.instruction = instruction;
                textName.text = instruction.Name;
                this.instructionParamPanel = instructionParamPanel;
            }
        }
    }
}
