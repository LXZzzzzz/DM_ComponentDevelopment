using System;
using UiManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIChangeZyData : BasePanel
{
    private FireDataView _fireDataView;

    private ChangeZyDataBase currentZyView;

    public override void Init()
    {
        base.Init();
        _fireDataView.Init(this);
        GetControl<Button>("close").onClick.AddListener(()=>Close(UIName.UIChangeZyData));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);


        currentZyView.OnShow(userData);
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
}

public class FireDataView : ChangeZyDataBase
{
    private GameObject view;
    private InputField fs, pd, csrsmj;

    protected override void OnInit()
    {
        fs = mainView.myGetcontrol<InputField>("aaa");
        pd = mainView.myGetcontrol<InputField>("bbb");
        csrsmj = mainView.myGetcontrol<InputField>("ccc");
    }

    public override void OnShow(object data)
    {
        view.SetActive(true);
    }

    public override void OnHide()
    {
        view.SetActive(false);
    }
}