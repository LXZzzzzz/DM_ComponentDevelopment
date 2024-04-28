using DM.IFS;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    public class MissionItemBase : DMonoBehaviour
    {
        public Action FinishedCall;

        public MissionGroupItem misGroup;
        private float interval = 0.42f;
        private float timer;

        protected Text lblOperName;
        protected Toggle togFinished;

        protected int finStatus, finCount;
        public virtual void Init(string objName, int id, int status, int count, string text)
        {
            misGroup = GetComponentInParent<MissionGroupItem>();
            lblOperName = transform.Find("Label").GetComponent<Text>();
            togFinished = transform.GetComponent<Toggle>();
            finStatus = status;
            finCount = count;
        }
        public virtual void IntervalCheck() { }

        protected Transform GetBObjectByName(string name)
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (allBObjects[i].name == name)
                    return allBObjects[i].transform;
            }
            return null;
        }
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer > interval)
            {
                timer = 0;
                if (misGroup.Active)   //只有激活的任务才会检测子项内容
                    IntervalCheck();
            }
        }
    }
}
