using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UiManager
{
    public abstract class BasePanel : DMonoBehaviour
    {
        public enum UIType
        {
            popUp,
            upper,
            middle
        }

        //通过里氏转换原则，存储所有UI控件
        private Dictionary<string, List<UIBehaviour>> controlDic;
        protected UIType _myUIType = UIType.middle;
        [HideInInspector] public bool IsShow;

        public UIType myUIType => _myUIType;

        private void Awake()
        {
            controlDic = new Dictionary<string, List<UIBehaviour>>();
            FindChildControl<Button>();
            FindChildControl<Text>();
            FindChildControl<InputField>();
            FindChildControl<Toggle>();
            FindChildControl<Image>();
            FindChildControl<ToggleGroup>();
            FindChildControl<Dropdown>();
            FindChildControl<ScrollRect>();
            IsShow = false;
            Init();
        }

        private void WindowAni()
        {
            // switch (myUIType)
            // {
            //     case UIType.View:
            //         break;
            //     case UIType.Window:
            //         //transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector2.zero;
            //         // transform.GetChild(0).GetComponent<RectTransform>().DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            //         break;
            //     default:
            //         break;
            // }
        }

        /// <summary>
        /// 获取对应名字的控件
        /// </summary>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            if (controlDic.ContainsKey(controlName))
            {
                for (int i = 0; i < controlDic[controlName].Count; i++)
                {
                    if (controlDic[controlName][i] is T)
                    {
                        return controlDic[controlName][i] as T;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 找到panel下所有指定类型的控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void FindChildControl<T>() where T : UIBehaviour
        {
            T[] controls = this.GetComponentsInChildren<T>(true);
            string objName;
            for (int i = 0; i < controls.Length; i++)
            {
                objName = controls[i].gameObject.name;
                if (controlDic.ContainsKey(objName))
                {
                    controlDic[objName].Add(controls[i]);
                }
                else
                {
                    controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
                }
            }
        }

        public virtual void Init()
        {
        }

        public virtual void ShowMe(object userData)
        {
            IsShow = true;
            WindowAni();
        }

        protected void Close(UIName uiName)
        {
            UIManager.Instance.HidePanel(uiName.ToString());
        }

        public virtual void HideMe()
        {
            IsShow = false;
        }
    }
}