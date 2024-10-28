using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PathPointCell : DMonoBehaviour
{
    public Text showInfo;
    public Button insertToAhead, insertToBehind, editBtn, deleBtn;
    private UnityAction<string> removeCb;
    private UnityAction<string, bool> insertCb;

    public void Init(string pointId, UnityAction<string> removeCb, UnityAction<string, bool> insertCb)
    {
        //四个按钮的功能绑定
        this.removeCb = removeCb;
        this.insertCb = insertCb;
        showInfo.text = $"{pointId}路径点";
        editBtn.onClick.AddListener(() =>
        {
            var pathPointData = PathPointManager.Instance.GetPointDataById(pointId);
            UIManager.Instance.ShowPanel<UIChangePointDataInfo>(UIName.UIChangePointDataInfo, pathPointData);
        });
        deleBtn.onClick.AddListener(() =>
        {
            removeCb?.Invoke(pointId);
            Destroy(gameObject);
        });
        insertToAhead.onClick.AddListener(() => { insertCb?.Invoke(pointId, true); });
        insertToBehind.onClick.AddListener(() => { insertCb?.Invoke(pointId, false); });
    }
}