using System.Collections.Generic;
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
    private InputField zyName, num, type;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/disasterPart").gameObject;
        zyName = view.transform.Find("name/inputF_name").GetComponent<InputField>();
        num = view.transform.Find("num/inputF_num").GetComponent<InputField>();
        type = view.transform.Find("type/inputF_type").GetComponent<InputField>();
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
        DisasterVariableData variableData = new DisasterVariableData() { ZyType = ZiYuanType.DisasterArea, ZyName = zyName.text, personNum = int.Parse(num.text), type = int.Parse(type.text) };
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
                if (MyDataInfo.sceneAllZiYuan[i] is ITaskProgress)
                    continue;
            if (info.type == 2)
                if (!(MyDataInfo.sceneAllZiYuan[i] is ITaskProgress))
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
                _text.text = "存储油量";
                _inputField.text = (info.currentData as SupplyVariableData)?.oilNum.ToString();
                break;
            case ZiYuanType.GoodsPoint:
                _text.text = "物资数量";
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
    private Dropdown tqSetting;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/taskBgSettingPart").gameObject;
        taskTarget = view.transform.Find("InputF_taskTarget").GetComponent<InputField>();
        disInfo = view.transform.Find("InputF_disInfo").GetComponent<InputField>();
        kongGuan = view.transform.Find("InputF_kongGuan").GetComponent<InputField>();
        tqSetting = view.transform.Find("dp_tqSetting").GetComponent<Dropdown>();
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
        Debug.LogError(tqSetting.value);
        Debug.LogError(getStrData());
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendTaskBgInfo, getStrData());
    }

    private string getStrData()
    {
        return tqSetting.value.ToString() + '_' + taskTarget.text + '_' + disInfo.text + '_' + kongGuan.text;
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
        switch ((int)data)
        {
            case 4:
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
            case 6:
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

//装备和机组人员设置部分
public class EquipsAndPersonSetView : ChangeDataBase
{
    private GameObject view;
    private Transform equipParent, jizuParent;
    private ChangeData_cellEquipChooseItem equipCell;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/equipsAndPersonSetPart").gameObject;
        equipParent = view.transform.Find("ScrollR_Equip").GetComponent<ScrollRect>().content;
        jizuParent = view.transform.Find("ScrollR_Jizu").GetComponent<ScrollRect>().content;
        equipCell = view.transform.Find("equipCell").GetComponent<ChangeData_cellEquipChooseItem>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("装备和机组人员设置");
        view.SetActive(true);
        //展示所有直升机，选择启用情况
        //展示所有机组人员，可增删
        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            var eqItem = GameObject.Instantiate(equipCell, equipParent);
            eqItem.Init(MyDataInfo.sceneAllEquips[i]);
            eqItem.gameObject.SetActive(true);
        }
    }

    public override void OnHide()
    {
        view.SetActive(false);
        for (int i = 0; i < equipParent.childCount; i++)
        {
            GameObject.Destroy(equipParent.GetChild(i).gameObject);
        }
    }

    public override void OnSave()
    {
        //装备可用状态和机组人员显示状态 发送
        string chooseEquips = "";
        for (int i = 0; i < equipParent.childCount; i++)
        {
            string chooseId = equipParent.GetChild(i).GetComponent<ChangeData_cellEquipChooseItem>().GetRunEquip();
            if (string.IsNullOrEmpty(chooseId)) continue;
            chooseEquips += chooseId + '_';
        }

        //发送出去
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendUseEquips, chooseEquips);
        string chooseJizus = "";
        for (int i = 0; i < jizuParent.childCount; i++)
        {
            if (jizuParent.GetChild(i).GetComponentInChildren<Toggle>().isOn)
                chooseJizus += jizuParent.GetChild(i).GetComponentInChildren<Text>().text + '_';
        }

        Debug.LogError(chooseJizus);
        EventManager.Instance.EventTrigger(EventType.SendSkillInfoForControler.ToString(), (int)Enums.MessageID.SendUseJizus, chooseJizus);
    }
}

//任务背景查看部分
public class ShowTaskBgDataView : ChangeDataBase
{
    private GameObject view;
    private InputField taskTarget, disInfo, kongGuan, oilNum;
    private Dropdown tqSetting;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/showTaskBgDataPart").gameObject;
        taskTarget = view.transform.Find("InputF_taskTarget").GetComponent<InputField>();
        disInfo = view.transform.Find("InputF_disInfo").GetComponent<InputField>();
        kongGuan = view.transform.Find("InputF_kongGuan").GetComponent<InputField>();
        oilNum = view.transform.Find("InputF_oilNum").GetComponent<InputField>();
        tqSetting = view.transform.Find("dp_tqSetting").GetComponent<Dropdown>();
    }

    public override void OnShow(object data)
    {
        mainView.ChangeViewSize(1);
        mainView.ChangeTitleInfo("任务背景信息");
        view.SetActive(true);
        var strs = (data as string).Split('_');
        if (strs != null && strs.Length == 5)
        {
            tqSetting.value = int.Parse(strs[0]);
            taskTarget.text = strs[1];
            disInfo.text = strs[2];
            kongGuan.text = strs[3];
            oilNum.text = strs[4];
        }
        else
        {
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), "任务背景信息还未收到");
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
        ShowDisasterSituationInfo sdsi = data as ShowDisasterSituationInfo;
        mainView.ChangeTitleInfo(sdsi.disInfo);
        mainView.ChangeViewSize(1);
        view.SetActive(true);
        disInfo.text = sdsi.disInfo;
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

public class test : ChangeDataBase
{
    protected override void OnInit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnShow(object data)
    {
        throw new System.NotImplementedException();
    }

    public override void OnHide()
    {
        throw new System.NotImplementedException();
    }

    public override void OnSave()
    {
        throw new System.NotImplementedException();
    }
}