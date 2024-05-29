using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

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
        //todo：新建方案后，要对场景中的元素清空
    }

    private void save()
    {
        ProgrammeDataManager.Instance.SaveProgramme();
    }

    private void saveAs()
    {
    }

    private void load()
    {
        var data = ProgrammeDataManager.Instance.LoadProgramme();

        if (data != null)
        {
            EventManager.Instance.EventTrigger(Enums.EventType.LoadProgrammeDataSuc.ToString(), data);
        }
        else
        {
            Debug.LogError("所选文件解析失败");
        }
    }

    private void release()
    {
        //随便写一组数据，走一下这个逻辑
        string packedData = ProgrammeDataManager.Instance.PackedData();

        for (int i = 0; i < allBObjects.Length; i++)
        {
            if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) != null)
            {
                sender.RunSend(SendType.MainToAll, allBObjects[i].BObject.Id, (int)Enums.MessageID.SendProgramme, packedData);
            }
        }
    }
}