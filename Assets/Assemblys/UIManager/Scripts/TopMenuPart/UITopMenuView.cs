using System;
using System.Collections;
using System.Collections.Generic;
using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UITopMenuView : BasePanel
{
    public override void Init()
    {
        base.Init();
        _myUIType = UIType.upper;
        GetControl<Button>("btn_NewBuild").onClick.AddListener(newBuild);
        GetControl<Button>("btn_Save").onClick.AddListener(save);
        GetControl<Button>("btn_SaveAs").onClick.AddListener(saveAs);
        GetControl<Button>("btn_Load").onClick.AddListener(load);
        GetControl<Button>("btn_Release").onClick.AddListener(release);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        int mainLevel = (int)userData;
        GetControl<Toggle>("Tog_Fazd").isOn = false;
        GetControl<Toggle>("Tog_Fazd").interactable = mainLevel == 1;
    }

    public override void HideMe()
    {
        base.HideMe();
    }

    private void newBuild()
    {
        ProgrammeDataManager.Instance.CreatProgramme("测试默认方案");
        Debug.LogError("新建了方案");

    }

    private void save()
    {
    }

    private void saveAs()
    {
    }

    private void load()
    {
    }

    private void release()
    {
        //随便写一组数据，走一下这个逻辑
        string packedData = ProgrammeDataManager.Instance.PackedData();

        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (allBObjects[i].BObject.Id==MyDataInfo.leadId)
            {
                sender.LogError(allBObjects[i].gameObject.name);
                break;
            }
        }
        sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendProgramme, packedData);
    }
}