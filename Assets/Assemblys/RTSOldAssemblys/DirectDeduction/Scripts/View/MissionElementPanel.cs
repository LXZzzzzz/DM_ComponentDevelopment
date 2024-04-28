using DM.Core.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace 指挥端
{
    [System.Serializable]
    public class MissionElementPanel : InfoBase
    {
        private Transform panelItems;
        private Transform elementTitleContent;

        //预制体
        private GameObject prefabElementTitleItem;
        private GameObject prefabMissionElementItem;
        private GameObject prefabPanelItem;

        public List<BObjectModel> bObjectModels = new List<BObjectModel>();

        private Dictionary<string, Transform> dicPanelItemContent = new Dictionary<string, Transform>();

        private List<MissionElement> allMissionElement = new List<MissionElement>();

        private int titleContentCount;
        private int index = 0;
        private bool isInit = false;

        protected override void OnInitUI()
        {
            base.OnInitUI();
            panelItems = transform.Find("InfoPanel/PanelItems");
            Transform elementTitles = transform.Find("InfoPanel/ElementTitles");
            elementTitleContent = elementTitles.Find("Viewport/Content");
            Button[] btns = elementTitles.GetComponentsInChildren<Button>();
            foreach (var item in btns)
            {
                item.onClick.AddListener(() => {
                    ClickCallBack(item.name);
                });
            }

            dicPanelItemContent.Add("框选", panelItems.Find("BoxSelecteds/Scroll View/Viewport/Content"));

            //预制体
            prefabPanelItem = panelItems.Find("PanelItem").gameObject;
            Transform prefabs = transform.Find("Prefabs");
            prefabElementTitleItem = prefabs.Find("ElementTitleItem").gameObject;
            prefabMissionElementItem = prefabs.Find("MissionElementItem").gameObject;
        }

        private void ClickCallBack(string btnName)
        {
            Debug.Log("ClickCallBack--->" + btnName);
            if (btnName.Equals("Btn_Left"))
            {
                BtnLeftOrRight(true);
            }
            else if (btnName.Equals("Btn_Right"))
            {
                BtnLeftOrRight(false);
            }
        }

        private void BtnLeftOrRight(bool isLeft)
        {
            int num = isLeft ? -1 : 1;
            index += num;
            index = Mathf.Clamp(index, 0, titleContentCount - 1);
            elementTitleContent.GetChild(index).GetComponent<Toggle>().isOn = true;
        }

        public void SetData(Dictionary<string, List<BObjectModel>> dicData)
        {
            isInit = true;
            //格式:分组名:TagId_SubTagId,TagId_SubTagId; 例如:救援力量:8_1,8_2;遇险事件:9_5;
            foreach (var item in dicData)
            {
                CreateElementTitleItem(item.Key);
                CreateMissionElementItem(item.Key, item.Value);
            }
            titleContentCount = elementTitleContent.childCount;
            index = titleContentCount - 1;
        }

        public void SelectTitle(string title)
        {
            for (int i = 0; i < elementTitleContent.childCount; i++)
            {
                Transform item = elementTitleContent.GetChild(i);
                if (item.name == title)
                {
                    item.GetComponent<Toggle>().isOn = true;
                    return;
                }
            }
        }

        public void SelectedComs(Vector2 minPos, Vector2 maxPos)
        {
            List<MissionElement> missionElements = new List<MissionElement>();
            foreach (var item in allMissionElement)
            {
                if (item.uiFixedObj.gameObject.activeInHierarchy) 
                {
                    if (item.uiFixedObj.transform.localPosition.x > minPos.x && 
                        item.uiFixedObj.transform.localPosition.x < maxPos.x &&
                        item.uiFixedObj.transform.localPosition.y > minPos.y &&
                        item.uiFixedObj.transform.localPosition.y < maxPos.y)
                    {
                        missionElements.Add(item);
                    }
                }
            }
            Debug.Log("-----------missionElements---=" + missionElements.Count);
            CreateBoxSelectedItem(missionElements);
        }

        public void OnDestroy()
        {
            dicPanelItemContent.Clear();
            allMissionElement.Clear();
        }

        private void CreateElementTitleItem(string titleName)
        {
            GameObject obj = CreateItem(ItemType.ElementTitleItem);
            obj.name = titleName;
            Text title = obj.GetComponentInChildren<Text>();
            title.text = titleName;
            RectTransform objRect = obj.GetComponent<RectTransform>();
            objRect.sizeDelta = new Vector2(title.preferredWidth + 7, objRect.sizeDelta.y);
            LayoutRebuilder.ForceRebuildLayoutImmediate(objRect);

            GameObject panelObj = CreatePanelItem(titleName);
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isOn) => {
                panelObj.SetActive(isOn);
                if (isOn) index = obj.transform.GetSiblingIndex();
            });
            toggle.isOn = false;
            if (isInit)
            {
                isInit = false;
                toggle.isOn = true;
            }
        }

        private GameObject CreatePanelItem(string titleName)
        {
            GameObject obj = CreateItem(ItemType.PanelItem);
            obj.name = titleName;
            obj.SetActive(false);
            dicPanelItemContent.Add(titleName, obj.transform.Find("Scroll View/Viewport/Content"));
            return obj;
        }

        private void CreateMissionElementItem(string titleName, List<BObjectModel> bObjectModels)
        {
            Transform parent = dicPanelItemContent[titleName];
            foreach (var item in bObjectModels)
            {
                GameObject obj = CreateItem(ItemType.MissionElementItem, parent);
                MissionElement missionElement = obj.AddComponent<MissionElement>();
                obj.GetComponent<Toggle>().group = parent.GetComponent<ToggleGroup>();
                missionElement.SetInfo(item, titleName);
                missionElement.Action_SetTitle = SelectTitle;
                allMissionElement.Add(missionElement);
            }
        }

        private void CreateBoxSelectedItem(List<MissionElement> missionElements)
        {
            Transform parent = dicPanelItemContent["框选"];
            int count = parent.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject.DestroyImmediate(parent.GetChild(0).gameObject);
            }
            foreach (var item in missionElements)
            {
                GameObject obj = CreateItem(ItemType.MissionElementItem, parent);
                MissionElement missionElement = obj.AddComponent<MissionElement>();
                obj.GetComponent<Toggle>().group = parent.GetComponent<ToggleGroup>();
                missionElement.SetInfo(item.bobjModel, "框选", true, item.uiFixedObj);
            }
        }

        private GameObject CreateItem(ItemType itemType, Transform parent = null)
        {
            GameObject prefab = null;
            switch (itemType)
            {
                case ItemType.ElementTitleItem:
                    parent = elementTitleContent;
                    prefab = prefabElementTitleItem;
                    break;
                case ItemType.MissionElementItem:
                    prefab = prefabMissionElementItem;
                    break;
                case ItemType.PanelItem:
                    parent = panelItems;
                    prefab = prefabPanelItem;
                    break;
            }

            GameObject obj = GameObject.Instantiate<GameObject>(prefab, parent, false);
            return obj;
        }

        private enum ItemType
        {
            ElementTitleItem = 0,
            MissionElementItem,
            PanelItem,
        }
    }
}
