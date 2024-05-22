using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiManager
{
    public class UIConfirmation : BasePanel
    {
        private bool isSync;
        public override void Init()
        {
            base.Init();
            _myUIType = UIType.popUp;
        }

        public override void ShowMe(object userData)
        {
            base.ShowMe(userData);
            isSync = userData is string;
            
            if(!isSync)
            {
                ShowWindowInfo data = (ShowWindowInfo)userData;
                string showInfo = data.showInfo;
                GetControl<Text>("title").text = showInfo;

                GetControl<Button>("sure").onClick.AddListener(data.callBack);
                GetControl<Button>("cancel").onClick.AddListener(() => Close(UIName.UIConfirmation));
            }
            else
            {
                GetControl<Text>("title").text = (string)userData;
                GetControl<Button>("sure").onClick.AddListener(() =>
                {
                    sender.RunSend(SendType.SubToMain,UIManager.Instance.main.BObjectId,888,MyDataInfo.leadId);
                });
                GetControl<Button>("cancel").onClick.AddListener(() =>
                {
                    sender.RunSend(SendType.SubToMain,UIManager.Instance.main.BObjectId,999,MyDataInfo.leadId);
                });
            }
        }
    }

    public struct ShowWindowInfo
    {
        public string showInfo;
        public UnityAction callBack;
    }
}