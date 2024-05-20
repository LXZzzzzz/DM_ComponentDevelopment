using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;

namespace UiManager
{
    public class UIManager : MonoSingleTon<UIManager>
    {
        public Canvas CurrentCanvans;

        //public Camera UICamera;
        public Dictionary<string, List<BasePanel>> panelDic;

        private void Start()
        {
            //DontDestroyOnLoad(this);
            panelDic = new Dictionary<string, List<BasePanel>>();
            CurrentCanvans = GetComponentInChildren<Canvas>();
            //UICamera = GetComponentInChildren<Camera>();
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
            switch (uiName)
            {
                case UIName.UIIconShow:
                    itemUI = Instantiate((main as UIManagerMain).UIIconShow, CurrentCanvans.transform);
                    break;
                case UIName.UICursorShow:
                    itemUI = Instantiate((main as UIManagerMain).UICursorShow, CurrentCanvans.transform);
                    break;
                case UIName.UIBarChart:
                    itemUI = Instantiate((main as UIManagerMain).UIBarChartController, CurrentCanvans.transform);
                    break;
                case UIName.UIConfirmation:
                    itemUI = Instantiate((main as UIManagerMain).UIConfirmation, CurrentCanvans.transform);
                    break;
                case UIName.UIMap:
                    itemUI = Instantiate((main as UIManagerMain).UIMap, CurrentCanvans.transform);
                    break;
                case UIName.UICommanderFirstLevel:
                    itemUI = Instantiate((main as UIManagerMain).UICommanderFirstLevel, CurrentCanvans.transform);
                    break;
                default:
                    break;
            }

            itemUI?.gameObject.SetActive(true);
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
        UICommanderFirstLevel
    }
}