using DM.IFS;
using ToolsLibrary;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
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
        GetControl<Button>("btn_StandAlone").onClick.AddListener(standAlone);
        GetControl<Button>("btn_Online").onClick.AddListener(onLine);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        int mainLevel = (int)userData;
        GetControl<Toggle>("Tog_Fazd").isOn = false;
        GetControl<Toggle>("Tog_Fazd").transform.parent.gameObject.SetActive(mainLevel == 1);
    }

    public override void HideMe()
    {
        base.HideMe();
    }

    private void newBuild()
    {
        UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, null);
        EventManager.Instance.AddEventListener<string>(EventType.ConfirmationCbSure.ToString(), runNewScheme);
        //todo：新建方案后，要对场景中的元素清空
    }

    private void runNewScheme(string schemeName)
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.ConfirmationCbSure.ToString(), runNewScheme);
        ProgrammeDataManager.Instance.CreatProgramme(schemeName);
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
        string packedData = ProgrammeDataManager.Instance.PackedData();

        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendProgramme, packedData);
        }

        // for (int i = 0; i < allBObjects.Length; i++)
        // {
        //     if (allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 8) != null)
        //     {
        //         sender.RunSend(SendType.MainToAll, allBObjects[i].BObject.Id, (int)Enums.MessageID.SendProgramme, packedData);
        //     }
        // }
    }

    private void standAlone()
    {
        sender.RunSend(SendType.MainToAll, MyDataInfo.leadId, (int)Enums.MessageID.SendGameStart, "");
    }

    private void onLine()
    {
        if (MyDataInfo.gameState != GameState.Preparation)
        {
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, "请先发布方案，再通知游戏开始");
            return;
        }

        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            sender.RunSend(SendType.MainToAll, MyDataInfo.playerInfos[i].RoleId, (int)Enums.MessageID.SendGameStart, "");
        }
    }
}