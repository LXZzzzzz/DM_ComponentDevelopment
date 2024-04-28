using UnityEngine;
using UnityEngine.UI;
using DM.IFS;

namespace 指挥端
{
    public class MissionGroupItem : DMonoBehaviour
    {
        public bool Finished { get; set; }

        public bool Active
        {
            get { return toggle.isOn; }
            set
            {
                toggle.isOn = value;
            }
        }

        public string MisId { get; set; }

        private Toggle toggle;
        private Text misName;
        private Transform subMissionList;
        private GameObject prefabSubMission;

        private int totalCount;
        private int finishedCount = 0;

        private void Awake()
        {
            toggle = gameObject.GetComponent<Toggle>();
            misName = gameObject.GetComponentInChildren<Text>(true);
            subMissionList = transform.Find("SubMissionList");
        }

        public void Set(GameObject prefabSubMission)
        {
            this.prefabSubMission = prefabSubMission;
        }

        public void SetMission(RoleMission mis)
        {
            //临时
            toggle = gameObject.GetComponent<Toggle>();
            misName = gameObject.GetComponentInChildren<Text>(true);
            subMissionList = transform.Find("SubMissionList");

            misName.text = mis.Name;
            MisId = mis.Id;
            Finished = mis.Finished;
            Active = !mis.Finished;

            totalCount = mis.Operations.Count + mis.Knapsacks.Count;
            for (int i = 0; i < mis.Operations.Count; i++)
            {
                MisOper oper = mis.Operations[i];
                GameObject item = AddSubMissionItem();
                MissionOperItem itemScript = item.AddComponent<MissionOperItem>();
                itemScript.Init(oper.BObjectName, oper.OperId, oper.Status, oper.OperCount, oper.ShowText);
                itemScript.misGroup = this;
                itemScript.FinishedCall += FinishCount;
            }
            for (int i = 0; i < mis.Knapsacks.Count; i++)
            {
                MisKnap knap = mis.Knapsacks[i];
                GameObject item = AddSubMissionItem();
                MissionKnapItem itemScript = item.AddComponent<MissionKnapItem>();
                itemScript.Init(knap.BObjectName, knap.KnapId, knap.Status, 0, knap.ShowText);
                itemScript.misGroup = this;
                itemScript.FinishedCall += FinishCount;
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)subMissionList);
            float h = subMissionList.childCount * 15;
            RectTransform rect = (RectTransform)transform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, 17 + h);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }

        private void FinishCount()
        {
            finishedCount += 1;
            if (finishedCount == totalCount) //该任务完成
            {
                Finished = true;
                Active = false;
            }
        }

        private GameObject AddSubMissionItem()
        {
            return GameObject.Instantiate<GameObject>(prefabSubMission, subMissionList);
        }
    }
}
