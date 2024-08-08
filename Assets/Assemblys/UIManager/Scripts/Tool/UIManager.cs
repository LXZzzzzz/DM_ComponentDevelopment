using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;

namespace UiManager
{
    public class UIManager : MonoSingleTon<UIManager>
    {
        public Canvas CurrentCanvans;
        private Canvas popUp;
        private Canvas upper;
        private Canvas middle;
        private Canvas below;

        //public Camera UICamera;
        public Dictionary<string, List<BasePanel>> panelDic;

        private Dictionary<UIName, BasePanel.UIType> uiPanelWhereLayer;

        public Dictionary<string, Sprite> PicBObjects;

        public string MisName;
        public string terrainName;

        private void Start()
        {
            //DontDestroyOnLoad(this);
            panelDic = new Dictionary<string, List<BasePanel>>();
            uiPanelWhereLayer = new Dictionary<UIName, BasePanel.UIType>();
            CurrentCanvans = GetComponentInChildren<Canvas>();
            //UICamera = GetComponentInChildren<Camera>();
            popUp = transform.Find("Canvas_Popup").GetComponent<Canvas>();
            upper = transform.Find("Canvas_Upper").GetComponent<Canvas>();
            middle = transform.Find("Canvas_Middle").GetComponent<Canvas>();
            below = transform.Find("Canvas_Below").GetComponent<Canvas>();
            SetPanelLayer();
        }

        //设置特殊UI对应层级
        private void SetPanelLayer()
        {
            uiPanelWhereLayer.Add(UIName.UIConfirmation, BasePanel.UIType.popUp);
            uiPanelWhereLayer.Add(UIName.UITopMenuView, BasePanel.UIType.upper);
            uiPanelWhereLayer.Add(UIName.UIRightClickMenuView, BasePanel.UIType.upper);
            // uiPanelWhereLayer.Add(UIName.UIAttributeView, BasePanel.UIType.upper);
            uiPanelWhereLayer.Add(UIName.UIHangShowInfo, BasePanel.UIType.popUp);
            uiPanelWhereLayer.Add(UIName.UIAirportAircraftShowView, BasePanel.UIType.upper);
            uiPanelWhereLayer.Add(UIName.UIThreeDIcon, BasePanel.UIType.below);
        }

        /// <summary>
        /// 创建UI
        /// </summary>
        public void ShowPanel<T>(UIName panelName, object infoData, int count = 1) where T : BasePanel
        {
            if (panelDic.ContainsKey(panelName.ToString()))
            {
                if (panelDic[panelName.ToString()].Count < count)
                {
                    T panel = FindUI(panelName).GetComponent<T>();
                    panel.ShowMe(infoData);
                    panelDic[panelName.ToString()].Add(panel);
                }
                else
                {
                    panelDic[panelName.ToString()][0].ShowMe(infoData);
                    return;
                }
            }
            else
            {
                panelDic.Add(panelName.ToString(), new List<BasePanel>());
                T panel = FindUI(panelName).GetComponent<T>();
                panel.ShowMe(infoData);
                panelDic[panelName.ToString()].Add(panel);
            }
        }

        /// <summary>
        /// 删除UI
        /// </summary>
        public void HidePanel(string panelName)
        {
            if (panelDic.ContainsKey(panelName))
            {
                int index = 0;
                panelDic[panelName][index].HideMe();
                Destroy(panelDic[panelName][index].gameObject);
                panelDic[panelName].RemoveAt(index);
                if (panelDic[panelName].Count == 0)
                    panelDic.Remove(panelName);
            }
        }

        public T GetUIPanel<T>(UIName panelName) where T : BasePanel
        {
            if (panelDic.ContainsKey(panelName.ToString()))
            {
                return panelDic[panelName.ToString()][0].GetComponent<T>();
            }

            return null;
        }

        //这里需要改成在自己下方指定位置寻找指定UI
        private GameObject FindUI(UIName uiName)
        {
            //在UI预制体库里找到制定UI，实例化一个放到制定Canvas下
            BasePanel itemUI = null;
            Transform canvansTran = null;
            if (!uiPanelWhereLayer.ContainsKey(uiName))
                canvansTran = middle.transform;
            else
            {
                switch (uiPanelWhereLayer[uiName])
                {
                    case BasePanel.UIType.upper:
                        canvansTran = upper.transform;
                        break;
                    case BasePanel.UIType.popUp:
                        canvansTran = popUp.transform;
                        break;
                    case BasePanel.UIType.below:
                        canvansTran = below.transform;
                        break;
                }
            }

            switch (uiName)
            {
                case UIName.UIIconShow:
                    itemUI = Instantiate((main as UIManagerMain).UIIconShow, canvansTran);
                    break;
                case UIName.UICursorShow:
                    itemUI = Instantiate((main as UIManagerMain).UICursorShow, canvansTran);
                    break;
                case UIName.UIBarChart:
                    itemUI = Instantiate((main as UIManagerMain).UIBarChartController, canvansTran);
                    break;
                case UIName.UIConfirmation:
                    itemUI = Instantiate((main as UIManagerMain).UIConfirmation, canvansTran);
                    break;
                case UIName.UIMap:
                    itemUI = Instantiate((main as UIManagerMain).UIMap, canvansTran);
                    break;
                case UIName.UICommanderView:
                    itemUI = Instantiate((main as UIManagerMain).uiCommanderView, canvansTran);
                    break;
                case UIName.UITopMenuView:
                    itemUI = Instantiate((main as UIManagerMain).UITopMenuView, canvansTran);
                    break;
                case UIName.UIRightClickMenuView:
                    itemUI = Instantiate((main as UIManagerMain).UIRightClickMenuView, canvansTran);
                    break;
                case UIName.UIAttributeView:
                    itemUI = Instantiate((main as UIManagerMain).UIAttributeView, canvansTran);
                    break;
                case UIName.UIHangShowInfo:
                    itemUI = Instantiate((main as UIManagerMain).UIHangShowInfo, canvansTran);
                    break;
                case UIName.UIAirportAircraftShowView:
                    itemUI = Instantiate((main as UIManagerMain).UIAirportAircraftShowView, canvansTran);
                    break;
                case UIName.UIThreeDIcon:
                    itemUI = Instantiate((main as UIManagerMain).UIThreeDIconView, canvansTran);
                    break;
            }

            itemUI.gameObject.SetActive(true);
            itemUI.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            itemUI.transform.localScale = Vector3.one;
            RectTransform goRT = itemUI.GetComponent<RectTransform>();
            goRT.offsetMax = Vector2.zero;
            goRT.offsetMin = Vector2.zero;
            goRT.SetAsLastSibling();
            return itemUI.gameObject;
        }


        public void ClearUI()
        {
            if (panelDic == null) return;
            foreach (var item in panelDic)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i].IsShow)
                        item.Value[i].HideMe();
                }
            }

            foreach (var item in panelDic)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    Destroy(item.Value[i].gameObject);
                }
            }

            panelDic.Clear();
        }
    }

    public enum UIName
    {
        UIIconShow,
        UICursorShow,
        UIBarChart,
        UIConfirmation,
        UIMap,
        UICommanderView,
        UITopMenuView,
        UIRightClickMenuView,
        UIAttributeView,
        UIHangShowInfo,
        UIAirportAircraftShowView,
        UIThreeDIcon
    }
}