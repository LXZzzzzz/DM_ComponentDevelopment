using DM.Entity;
using DM.IFS;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [System.Serializable]
    public class MissionListPanel : DMonoBehaviour
    {
        private Text simpleInfoTitle;
        private Text listPanelTitle;
        private Transform simpleInfo;
        private Transform listPanelContent;
        private GameObject prefabMissionItem;
        private GameObject prefabSubMissionItem;

        private List<MissionOperItem> simpleInfoMissions = new List<MissionOperItem>();
        private List<MissionGroupItem> listPanelMissions = new List<MissionGroupItem>();

        private List<RoleMission> data = new List<RoleMission>();
        private Dictionary<string, MissionGroupItem> dicMis = new Dictionary<string, MissionGroupItem>();

        private float timer;
        private float interval = 0.64f;

        private void Awake()
        {
            prefabMissionItem = transform.Find("Prefabs/MissionItem").gameObject;
            prefabSubMissionItem = transform.Find("Prefabs/SubMissionItem").gameObject;
            simpleInfo = transform.Find("SimpleInfo");
            simpleInfoTitle = simpleInfo.Find("TitleBg/Text").GetComponent<Text>();
            for (int i = 0; i < simpleInfo.Find("List").childCount; i++)
            {
                GameObject obj = simpleInfo.Find("List").GetChild(i).gameObject;
                MissionOperItem misOperItem = obj.AddComponent<MissionOperItem>();
                obj.SetActive(false);
                simpleInfoMissions.Add(misOperItem);
            }
            simpleInfo.gameObject.SetActive(false);
            Transform listPanel = transform.Find("ListPanel");
            listPanelTitle = listPanel.Find("TitleBg/Text").GetComponent<Text>();
            listPanelContent = listPanel.Find("List/Viewport/Content");
        }

        public void SetData(string misXMLPath)
        {
            //从机没有MissionXMLPath信息的问题:临时方法
            //misXMLPath = string.IsNullOrEmpty(misXMLPath) ? GetXMLPath(misName) : misXMLPath;
            //任务XMLPath从动态属性中读取，可以灵活设置每个角色的任务
            string path = Application.dataPath + "/" + misXMLPath;
            if (File.Exists(path))
            {
                data = SerializeHelper.XMLDeSerializeFromFile<List<RoleMission>>(path);
                for (int i = 0; i < data.Count; i++)
                {
                    MissionGroupItem misGroupItem = CreateMissionItem();
                    misGroupItem.SetMission(data[i]);
                    dicMis.Add(data[i].Id, misGroupItem);
                    
                    if (i == 0)
                    {
                        listPanelTitle.text = data[0].Name;
                        simpleInfo.gameObject.SetActive(true);
                        simpleInfoTitle.text = data[0].Name;
                        for (int j = 0; j < data[0].Operations.Count; j++)
                        {
                            MisOper oper = data[0].Operations[j];
                            simpleInfoMissions[j].Init(oper.BObjectName, oper.OperId, oper.Status, oper.OperCount, oper.ShowText);
                            simpleInfoMissions[j].gameObject.SetActive(true);
                        }
                    }
                }
            }
        }

        private MissionGroupItem CreateMissionItem()
        {
            GameObject obj = GameObject.Instantiate<GameObject>(prefabMissionItem, listPanelContent);
            MissionGroupItem misGroupItem = obj.AddComponent<MissionGroupItem>();
            misGroupItem.Set(prefabSubMissionItem);
            return misGroupItem;
        }

        public void CheckMisStatus()
        {
            for (int i = 0; i < data.Count; i++)
            {
                bool active = true;
                for (int j = 0; j < data[i].PreMissions.Count; j++)
                {
                    PreMis pre = data[i].PreMissions[j];
                    if (pre.Finished != dicMis[pre.MissionId].Finished)
                    {
                        active = false;
                        break;
                    }
                }
                if (!dicMis[data[i].Id].Finished)
                    dicMis[data[i].Id].Active = active;
            }
        }
        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                timer = 0;
                CheckMisStatus();
            }
        }

        private void OnDestroy()
        {
            simpleInfoMissions.Clear();
            int count = listPanelMissions.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject.Destroy(listPanelMissions[i].gameObject);
            }
            listPanelMissions.Clear();
            data.Clear();
            dicMis.Clear();
        }
    }
}
