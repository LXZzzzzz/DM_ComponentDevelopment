using System.Collections;
using System.Collections.Generic;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

//创建着火点数据设定
public class FireDataView : ChangeDataBase
{
    private GameObject view;
    private InputField zyName, fs, pd, csrsmj;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/firePart").gameObject;
        zyName = view.transform.Find("name/inputF_name").GetComponent<InputField>();
        fs = view.transform.Find("fs/inputF_fs").GetComponent<InputField>();
        pd = view.transform.Find("pd/inputF_pd").GetComponent<InputField>();
        csrsmj = view.transform.Find("csrsmj/inputF_csrsmj").GetComponent<InputField>();
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("火场数据设定");
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        FireVariableData variableData = new FireVariableData()
        {
            ZyType = ZiYuanType.SourceOfAFire, ZyName = zyName.text, csrsmj = float.Parse(csrsmj.text), fs = float.Parse(fs.text), pd = float.Parse(pd.text)
        };

        EventManager.Instance.EventTrigger<ZyVariableDataBase>(EventType.CreatZaiQuZyRun.ToString(), variableData);
    }
}

//创建灾区点数据设定
public class DisasterDataView : ChangeDataBase
{
    private GameObject view;
    private InputField zyName, num;
    private Dropdown type;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/disasterPart").gameObject;
        zyName = view.transform.Find("name/inputF_name").GetComponent<InputField>();
        num = view.transform.Find("num/inputF_num").GetComponent<InputField>();
        type = view.transform.Find("type/dp_Type").GetComponent<Dropdown>();
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("灾区数据设定");
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        DisasterVariableData variableData = new DisasterVariableData() { ZyType = ZiYuanType.DisasterArea, ZyName = zyName.text, personNum = int.Parse(num.text), type = type.value + 1 };
        EventManager.Instance.EventTrigger<ZyVariableDataBase>(EventType.CreatZaiQuZyRun.ToString(), variableData);
    }
}

//资源分配
public class ZYFPPartView : ChangeDataBase
{
    private GameObject view;
    private RectTransform zyParent;
    private ChangeData_cellZyItem zyTemplate;
    private ZyfpInfo info;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/zyfpPart").gameObject;
        zyParent = view.transform.GetComponentInChildren<ScrollRect>(true).content;
        zyTemplate = view.transform.GetComponentInChildren<ChangeData_cellZyItem>(true);
    }

    public override void OnShow(object data)
    {
        info = (ZyfpInfo)data;
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("直升机资源分配");
        for (int i = 0; i < MyDataInfo.sceneAllZiYuan.Count; i++)
        {
            if (info.type == 1)
                if (MyDataInfo.sceneAllZiYuan[i] is ITaskProgress && MyDataInfo.sceneAllZiYuan[i].ZiYuanType != ZiYuanType.Hospital)
                    continue;
            if (info.type == 2)
                if (!(MyDataInfo.sceneAllZiYuan[i] is ITaskProgress) || MyDataInfo.sceneAllZiYuan[i].ZiYuanType == ZiYuanType.Hospital)
                    continue;

            var zyItem = GameObject.Instantiate(zyTemplate, zyParent);
            var zy = MyDataInfo.sceneAllZiYuan[i];
            zyItem.Init(zy.ziYuanName, zy.BobjectId, info.currentInfo != null && info.currentInfo.Contains(zy.BobjectId));
            zyItem.gameObject.SetActive(true);
        }

        view.SetActive(true);
    }

    public override void OnHide()
    {
        for (int i = 0; i < zyParent.childCount; i++)
        {
            GameObject.Destroy(zyParent.GetChild(i).gameObject);
        }

        view.SetActive(false);
    }

    public override void OnSave()
    {
        //获取列表中所有勾选的项，加入选中列表中
        List<string> chooseZy = new List<string>();
        for (int i = 0; i < zyParent.childCount; i++)
        {
            var item = zyParent.GetChild(i).GetComponent<ChangeData_cellZyItem>();
            if (item.GetIsChoose())
            {
                chooseZy.Add(item.zyId);
            }
        }

        //再把当前数据中另一个type的项再加进去
        for (int i = 0; i < info.currentInfo?.Count; i++)
        {
            var ziyuan = MyDataInfo.sceneAllZiYuan?.Find(x => string.Equals(x.BobjectId, info.currentInfo[i]));
            if (ziyuan == null) continue;
            if (info.type == 1)
            {
                //加目前所有灾区
                if (ziyuan is ITaskProgress) chooseZy.Add(ziyuan.BobjectId);
            }

            if (info.type == 2)
            {
                //加目前所有资源
                if (!(ziyuan is ITaskProgress)) chooseZy.Add(ziyuan.BobjectId);
            }
        }

        info.sureCb?.Invoke(chooseZy);
    }
}

//天气设定
public class TianQiSetView : ChangeDataBase
{
    private GameObject view;
    private Dropdown dptq, dpfl;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/tianQiSetPart").gameObject;
        dptq = view.transform.Find("dp_tqSetting").GetComponent<Dropdown>();
        dpfl = view.transform.Find("dp_flSetting").GetComponent<Dropdown>();
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
        mainView.ChangeViewSize(2);
        mainView.ChangeTitleInfo("天气设定");
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        //发出当前天气情况
        string tqStr = dptq.value.ToString() + '_' + dpfl.value.ToString();
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendTianQi, tqStr);
    }
}

//直升机装备故障
public class MalfunctionView : ChangeDataBase
{
    private GameObject view;
    private RectTransform equipParent;
    private ChangeData_cellEquipItem equipItem;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/malfunctionPart").gameObject;
        equipParent = view.transform.GetComponentInChildren<ScrollRect>(true).content;
        equipItem = view.transform.GetComponentInChildren<ChangeData_cellEquipItem>(true);
    }

    public override void OnShow(object data)
    {
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            var eqItem = GameObject.Instantiate(equipItem, equipParent);
            var equip = MyDataInfo.sceneAllEquips[i];
            eqItem.Init(equip.name, equip.BObjectId, equip.CurrentState);
            eqItem.gameObject.SetActive(true);
        }

        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("直升机装备故障设定");
        view.SetActive(true);
    }

    public override void OnHide()
    {
        for (int i = 0; i < equipParent.childCount; i++)
        {
            GameObject.Destroy(equipParent.GetChild(i).gameObject);
        }

        view.SetActive(false);
    }

    public override void OnSave()
    {
        //获取到界面上所有状态，和所有端同步
        string equipStates = "";
        for (int i = 0; i < equipParent.childCount; i++)
        {
            var item = equipParent.GetChild(i).GetComponent<ChangeData_cellEquipItem>();
            equipStates += item.myId + ':' + item.GetState() + '_';
        }

        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendEquipState, equipStates);
    }
}

//资源数据设置，补给点存油量，物资点存物资量
public class SupplyOrGoodsView : ChangeDataBase
{
    private GameObject view;
    private ZyComsInfo info;
    private Text _text;
    private InputField _inputField;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/supplyOrGoodsPart").gameObject;
        _text = view.GetComponentInChildren<Text>(true);
        _inputField = view.GetComponentInChildren<InputField>(true);
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
        mainView.ChangeViewSize(2);
        mainView.ChangeTitleInfo("资源数据设定");
        info = (ZyComsInfo)data;
        switch (info.zyType)
        {
            case ZiYuanType.Supply:
                _text.text = "存储油量(Kg)";
                _inputField.text = (info.currentData as SupplyVariableData)?.oilNum.ToString();
                break;
            case ZiYuanType.GoodsPoint:
                _text.text = "物资数量(Kg)";
                _inputField.text = (info.currentData as GoodsPointVariableData)?.goodsNum.ToString();
                break;
        }
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        switch (info.zyType)
        {
            case ZiYuanType.Supply:
                if (info.currentData != null) (info.currentData as SupplyVariableData).oilNum = float.Parse(_inputField.text);
                else info.currentData = new SupplyVariableData() { oilNum = float.Parse(_inputField.text) };
                break;
            case ZiYuanType.GoodsPoint:
                if (info.currentData != null) (info.currentData as GoodsPointVariableData).goodsNum = float.Parse(_inputField.text);
                else info.currentData = new GoodsPointVariableData() { goodsNum = float.Parse(_inputField.text) };
                break;
        }

        info.changeComs?.Invoke(info.currentData);
    }
}

//任务背景设置部分
public class TaskBgSettingView : ChangeDataBase
{
    private GameObject view;
    private InputField taskTarget, disInfo, kongGuan;
    private Dropdown tqSetting, flSetting;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/taskBgSettingPart").gameObject;
        taskTarget = view.transform.Find("InputF_taskTarget").GetComponent<InputField>();
        disInfo = view.transform.Find("InputF_disInfo").GetComponent<InputField>();
        kongGuan = view.transform.Find("InputF_kongGuan").GetComponent<InputField>();
        tqSetting = view.transform.Find("dp_tqSetting").GetComponent<Dropdown>();
        flSetting = view.transform.Find("dp_flSetting").GetComponent<Dropdown>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("任务背景设置");
        view.SetActive(true);
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        Debug.LogError(getStrData());
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendTaskBgInfo, getStrData());
    }

    private string getStrData()
    {
        return tqSetting.value.ToString() + '_' + flSetting.value.ToString() + '_' + taskTarget.text + '_' + disInfo.text + '_' + kongGuan.text;
    }
}

//地面保障设置部分
public class GroundSupportDataView : ChangeDataBase
{
    private GameObject view;
    private ChangeData_cellGroundZySet _cellGroundZySet;
    private Transform zyParent;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/groundSupportDataPart").gameObject;
        _cellGroundZySet = view.GetComponentInChildren<ChangeData_cellGroundZySet>(true);
        zyParent = view.GetComponentInChildren<ScrollRect>(true).content;
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        switch ((ShowZyDataType)((ShowViewInfoBase)data).showType)
        {
            case ShowZyDataType.GroundSupport:
                mainView.ChangeTitleInfo("地面保障数据");
                for (int i = 0; i < MyDataInfo.sceneAllZiYuan.Count; i++)
                {
                    var zy = MyDataInfo.sceneAllZiYuan[i];
                    if (zy.ZiYuanType == ZiYuanType.SourceOfAFire || zy.ZiYuanType == ZiYuanType.DisasterArea) continue;
                    var zyItem = GameObject.Instantiate(_cellGroundZySet, zyParent);
                    zyItem.Init(zy);
                    zyItem.gameObject.SetActive(true);
                }

                break;
            case ShowZyDataType.GroundDisaster:
                mainView.ChangeTitleInfo("地面灾情点数据");
                for (int i = 0; i < MyDataInfo.sceneAllZiYuan.Count; i++)
                {
                    var zy = MyDataInfo.sceneAllZiYuan[i];
                    if (zy.ZiYuanType == ZiYuanType.SourceOfAFire || zy.ZiYuanType == ZiYuanType.DisasterArea)
                    {
                        var zyItem = GameObject.Instantiate(_cellGroundZySet, zyParent);
                        zyItem.Init(zy);
                        zyItem.gameObject.SetActive(true);
                    }
                }

                break;
        }

        view.SetActive(true);
    }

    public override void OnHide()
    {
        for (int i = 0; i < zyParent.childCount; i++)
        {
            GameObject.Destroy(zyParent.GetChild(i).gameObject);
        }

        view.SetActive(false);
    }

    public override void OnSave()
    {
        for (int i = 0; i < zyParent.childCount; i++)
        {
            string zysetData = zyParent.GetChild(i).GetComponent<ChangeData_cellGroundZySet>().GetSaveData();
            if (string.IsNullOrEmpty(zysetData)) continue;
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendZySetData, zysetData);
        }
    }
}

//机组人员设置部分，导教端修改页面和一级查看页面共用
public class PersonSetView : ChangeDataBase
{
    private GameObject view;
    private Transform jizuParent, baozhangParent;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/personSetPart").gameObject;
        jizuParent = view.transform.Find("ScrollR_Jizuu").GetComponent<ScrollRect>().content;
        baozhangParent = view.transform.Find("ScrollR_Baozhang").GetComponent<ScrollRect>().content;
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("机组人员设置");
        view.SetActive(true);

        //一级指挥打开的时候，要把信息初始化进去，，导教端打开不用管，数据让他自己填
        if (data is ShowStrInputData)
        {
            var cellsInfo = (data as ShowStrInputData).strInfo.Split(';');

            //所有机组信息
            var jizuInfos = cellsInfo[0].Split(':');
            for (int i = 0; i < jizuParent.childCount; i++)
            {
                ChangeData_cellPersonInfoItem personCell = jizuParent.GetChild(i).GetComponent<ChangeData_cellPersonInfoItem>();
                personCell?.Init(jizuInfos[i]);
            }

            //所有保障组信息
            var baozhangInfos = cellsInfo[1].Split(':');
            for (int i = 0; i < baozhangParent.childCount; i++)
            {
                ChangeData_cellPersonInfoItem personCell = baozhangParent.GetChild(i).GetComponent<ChangeData_cellPersonInfoItem>();
                personCell?.Init(baozhangInfos[i]);
            }
        }
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        //只有导教端才有修改机组人员状态的权限，一级指挥只是查看
        //机组人员显示状态 发送
        string choosePersons = "";
        for (int i = 0; i < jizuParent.childCount; i++)
        {
            choosePersons += jizuParent.GetChild(i).GetComponent<ChangeData_cellPersonInfoItem>().GetData() + ':';
        }

        choosePersons += ';'; //机组数据和保障数据用;隔开
        for (int i = 0; i < baozhangParent.childCount; i++)
        {
            choosePersons += baozhangParent.GetChild(i).GetComponent<ChangeData_cellPersonInfoItem>().GetData() + ':';
        }

        Debug.LogError(choosePersons);
        if (MyDataInfo.MyLevel == -1)
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendPersonsInfo, choosePersons);
        else if (MyDataInfo.MyLevel == 1)
        {
            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendPersonUsedInfo, choosePersons);

            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.ZBLDSurePersonInfo.ToString());
        }
    }
}

//任务背景查看部分,,这里应该删掉，展示灾情信息界面就行
public class ShowTaskBgDataView : ChangeDataBase
{
    private GameObject view;
    private InputField taskTarget, disInfo, kongGuan;
    private Dropdown tqSetting, flSetting;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/showTaskBgDataPart").gameObject;
        taskTarget = view.transform.Find("InputF_taskTarget").GetComponent<InputField>();
        disInfo = view.transform.Find("InputF_disInfo").GetComponent<InputField>();
        kongGuan = view.transform.Find("InputF_kongGuan").GetComponent<InputField>();
        tqSetting = view.transform.Find("dp_tqSetting").GetComponent<Dropdown>();
        flSetting = view.transform.Find("dp_flSetting").GetComponent<Dropdown>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("任务背景信息");
        view.SetActive(true);
        if (data is ShowStrInputData)
        {
            var strs = (data as ShowStrInputData).strInfo.Split('_');
            if (strs != null && strs.Length == 5)
            {
                tqSetting.value = int.Parse(strs[0]);
                flSetting.value = int.Parse(strs[1]);
                taskTarget.text = strs[2];
                disInfo.text = strs[3];
                kongGuan.text = strs[4];
            }
            else
            {
                EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "任务背景信息还未收到");
            }
        }
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
    }
}

//灾害类型确认部分
public class DisasterSituationView : ChangeDataBase
{
    private GameObject view;
    private Text disInfo;
    private InputField zhlx, zhgm;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/disasterSituationPart").gameObject;
        disInfo = view.transform.Find("text_disInfo").GetComponent<Text>();
        zhlx = view.transform.Find("InputF_zhlx").GetComponent<InputField>();
        zhgm = view.transform.Find("InputF_zhgm").GetComponent<InputField>();
    }

    public override void OnShow(object data)
    {
        ShowStrInputData sdsi = data as ShowStrInputData;
        mainView.ChangeTitleInfo("灾害信息");
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        disInfo.text = sdsi.strInfo;
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        //存到cc中的数据结构中，用于报告显示
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.ZBLDSureDisasterInfo.ToString());
    }
}

/// <summary>
/// 装备信息,导教端修改页面和一级查看页面共用
/// </summary>
public class EquipmentInfoView : ChangeDataBase
{
    private GameObject view;
    private Transform equipParent;
    private ChangeData_cellEquipInfoItem cell;
    private List<ChangeData_cellEquipInfoItem> equips;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/equipmentInfoPart").gameObject;
        equipParent = view.transform.Find("SR_equipParent").GetComponentInChildren<ScrollRect>().content;
        cell = view.transform.Find("equipCell").GetComponent<ChangeData_cellEquipInfoItem>();
        equips = new List<ChangeData_cellEquipInfoItem>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeTitleInfo("装备信息");
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        if (equipParent.childCount != 0) return;
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            ChangeData_cellEquipInfoItem item = GameObject.Instantiate(cell, equipParent);
            item.Init(MyDataInfo.sceneAllEquips[i]);
            equips.Add(item);
            item.gameObject.SetActive(true);
        }

        view.transform.Find("TitleInfo/group").gameObject.SetActive(MyDataInfo.MyLevel == 1);
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        if (MyDataInfo.MyLevel == -1)
        {
            string equipDatas = "";
            for (int i = 0; i < equips.Count; i++)
            {
                equipDatas += equips[i].getData() + ':';
            }

            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendEquipsInfo, equipDatas);
        }
        else if (MyDataInfo.MyLevel == 1)
        {
            string equipDatas = "";
            for (int i = 0; i < equips.Count; i++)
            {
                equipDatas += equips[i].GetUsedEquipId + ':';
            }

            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendEquipUsedInfo, equipDatas);

            EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.ZBLDSureEquipInfo.ToString());
        }
    }
}


/// <summary>
/// 人员信息,这里应该删掉
/// </summary>
public class PersonnelInfoView : ChangeDataBase
{
    private GameObject view;

    /// <summary>
    /// 空勤人员
    /// </summary>
    private Transform aircrewParent;

    /// <summary>
    /// 机务人员
    /// </summary>
    private Transform arcraftCrewParent;


    private ChangeData_cellPersonInfoItem cellAircrew, cellArcraftCrew;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/personnelInfoPart").gameObject;
        aircrewParent = view.transform.Find("Aircrew/SR_Parent").GetComponentInChildren<ScrollRect>().content;
        arcraftCrewParent = view.transform.Find("AircraftCrew/SR_Parent").GetComponentInChildren<ScrollRect>().content;

        cellAircrew = view.transform.Find("Aircrew/Cell").GetComponent<ChangeData_cellPersonInfoItem>();
        cellArcraftCrew = view.transform.Find("AircraftCrew/Cell").GetComponent<ChangeData_cellPersonInfoItem>();
    }

    public override void OnShow(object data)
    {
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        //存到cc中的数据结构中，用于报告显示
    }
}


/// <summary>
/// 空管信息
/// </summary>
public class AirTrafficControlInfoView : ChangeDataBase
{
    private GameObject view;

    private InputField kg;
    private ChangeData_cellFairWayInfoItem cell;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/airTrafficControlInfoPart").gameObject;
        kg = view.transform.Find("InputF_fairway").GetComponent<InputField>();
        cell = view.transform.Find("fairway").GetComponent<ChangeData_cellFairWayInfoItem>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeTitleInfo("空管信息");
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        kg.text = (data as ShowStrInputData).strInfo;
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
    }
}

/// <summary>
/// 任务信息
/// </summary>
public class TaskInfoView : ChangeDataBase
{
    private GameObject view;
    private Text text_tq;
    private Image imgTask;
    private GameObject hz, sz;

    /// <summary>
    /// 受灾数量点
    /// </summary>
    private InputField InputField_sz;

    /// <summary>
    /// 医院数量点
    /// </summary>
    private InputField InputField_yy;

    /// <summary>
    /// 补给数量
    /// </summary>
    private InputField InputField_bj;

    /// <summary>
    /// 取水点数量
    /// </summary>
    private InputField InputField_qs;

    /// <summary>
    /// 安置点数量
    /// </summary>
    private InputField InputField_az;

    /// <summary>
    /// 起降点数量
    /// </summary>
    private InputField InputField_qj;

    /// <summary>
    /// 火场点数量
    /// </summary>
    private InputField InputField_hc;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/taskInfoPart").gameObject;
        text_tq = view.transform.Find("text_tq").GetComponent<Text>();
        InputField_sz = view.transform.Find("grid/inputpoints/input").GetComponent<InputField>();
        InputField_yy = view.transform.Find("grid/inputpoints (1)/input").GetComponent<InputField>();
        InputField_bj = view.transform.Find("grid/inputpoints (2)/input").GetComponent<InputField>();
        InputField_qs = view.transform.Find("grid/inputpoints (3)/input").GetComponent<InputField>();
        InputField_az = view.transform.Find("grid/inputpoints (4)/input").GetComponent<InputField>();
        InputField_qj = view.transform.Find("grid/inputpoints (5)/input").GetComponent<InputField>();
        InputField_hc = view.transform.Find("grid/inputpoints (6)/input").GetComponent<InputField>();
        hz = view.transform.Find("hz").gameObject;
        sz = view.transform.Find("sz").gameObject;
    }

    public override void OnShow(object data)
    {
        mainView.ChangeTitleInfo("任务信息");
        mainView.ChangeViewSize(3);
        view.SetActive(true);
        hz.SetActive(MyDataInfo.gameScene == 1);
        sz.SetActive(MyDataInfo.gameScene == 2);
        if (data is ShowStrInputData)
            text_tq.text = "任务区气象条件：" + (data as ShowStrInputData).strInfo;
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        //存到cc中的数据结构中，用于报告显示
    }
}

/// <summary>
/// 二级指挥员
/// </summary>
public class FieldCommanderView : ChangeDataBase
{
    private GameObject view;
    private Dropdown checkEquip;
    private Text oilMax, loadMax;
    private Slider zyl, zzl;
    private float maxOil, maxZzl;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/fieldCommanderPart").gameObject;
        checkEquip = view.transform.Find("dp_checkEquip").GetComponent<Dropdown>();
        zyl = view.transform.Find("fuelSlider").GetChild(0).GetComponent<Slider>();
        zzl = view.transform.Find("loadSlider").GetChild(0).GetComponent<Slider>();
        oilMax = view.transform.Find("fuelSlider").GetChild(2).GetComponent<Text>();
        loadMax = view.transform.Find("loadSlider").GetChild(2).GetComponent<Text>();
        checkEquip.onValueChanged.AddListener(OnChangEquip);
        zyl.onValueChanged.AddListener(OnChangeOilNum);
        zzl.onValueChanged.AddListener(OnChangeZzlNum);
    }

    public override void OnShow(object data)
    {
        mainView.ChangeTitleInfo("任务前准备信息");
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        checkEquip.options.Clear();
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            checkEquip.options.Add(new Dropdown.OptionData(MyDataInfo.sceneAllEquips[i].name));
        }

        checkEquip.value = 1;
        checkEquip.value = 0;
    }

    private void OnChangEquip(int index)
    {
        var _equip = MyDataInfo.sceneAllEquips[index];
        (_equip as IDqChangePart).GetOilAndLoad(out float oil, out float load);
        oilMax.text = oil.ToString();
        loadMax.text = load.ToString();
        maxOil = oil;
        maxZzl = load;
        OnChangeOilNum(zyl.value);
        OnChangeZzlNum(zzl.value);
    }

    private void OnChangeOilNum(float num)
    {
        zyl.GetComponentInChildren<Text>().text = (num * maxOil).ToString();
    }

    private void OnChangeZzlNum(float num)
    {
        zzl.GetComponentInChildren<Text>().text = (num * maxZzl).ToString();
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.XCZHInspectEquipInfo.ToString());
    }
}

/// <summary>
/// 机长
/// </summary>
public class CaptainView : ChangeDataBase
{
    private GameObject view;
    private Text jixing, bianhao, oilMax, loadMax;
    private Slider zyl, zzl;
    private EquipBase _equip;
    private float maxOil, maxZzl;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/captainPart").gameObject;
        jixing = view.transform.Find("modeldes").GetComponent<Text>();
        bianhao = view.transform.Find("numberdes").GetComponent<Text>();
        zyl = view.transform.Find("fuelSlider").GetChild(0).GetComponent<Slider>();
        zzl = view.transform.Find("loadSlider").GetChild(0).GetComponent<Slider>();
        oilMax = view.transform.Find("fuelSlider").GetChild(2).GetComponent<Text>();
        loadMax = view.transform.Find("loadSlider").GetChild(2).GetComponent<Text>();
        zyl.onValueChanged.AddListener(OnChangeOilNum);
        zzl.onValueChanged.AddListener(OnChangeZzlNum);
    }

    private void OnChangeOilNum(float num)
    {
        zyl.GetComponentInChildren<Text>().text = (num * maxOil).ToString();
    }

    private void OnChangeZzlNum(float num)
    {
        zzl.GetComponentInChildren<Text>().text = (num * maxZzl).ToString();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeTitleInfo("地面准备信息");
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        _equip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BeLongToCommanderId, MyDataInfo.leadId));
        if (_equip == null)
        {
            Debug.LogError("找不到我自己的飞机");
            return;
        }

        (_equip as IDqChangePart).GetOilAndLoad(out float oil, out float load);
        jixing.text = bianhao.text = _equip.name;
        oilMax.text = oil.ToString();
        loadMax.text = load.ToString();
        maxOil = oil;
        maxZzl = load;
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        string info = _equip.BObjectId + '_' + zyl.value.ToString() + '_' + zzl.value.ToString();
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendChangeEquipOilAndLoad, info);
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)MessageID.SendTrainPointSucInfo, TrainsPintType.JZSureOilAndLoad.ToString());
    }
}