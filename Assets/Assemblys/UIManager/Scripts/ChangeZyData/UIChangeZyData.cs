using Enums;
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
    private PersonSetView _personSetView;
    private EquipmentInfoView _equipmentInfoView;
    private ShowTaskBgDataView _showTaskBgDataView;
    private DisasterSituationView _disasterSituationView;
    private AirTrafficControlInfoView _airTrafficControlInfoView;
    private TaskInfoView _taskInfoView;
    private FieldCommanderView _fieldCommanderView;
    private CaptainView _captainView;

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
        _personSetView = new PersonSetView();
        _personSetView.Init(this);
        _equipmentInfoView = new EquipmentInfoView();
        _equipmentInfoView.Init(this);
        _showTaskBgDataView = new ShowTaskBgDataView();
        _showTaskBgDataView.Init(this);
        _disasterSituationView = new DisasterSituationView();
        _disasterSituationView.Init(this);
        _airTrafficControlInfoView = new AirTrafficControlInfoView();
        _airTrafficControlInfoView.Init(this);
        _taskInfoView = new TaskInfoView();
        _taskInfoView.Init(this);
        _fieldCommanderView = new FieldCommanderView();
        _fieldCommanderView.Init(this);
        _captainView = new CaptainView();
        _captainView.Init(this);
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

        if (userData is ShowViewInfoBase)
        {
            switch ((ShowZyDataType)((ShowViewInfoBase)userData).showType)
            {
                case ShowZyDataType.TaskBgShow:
                    _currentView = _taskBgSettingView;
                    break;
                case ShowZyDataType.GroundSupport:
                case ShowZyDataType.GroundDisaster:
                    _currentView = _groundSupportDataView;
                    break;
                case ShowZyDataType.EquipsShow:
                case ShowZyDataType.zbxxShow:
                    _currentView = _equipmentInfoView;
                    break;
                case ShowZyDataType.PersonShow:
                case ShowZyDataType.ryxxShow:
                    _currentView = _personSetView;
                    break;
                case ShowZyDataType.TqChange:
                    _currentView = _tianQiSetView;
                    break;
                case ShowZyDataType.zbgzChange:
                    _currentView = _malfunctionView;
                    break;
                case ShowZyDataType.zqxxShow:
                    _currentView = _disasterSituationView;
                    break;
                case ShowZyDataType.kgxxShow:
                    _currentView = _airTrafficControlInfoView;
                    break;
                case ShowZyDataType.rwxxShow:
                    _currentView = _taskInfoView;
                    break;
                case ShowZyDataType.rwqzbShow:
                    _currentView = _fieldCommanderView;
                    break;
                case ShowZyDataType.dmzbShow:
                    _currentView = _captainView;
                    break;
            }
        }


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