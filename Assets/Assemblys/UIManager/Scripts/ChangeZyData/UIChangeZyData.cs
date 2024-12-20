using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeZyData : BasePanel
{
    private Text title;
    private RectTransform bgImage;
    private FireDataView _fireDataView;
    private DisasterDataView _disasterDataView;
    private ZYFPPartView _zyfpPartView;
    private TianQiSetView _tianQiSetView;
    private MalfunctionView _malfunctionView;
    private SupplyOrGoodsView _supplyOrGoodsView;
    private TaskBgSettingView _taskBgSettingView;
    private GroundSupportDataView _groundSupportDataView;
    private EquipsAndPersonSetView _equipsAndPersonSetView;
    private ShowTaskBgDataView _showTaskBgDataView;
    private DisasterSituationView _disasterSituationView;

    private ChangeDataBase _currentView;

    public override void Init()
    {
        base.Init();
        title = GetControl<Text>("title");
        bgImage = transform.GetChild(0).GetComponent<RectTransform>();
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
        _taskBgSettingView = new TaskBgSettingView();
        _taskBgSettingView.Init(this);
        _groundSupportDataView = new GroundSupportDataView();
        _groundSupportDataView.Init(this);
        _equipsAndPersonSetView = new EquipsAndPersonSetView();
        _equipsAndPersonSetView.Init(this);
        _showTaskBgDataView = new ShowTaskBgDataView();
        _showTaskBgDataView.Init(this);
        _disasterSituationView = new DisasterSituationView();
        _disasterSituationView.Init(this);
        GetControl<Button>("close").onClick.AddListener(() => Close(UIName.UIChangeZyData));
        GetControl<Button>("sure").onClick.AddListener(() =>
        {
            _currentView?.OnSave();
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
                    _currentView = _fireDataView;
                    break;
                case ZiYuanType.DisasterArea:
                    _currentView = _disasterDataView;
                    break;
            }
        }

        if (userData is ZyComsInfo)
        {
            switch ((userData as ZyComsInfo).zyType)
            {
                case ZiYuanType.Supply:
                case ZiYuanType.GoodsPoint:
                    _currentView = _supplyOrGoodsView;
                    break;
            }
        }

        if (userData is ZyfpInfo)
        {
            _currentView = _zyfpPartView;
        }

        if (userData is int)
        {
            if ((int)userData == 1) _currentView = _tianQiSetView;
            if ((int)userData == 2) _currentView = _malfunctionView;
            if ((int)userData == 3) _currentView = _taskBgSettingView;
            if ((int)userData == 4) _currentView = _groundSupportDataView;
            if ((int)userData == 5) _currentView = _equipsAndPersonSetView;
            if ((int)userData == 6) _currentView = _groundSupportDataView;
        }

        if (userData is string)
        {
            _currentView = _showTaskBgDataView;
        }

        if (userData is ShowDisasterSituationInfo)
        {
            _currentView = _disasterSituationView;
        }


        _currentView?.OnShow(userData);
    }

    public void ChangeTitleInfo(string infoStr)
    {
        title.text = infoStr;
    }

    public void ChangeViewSize(int type)
    {
        switch (type)
        {
            case 1:
                bgImage.sizeDelta = new Vector2(650, 520);
                break;
            case 2:
                bgImage.sizeDelta = new Vector2(442, 256 + 46);
                break;
        }
    }

    public override void HideMe()
    {
        base.HideMe();
        _currentView?.OnHide();
        _currentView = null;
    }
}

public abstract class ChangeDataBase
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