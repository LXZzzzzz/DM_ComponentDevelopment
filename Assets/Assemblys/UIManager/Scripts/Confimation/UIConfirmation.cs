using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using EventType = Enums.EventType;

namespace UiManager
{
    public enum showType
    {
        tipView,
        newScheme,
        secondConfirm
    }

    public struct ConfirmatonInfo
    {
        public showType type;
        public string showStrInfo;
        public UnityAction<object> sureCallBack;
    }

    public class UIConfirmation : BasePanel
    {
        private ShowTipsViewBase currentShowView;

        [HideInInspector] public Text titleText;
        [HideInInspector] public GameObject newSchemePart;
        [HideInInspector] public GameObject tipsPart;
        [HideInInspector] public GameObject btns;

        public override void Init()
        {
            base.Init();
            //对组件中的所有元素进行获取
            titleText = transform.Find("View/title").GetComponent<Text>();
            newSchemePart = transform.Find("View/infos/newSchemePart").gameObject;
            tipsPart = transform.Find("View/infos/TipsPart").gameObject;
            btns = transform.Find("View/btns").gameObject;
        }


        public override void ShowMe(object userData)
        {
            base.ShowMe(userData);
            //根据传过来的数据类型，决定以哪种显示模型进行显示
            ConfirmatonInfo cinfo = (ConfirmatonInfo)userData;

            SelectShowViewLogic(cinfo.type);
            currentShowView?.OnShow(cinfo.showStrInfo);

            GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIConfirmation));
            GetControl<Button>("sure").onClick.AddListener(() =>
            {
                if (currentShowView.OnSure(out object cbObj))
                {
                    cinfo.sureCallBack?.Invoke(cbObj);
                    Close(UIName.UIConfirmation);
                }
            });

            GetControl<Button>("cancel").gameObject.SetActive(cinfo.type != showType.tipView);

            if (MyDataInfo.isPlayBack) StartCoroutine(closeMe());
        }

        IEnumerator closeMe()
        {
            yield return new WaitForSeconds(3);
            Close(UIName.UIConfirmation);
        }

        private void SelectShowViewLogic(showType st)
        {
            switch (st)
            {
                case showType.newScheme:
                    currentShowView = new ShowNewScheme(this);
                    break;
                case showType.tipView:
                    currentShowView = new ShowTip(this);
                    break;
                case showType.secondConfirm:
                    currentShowView = new ShowSecondConfirm(this);
                    break;
            }
        }

        public override void HideMe()
        {
            base.HideMe();
            GetControl<Button>("cancel").onClick.RemoveAllListeners();
            GetControl<Button>("sure").onClick.RemoveAllListeners();
        }
    }

    public abstract class ShowTipsViewBase
    {
        protected UIConfirmation mainLogic;

        public ShowTipsViewBase(UIConfirmation uicf)
        {
            mainLogic = uicf;
        }

        public abstract void OnShow(string infoStr);
        public abstract bool OnSure(out object cbObj);
    }

    public class ShowTip : ShowTipsViewBase
    {
        public ShowTip(UIConfirmation uicf) : base(uicf)
        {
        }

        public override void OnShow(string infoStr)
        {
            mainLogic.titleText.text = "提示！";
            mainLogic.tipsPart.SetActive(true);
            mainLogic.tipsPart.GetComponent<Text>().text = infoStr;
        }

        public override bool OnSure(out object cbObj)
        {
            cbObj = null;
            return true;
        }
    }

    public class ShowNewScheme : ShowTipsViewBase
    {
        public ShowNewScheme(UIConfirmation uicf) : base(uicf)
        {
        }

        public override void OnShow(string infoStr)
        {
            mainLogic.titleText.text = "新建方案";
            mainLogic.newSchemePart.SetActive(true);
        }

        public override bool OnSure(out object cbObj)
        {
            string name = mainLogic.newSchemePart.GetComponentInChildren<InputField>().text;
            if (string.IsNullOrEmpty(name))
            {
                cbObj = null;
                return false;
            }

            cbObj = name;

            return true;
        }
    }

    public class ShowSecondConfirm : ShowTipsViewBase
    {
        public ShowSecondConfirm(UIConfirmation uicf) : base(uicf)
        {
        }

        public override void OnShow(string infoStr)
        {
            mainLogic.titleText.text = "二次确认";
            mainLogic.tipsPart.SetActive(true);
            mainLogic.tipsPart.GetComponent<Text>().text = infoStr;
        }

        public override bool OnSure(out object cbObj)
        {
            cbObj = null;
            return true;
        }
    }
}