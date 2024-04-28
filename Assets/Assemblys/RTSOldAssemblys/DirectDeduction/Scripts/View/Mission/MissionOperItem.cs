using UnityEngine;
using DM.IFS;
using System.Collections.Generic;

namespace 指挥端
{
    public class MissionOperItem : MissionItemBase
    {
        protected DMOperationItem oper;
        public override void Init(string objName, int id, int status, int operCount, string text)
        {
            base.Init(objName, id, status, operCount, text);
            oper = GetDMOper(objName, id);
            if (oper == null)
                lblOperName.text = "场景中不存在该名字组件";
            else if (!string.IsNullOrEmpty(text))
                lblOperName.text = text;
            else
                lblOperName.text = oper.OperItems[oper.Status].Text;
        }
        public override void IntervalCheck()
        {
            if (oper != null && (finStatus == -1 || oper.Status == finStatus)
                && (finCount <= 0 || oper.OperCount == finCount))
            {
                //Debug.Log(oper.Status+";"+finStatus+";"+oper.OperCount);
                togFinished.isOn = true;
                if (FinishedCall != null)
                    FinishedCall();
                this.enabled = false;
            }
        }

        private DMOperationItem GetDMOper(string objName, int id)
        {
            Transform obj = GetBObjectByName(objName);
            if (obj == null) return null;
            return new List<DMOperationItem>(obj.GetComponentsInChildren<DMOperationItem>()).Find(k => k.Id == id);
        }
    }
}
