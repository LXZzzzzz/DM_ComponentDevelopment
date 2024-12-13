using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine.UI;

public class UIChangeZyData : BasePanel
{
    private Text title;
    private FireDataView _fireDataView;
    private DisasterDataView _disasterDataView;
    private ZYFPPartView _zyfpPartView;
    private TianQiSetView _tianQiSetView;
    private MalfunctionView _malfunctionView;
    private SupplyOrGoodsView _supplyOrGoodsView;

    private ChangeDataBase _currentView;

    public override void Init()
    {
        base.Init();
        title = GetControl<Text>("title");
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
        }


        _currentView?.OnShow(userData);
    }

    public void ChangeTitleInfo(string infoStr)
    {
        title.text = infoStr;
    }

    public override void HideMe()
    {
        base.HideMe();
        _currentView.OnHide();
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