using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ZiYuan_ComanderCell : DMonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private UnityAction<string, bool> removeCallBack;
    private string _comId;
    private Button removeBtn;

    public string comId => _comId;

    public void Init(string comName, string comId, UnityAction<string, bool> removeCallBack)
    {
        this._comId = comId;
        this.removeCallBack = removeCallBack;

        GetComponentInChildren<Text>(true).text = comName;
        removeBtn = GetComponentInChildren<Button>(true);
        GetComponent<Image>().color = MyDataInfo.playerInfos.Find(x => string.Equals(x.RoleId, comId)).MyColor;
        removeBtn.onClick.AddListener(OnRemoveMe);
        removeBtn.gameObject.SetActive(false);
    }

    private void OnRemoveMe()
    {
        removeCallBack?.Invoke(_comId, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if ( /*MyDataInfo.gameState == GameState.GameStart*/MyDataInfo.MyLevel != 1 || removeCallBack == null) return;
        removeBtn.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if (MyDataInfo.gameState == GameState.GameStart) return;
        removeBtn.gameObject.SetActive(false);
    }
}