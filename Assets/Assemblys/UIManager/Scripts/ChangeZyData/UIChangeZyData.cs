using System;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UIChangeZyData : BasePanel
{
    private FireDataView _fireDataView;
    private DisasterDataView _disasterDataView;
    private ZYFPPartView _zyfpPartView;
    private TianQiSetView _tianQiSetView;
    private MalfunctionView _malfunctionView;
    private SupplyOrGoodsView _supplyOrGoodsView;

    private ChangeZyDataBase currentZyView;

    public override void Init()
    {
        base.Init();
        _fireDataView = new FireDataView();
        _fireDataView.Init(this);
        _disasterDataView = new DisasterDataView();
        _disasterDataView.Init(this);
        _zyfpPartView = new ZYFPPartView();
        _zyfpPartView.Init(this);
        _tianQiSetView = new TianQiSetView();
        _tianQiSetView.Init(this);
        _malfunctionView = new MalfunctionView();
        _malfunctionView.Init(this);
        _supplyOrGoodsView = new SupplyOrGoodsView();
        _supplyOrGoodsView.Init(this);
        GetControl<Button>("close").onClick.AddListener(() => Close(UIName.UIChangeZyData));
        GetControl<Button>("sure").onClick.AddListener(() =>
        {
            currentZyView?.OnSave();
            Close(UIName.UIChangeZyData);
        });
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);

        if (userData is ZiYuanType)
        {
            switch ((ZiYuanType)userData)
            {
                case ZiYuanType.SourceOfAFire:
                    currentZyView = _fireDataView;
                    break;
                case ZiYuanType.DisasterArea:
                    currentZyView = _disasterDataView;
                    break;
            }
        }

        if (userData is ZyComsInfo)
        {
            switch ((userData as ZyComsInfo).zyType)
            {
                case ZiYuanType.Supply:
                case ZiYuanType.GoodsPoint:
                    currentZyView = _supplyOrGoodsView;
                    break;
            }
        }

        if (userData is ZyfpInfo)
        {
            currentZyView = _zyfpPartView;
        }

        if (userData is int)
        {
            if ((int)userData == 1) currentZyView = _tianQiSetView;
            if ((int)userData == 2) currentZyView = _malfunctionView;
        }


        currentZyView?.OnShow(userData);
    }

    public T myGetcontrol<T>(string name) where T : UIBehaviour
    {
        return GetControl<T>(name);
    }

    public override void HideMe()
    {
        base.HideMe();
        currentZyView.OnHide();
        currentZyView = null;
    }
}

public abstract class ChangeZyDataBase
{
    protected UIChangeZyData mainView;

    public void Init(UIChangeZyData mv)
    {
        mainView = mv;
        OnInit();
    }

    protected abstract void OnInit();
    public abstract void OnShow(object data);
    public abstract void OnHide();
    public abstract void OnSave();
}

public class FireDataView : ChangeZyDataBase
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
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
        Debug.LogError("写的名字" + zyName.text);
        FireVariableData variableData = new FireVariableData()
        {
            ZyType = ZiYuanType.SourceOfAFire, ZyName = zyName.text, csrsmj = float.Parse(csrsmj.text), fs = float.Parse(fs.text), pd = float.Parse(pd.text)
        };

        EventManager.Instance.EventTrigger<ZyVariableDataBase>(EventType.CreatZaiQuZyRun.ToString(), variableData);
    }
}

public class DisasterDataView : ChangeZyDataBase
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

public class ZYFPPartView : ChangeZyDataBase
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

public class TianQiSetView : ChangeZyDataBase
{
    private GameObject view;
    private Dropdown dptq;

    protected override void OnInit()
    {
        view = mainView.transform.Find("View/infos/tianQiSetPart").gameObject;
        dptq = view.transform.GetComponentInChildren<Dropdown>(true);
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }

    public override void OnSave()
    {
    }
}

public class MalfunctionView : ChangeZyDataBase
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
            eqItem.Init(equip.name, equip.BObjectId, equip.currentState);
            eqItem.gameObject.SetActive(true);
        }

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

public class SupplyOrGoodsView : ChangeZyDataBase
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