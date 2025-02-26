using System.Collections.Generic;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UIPeculiarInfoShow : BasePanel
{
    private Transform peculiarInfosParent;
    public GameObject peculiarCell;
    private List<GameObject> peculiars;

    public override void Init()
    {
        base.Init();
        peculiarInfosParent = GetControl<ScrollRect>("ScrollRectPrefab").content;
        peculiars = new List<GameObject>();
        GetControl<Button>("close").onClick.AddListener(()=>Close(UIName.UIPeculiarInfoShow));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        List<string> itemData = (List<string>)userData;
        peculiars.ForEach(Destroy);
        peculiars.Clear();
        for (int i = 0; i < itemData.Count; i++)
        {
            var item = Instantiate(peculiarCell, peculiarInfosParent);
            item.GetComponentInChildren<Text>().text = $"{i + 1}." + itemData[i];
            item.SetActive(true);
            peculiars.Add(item);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
    }
}