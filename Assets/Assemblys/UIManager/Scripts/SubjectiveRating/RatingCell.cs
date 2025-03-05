using System;
using System.Collections.Generic;
using DataTranfsers;
using Enums;
using Newtonsoft.Json;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class RatingCell : DMonoBehaviour
{
    public Text roleName;
    public Text trainingDescribe;
    public Button showDataBtn;
    public Text scoringCriteria;
    public Text completionStatus;
    public InputField score;

    private TrainsPintType trainingPointType;
    private bool isComplet;
    private bool isMeRequest;
    private string jzId;

    public void OnInit(SubjectiveRatingData data, string roleId)
    {
        jzId = roleId;
        roleName.text = data.trainingRole;
        trainingDescribe.text = data.trainingPoint;
        scoringCriteria.text = data.scoringCriteria;
        if (Enum.TryParse(data.associationType, out TrainsPintType tpt))
            trainingPointType = tpt;
        isComplet = MyDataInfo.CompletedTrainingPoints.Contains(data.associationType);
        completionStatus.text = isComplet ? "完成该训练点" : "未完成该训练点";
        showDataBtn.onClick.AddListener(onGetData);
        score.onValueChanged.AddListener(onCheckValue);
        EventManager.Instance.AddEventListener<string>(EventType.getTrainingPointData.ToString(), OnShowData);
        EventManager.Instance.AddEventListener<List<string>>(EventType.getPeculiarData.ToString(), OnShowPeculiarData);
        isMeRequest = false;
    }

    public void OnDelete()
    {
        EventManager.Instance.RemoveEventListener<string>(EventType.getTrainingPointData.ToString(), OnShowData);
        EventManager.Instance.RemoveEventListener<List<string>>(EventType.getPeculiarData.ToString(), OnShowPeculiarData);
        isMeRequest = false;
    }

    public void GetScore(ref ScoreStatistics scoreData)
    {
        //返回分数，默认10分
        int s = string.IsNullOrEmpty(score.text) ? 10 : int.Parse(score.text);
        var aa = isComplet ? s : 0;
        switch (trainingPointType)
        {
            case TrainsPintType.ZBLDSureDisasterInfo:
                scoreData.qrzqxx = aa;
                scoreData.qrzqxx_zg = s;
                break;
            case TrainsPintType.ZBLDSureEquipInfo:
                scoreData.cdzbxxqr = aa;
                scoreData.cdzbxxqr_zg = s;
                break;
            case TrainsPintType.ZBLDSurePersonInfo:
                scoreData.cdryxxqr = aa;
                scoreData.cdryxxqr_zg = s;
                break;
            case TrainsPintType.ZBLDRouteDeclaration:
                scoreData.zchxsb = aa;
                scoreData.zchxsb_zg = s;
                break;
            case TrainsPintType.ZBLDSendTask:
                scoreData.xdrw = aa;
                scoreData.xdrw_zg = s;
                break;
            case TrainsPintType.XCZHGetTask:
                scoreData.lsrw = aa;
                scoreData.lsrw_zg = s;
                break;
            case TrainsPintType.XCZHInspectEquipInfo:
                scoreData.qrzbztxx = aa;
                scoreData.qrzbztxx_zg = s;
                break;
            case TrainsPintType.XCZHSendTask:
                scoreData.fprwbxdrw = aa;
                scoreData.fprwbxdrw_zg = s;
                break;
            case TrainsPintType.XCZHSureTqInfo:
                scoreData.qrtqczbg = aa;
                scoreData.qrtqczbg_zg = s;
                break;
            case TrainsPintType.JZSureTaskInfo:
                break;
            case TrainsPintType.JZSureOilAndLoad:
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).qrzyl = aa;
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).qrzyl_zg = s;
                break;
            case TrainsPintType.JZCompletePlan:
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).rwqyhxgh = aa;
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).rwqyhxgh_zg = s;
                break;
            case TrainsPintType.JZSendTqInfo:
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).xxczhybg = aa;
                scoreData.thirdZhyScores.Find(a => string.Equals(a.roleId, jzId)).xxczhybg_zg = s;
                break;
        }
    }

    private void onGetData()
    {
        //打开数据展示
        isMeRequest = true;
        switch (trainingPointType)
        {
            case TrainsPintType.XCZHSureTqInfo:
                EventManager.Instance.EventTrigger<int>(EventType.requestPeculiarData.ToString(), 3);
                break;
            case TrainsPintType.JZSendTqInfo:
                EventManager.Instance.EventTrigger<int>(EventType.requestPeculiarData.ToString(), 2);
                break;
            case TrainsPintType.XCZHGetTask:
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "该训练点无相关采分项");
                break;
            case TrainsPintType.XCZHSendTask:
            case TrainsPintType.JZSureTaskInfo:
                //类型3：显示任务分配情况，并在页面显示切换机型功能
                ZyfpInfo zi = new ZyfpInfo() { type = 3, currentInfo = null, sureCb = null };
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, zi);
                break;
            default:
                EventManager.Instance.EventTrigger<string>(EventType.requestTrainingPointData.ToString(), trainingPointType.ToString());
                break;
        }
    }

    private void OnShowData(string tData)
    {
        if (!isMeRequest) return;
        isMeRequest = false;

        switch (trainingPointType)
        {
            case TrainsPintType.ZBLDSureDisasterInfo:
            case TrainsPintType.ZBLDSureEquipInfo:
            case TrainsPintType.ZBLDSurePersonInfo:
            case TrainsPintType.XCZHInspectEquipInfo:
            case TrainsPintType.JZSureOilAndLoad:
                string showJsonStr = AESUtils.Decrypt(tData);
                var data = JsonConvert.DeserializeObject<ShowInfoClass>(showJsonStr);
                UIManager.Instance.ShowPanel<UIChangeZyData>(UIName.UIChangeZyData, new ShowStrInputData_Daojiao((int)data.szdt, data.dataStr));
                break;
            case TrainsPintType.ZBLDRouteDeclaration:
                UIManager.Instance.ShowPanel<UIAirLineInfoShow>(UIName.UIAirLineInfoShow, tData);
                break;
            case TrainsPintType.ZBLDSendTask:
                UIManager.Instance.ShowPanel<UISendTaskInfoShow>(UIName.UISendTaskInfoShow, tData);
                break;
            case TrainsPintType.JZCompletePlan:
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "请查看地图显示的规划路线");
                break;
        }
    }

    private void OnShowPeculiarData(List<string> pData)
    {
        //这里要显示特情的信息
        if (!isMeRequest) return;
        isMeRequest = false;

        switch (trainingPointType)
        {
            case TrainsPintType.XCZHSureTqInfo:
                UIManager.Instance.ShowPanel<UIPeculiarInfoShow>(UIName.UIPeculiarInfoShow, pData);
                break;
            case TrainsPintType.JZSendTqInfo:
                List<string> myTq = new List<string>();
                for (int i = 0; i < pData.Count; i++)
                {
                    var data = pData[i].Split('_');
                    if (data.Length != 2)
                    {
                        myTq.Add(pData[i]);
                        continue;
                    }
                    if (string.Equals(jzId, data[1])) myTq.Add(data[0]);
                }

                UIManager.Instance.ShowPanel<UIPeculiarInfoShow>(UIName.UIPeculiarInfoShow, myTq);
                break;
            default:
                Debug.LogError($"{trainingPointType}该类型数据的展示不应该出现在这里");
                break;
        }
    }

    private void onCheckValue(string v)
    {
        if (int.TryParse(v, out int a))
        {
            score.text = Mathf.Clamp(a, 1, 10).ToString();
        }
    }
}