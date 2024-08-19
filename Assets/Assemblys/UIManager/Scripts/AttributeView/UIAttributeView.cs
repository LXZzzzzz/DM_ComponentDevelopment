using System;
using System.Collections;
using System.Collections.Generic;
using DM.Core.Map;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.ProgrammePart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UIAttributeView : BasePanel
{
    private InputField waterAmount;
    private Text watersName;
    private IWatersOperation operateObj;
    private Vector3 watersPos;
    private bool isReceiveMapInfo;
    private GameObject watersSkillView, equipInfoView;
    private AttributeView_ZiyuanPart ziYuanInfoView;
    private RectTransform msgParent;
    private msgCell msgObj;
    private List<msgCell> allMsgCells;


    private ZiYuanCell zycPrefab;
    private TaskCell taskPrefab;
    private RectTransform taskParent;
    private List<ZiYuanCell> allZyZqCells;
    private List<TaskCell> allTaskCells; //存储所有任务cell，方便后面数据修改

    public override void Init()
    {
        base.Init();
        waterAmount = GetControl<InputField>("input_waterAmount");
        watersName = GetControl<Text>("text_waters");
        watersSkillView = transform.Find("Skill_WaterIntaking/skillWatersInfoView").gameObject;
        equipInfoView = transform.Find("Skill_WaterIntaking/equipInfoView").gameObject;
        ziYuanInfoView = GetComponentInChildren<AttributeView_ZiyuanPart>(true);
        GetControl<Button>("btn_chooseWaters").onClick.AddListener(() => isReceiveMapInfo = true);
        GetControl<Button>("btn_sure").onClick.AddListener(OnSendSkillParameter);
        GetControl<Button>("btn_close").onClick.AddListener(() => Close(UIName.UIAttributeView));
        msgParent = GetControl<ScrollRect>("msgInfoView").content;
        msgObj = GetComponentInChildren<msgCell>(true);
        taskParent = GetControl<ScrollRect>("TaskListView").content;
        taskPrefab = GetComponentInChildren<TaskCell>(true);
        zycPrefab = GetComponentInChildren<ZiYuanCell>(true);
        EventManager.Instance.AddEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.AddEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
        EventManager.Instance.AddEventListener<string>(EventType.ChangeCurrentCom.ToString(), OnChangeCom);
        allMsgCells = new List<msgCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        StartCoroutine(ShowTaskView());
        return;
        //如果当前正在接收外部，则关闭其他属性展示逻辑
        if (isReceiveMapInfo) return;
        if (userData == null)
        {
            watersSkillView.SetActive(false);
            equipInfoView.SetActive(false);
            ziYuanInfoView.gameObject.SetActive(false);
        }

        if (userData is EquipBase)
        {
            EquipBase currentEquip = userData as EquipBase;
            switch (currentEquip.CurrentChooseSkillType)
            {
                case SkillType.None:
                    watersSkillView.SetActive(false);
                    equipInfoView.SetActive(true);
                    ziYuanInfoView.gameObject.SetActive(false);
                    equipInfoView.transform.Find("title").GetComponent<Text>().text = $"{currentEquip.name}-性能参数";
                    var showInfos = currentEquip.AttributeInfos;
                    var allInputf = equipInfoView.GetComponentsInChildren<InputField>(true);
                    for (int i = 0; i < allInputf.Length; i++)
                    {
                        allInputf[i].text = showInfos[i];
                    }

                    break;
                case SkillType.WaterIntaking:
                    watersSkillView.SetActive(true);
                    equipInfoView.SetActive(false);
                    ziYuanInfoView.gameObject.SetActive(false);
                    operateObj = (IWatersOperation)userData;
                    isReceiveMapInfo = false;
                    EventManager.Instance.AddEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
                    break;
            }
        }
        else if (userData is ZiYuanBase)
        {
            watersSkillView.SetActive(false);
            equipInfoView.SetActive(false);
            ziYuanInfoView.gameObject.SetActive(true);
            ziYuanInfoView.Init(userData as ZiYuanBase);
        }
    }

    IEnumerator ShowTaskView()
    {
        if (allZyZqCells != null) yield break;
        allZyZqCells = new List<ZiYuanCell>();
        allTaskCells = new List<TaskCell>();

        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 1 || y.Id == 5) != null)
            {
                var itemObj = allBObjects[i];
                ZiYuanBase zyObj = itemObj.GetComponent<ZiYuanBase>();
                bool isDisaster = zyObj.ZiYuanType == ZiYuanType.Hospital || zyObj.ZiYuanType == ZiYuanType.RescueStation ||
                                  zyObj.ZiYuanType == ZiYuanType.DisasterArea || zyObj.ZiYuanType == ZiYuanType.SourceOfAFire;
                if (!isDisaster) continue;
                var itemCell = Instantiate(zycPrefab, taskParent);
                itemCell.Init(itemObj.BObject.Info.Name, itemObj.BObject.Id, zyObj, OnChangeZiYuanBelongTo, null);
                itemCell.gameObject.SetActive(true);
                allZyZqCells.Add(itemCell);
            }
        }

        yield return new WaitForSeconds(1);
        int taskIndex = 0;
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            //任务列表展示
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 3) != null)
            {
                taskIndex++;
                //任务与资源逻辑应该是一样的
                var itemObj = allBObjects[i];
                var itemzy = itemObj.gameObject.GetComponent<ZiYuanBase>();
                var itemCell = Instantiate(taskPrefab, taskParent);
                itemCell.Init("任务" + taskIndex, itemzy);
                itemCell.gameObject.SetActive(true);
                allTaskCells.Add(itemCell);

                yield return 1;
                string zqId = String.Empty;
                while (string.IsNullOrEmpty(zqId))
                {
                    zqId = (itemzy as ITaskProgress)?.getAssociationAssemblyId();
                    yield return 1;
                }

                int targetIndex = 0;
                for (int j = 0; j < taskParent.childCount; j++)
                {
                    if (string.Equals(zqId, taskParent.GetChild(j).GetComponent<ZiYuanCell>()?.myEntityId))
                    {
                        targetIndex = j + 1;
                        break;
                    }
                }

                if (targetIndex >= 0 && targetIndex < taskParent.childCount)
                {
                    itemCell.transform.SetSiblingIndex(targetIndex);
                    Debug.LogError(targetIndex+"更换成功了位置");
                }
            }
        }
    }

    private bool OnChangeZiYuanBelongTo(string ziYuanId, string commanderId, bool addOrRemove)
    {
        bool isChangeSuc = ProgrammeDataManager.Instance.ChangeZiYuanData(ziYuanId, commanderId, addOrRemove);

        if (isChangeSuc)
        {
            for (int i = 0; i < allBObjects.Length; i++)
            {
                if (string.Equals(ziYuanId, allBObjects[i].BObject.Id))
                {
                    var zyObj = allBObjects[i].GetComponent<ZiYuanBase>();
                    if (addOrRemove) zyObj.AddBeUsdCom(commanderId);
                    else zyObj.RemoveBeUsedCom(commanderId);
                    break;
                }
            }
        }

        return isChangeSuc;
    }


    // ////////////////////////////////////////////////////////////消息列表逻辑/////////////////////////////////////////////////////////////////

    public void OnChooseCommander(bool isZong, string id)
    {
        if (isZong)
        {
            for (int i = 0; i < allTaskCells.Count; i++)
            {
                allTaskCells[i].gameObject.SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < allTaskCells.Count; i++)
            {
                bool isShow = allTaskCells[i].allcoms.Find(x => string.Equals(x.comId, id));
                allTaskCells[i].gameObject.SetActive(isShow);
            }
        }
    }

    #region 技能选择逻辑，现在无用了

    private void OnChooseWaters(BObjectModel bom)
    {
        if (!isReceiveMapInfo || bom == null) return;

        var coms = bom.GetComponent<ZiYuanBase>().beUsedCommanderIds;
        var isMeControl = coms?.Find(x => string.Equals(x, MyDataInfo.leadId));
        sender.LogError(bom.name + "我的级别：" + MyDataInfo.MyLevel);

        if (bom.GetComponent<ZiYuanBase>().ZiYuanType != ZiYuanType.Waters)
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "当前选择并不是取水点" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
            watersPos = Vector3.zero;
            return;
        }

        if (isMeControl == null)
        {
            //这里的目的是：当资源归属人为空，那就交给一级指挥官控制
            if (MyDataInfo.MyLevel != 1 || coms != null && coms.Count != 0)
            {
                ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "选择的取水点不可为我所用" };
                UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
                watersPos = Vector3.zero;
                return;
            }
        }

        watersName.text = bom.BObject.Info.Name;
        watersPos = bom.gameObject.transform.position;
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
    }

    private void OnSendSkillParameter()
    {
        if (string.IsNullOrEmpty(waterAmount.text) || watersPos == Vector3.zero)
        {
            ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "取水参数错误" };
            UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
            return;
        }

        isReceiveMapInfo = false;
        // float itemWaterAmount = int.Parse(waterAmount.text);
        // if (operateObj.CheckCapacity() > itemWaterAmount)
        //     operateObj.WaterIntaking(watersPos, 1, itemWaterAmount, false);
        // else
        // {
        //     ConfirmatonInfo infoa = new ConfirmatonInfo { type = showType.tipView, showStrInfo = "取水量超出飞机最大核载水量" };
        //     UIManager.Instance.ShowPanel<UIConfirmation>(UIName.UIConfirmation, infoa);
        // }
    }

    #endregion


    private void OnAddAMsg(string info)
    {
        var itemCell = Instantiate(msgObj, msgParent);
        itemCell.Init(ConvertSecondsToHHMMSS(MyDataInfo.gameStartTime), info);
        itemCell.gameObject.SetActive(true);
        allMsgCells.Add(itemCell);
        StartCoroutine(setSV());
    }

    IEnumerator setSV()
    {
        yield return 1;
        yield return 1;
        GetControl<ScrollRect>("msgInfoView").verticalScrollbar.value = 0;
    }

    private void OnCleraMsg()
    {
        for (int i = 0; i < allMsgCells.Count; i++)
        {
            Destroy(allMsgCells[i].gameObject);
        }

        allMsgCells.Clear();
    }

    private void OnChangeCom(string com)
    {
        allMsgCells.ForEach(a => a.ChangeController(com));
    }

    private string ConvertSecondsToHHMMSS(float seconds)
    {
        int hours = (int)(seconds / 3600); // 计算小时数
        int minutes = (int)(seconds % 3600 / 60); // 计算分钟数
        float remainingSeconds = seconds % 60; // 计算剩余秒数

        // 格式化为“时：分：秒”字符串
        return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, (int)remainingSeconds);
    }

    public override void HideMe()
    {
        base.HideMe();
        EventManager.Instance.RemoveEventListener<BObjectModel>(EventType.MapChooseIcon.ToString(), OnChooseWaters);
        EventManager.Instance.RemoveEventListener<string>(EventType.ShowAMsgInfo.ToString(), OnAddAMsg);
        EventManager.Instance.RemoveEventListener(EventType.ClearMsgBox.ToString(), OnCleraMsg);
        EventManager.Instance.RemoveEventListener<string>(EventType.ChangeCurrentCom.ToString(), OnChangeCom);
    }
}