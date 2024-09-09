using System.Collections;
using System.Collections.Generic;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UICommanderDirector : BasePanel
{
    private Transform zyParent;
    private ZaiQuTemplateCell zqc;

    public override void Init()
    {
        base.Init();
        zyParent = GetControl<ScrollRect>("ScrollRectZQMB").content;
        zqc = transform.GetComponentInChildren<ZaiQuTemplateCell>(true);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
#if !UNITY_EDITOR
        showZaiqu();
#endif
    }

    private void showZaiqu()
    {
        //获取场景全部组件，并找到灾区模板
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem == null || tagItem.SubTags.Find(y => y.Id == 6) == null) continue;

            if (allBObjects[i].GetComponent<ZiYuanBase>() != null)
            {
                var zyItem = allBObjects[i].GetComponent<ZiYuanBase>();
                var zqitem = Instantiate(zqc, zyParent);
                zqitem.Init(zyItem);
                zqitem.gameObject.SetActive(true);
            }
        }
    }
}