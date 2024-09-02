using System.Collections.Generic;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIChangeControllers : BasePanel
{
    private RectTransform viewPoint;
    private Transform menusParent;
    private ChangeControllerCell cccell;
    private List<ChangeControllerCell> ccCells;
    private UnityAction<List<string>> sureCb;
    private List<string> dataComs;

    public override void Init()
    {
        base.Init();
        viewPoint = transform.GetChild(0).GetComponent<RectTransform>();
        menusParent = GetControl<ScrollRect>("menuListView").content;
        cccell = GetComponentInChildren<ChangeControllerCell>(true);
        GetComponent<Button>().onClick.AddListener(OnSure);
        ccCells = new List<ChangeControllerCell>();
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);

        for (int i = 0; i < MyDataInfo.playerInfos?.Count; i++)
        {
            var ccitem = Instantiate(cccell, menusParent);
            ccitem.Init(MyDataInfo.playerInfos[i].RoleId, MyDataInfo.playerInfos[i].ClientLevelName, MyDataInfo.playerInfos[i].MyColor);
            ccitem.gameObject.SetActive(true);
            ccCells.Add(ccitem);
        }

        ZyComsInfo data = (ZyComsInfo)userData;
        viewPoint.position = data.pos;
        sureCb = data.changeComs;
        dataComs = data.coms;
        for (int i = 0; i < ccCells.Count; i++)
        {
            ccCells[i].ChangeState(data.coms?.Find(x => string.Equals(x, ccCells[i].comId)) != null);
        }
    }

    private void OnSure()
    {
        var items = ccCells?.FindAll(x => x.isChoose);

        List<string> coms = new List<string>();
        for (int i = 0; i < items.Count; i++)
        {
            coms.Add(items[i].comId);
        }

        bool ischange = false;
        if (dataComs?.Count == coms.Count)
        {
            for (int i = 0; i < dataComs.Count; i++)
            {
                if (!coms.Contains(dataComs[i]))
                {
                    ischange = true;
                    break;
                }
            }
        }
        else ischange = true;

        if (ischange) sureCb?.Invoke(coms);
        Close(UIName.UIChangeControllers);
    }

    public override void HideMe()
    {
        base.HideMe();
        for (int i = 0; i < ccCells.Count; i++)
        {
            Destroy(ccCells[i].gameObject);
        }

        ccCells.Clear();
    }
}