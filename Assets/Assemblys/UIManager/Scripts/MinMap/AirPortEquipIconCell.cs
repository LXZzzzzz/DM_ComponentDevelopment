using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class AirPortEquipIconCell : DMonoBehaviour
{
    private string id;
    public void Init(string id,Sprite icon)
    {
        this.id = id;
        transform.Find("icon").GetComponent<Image>().sprite = icon;
        transform.GetComponentInChildren<Button>().onClick.AddListener(openRightClickView);
    }
    
    private void openRightClickView()
    {
        EventManager.Instance.EventTrigger(Enums.EventType.ChooseEquip.ToString(), id);
        // var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, id));
        // RightClickShowInfo info = new RightClickShowInfo()
        // {
        //     PointPos = GetComponent<RectTransform>().anchoredPosition, ShowSkillDatas = itemEquip.GetSkillsData(), OnTriggerCallBack = itemEquip.OnSelectSkill
        // };
        // UIManager.Instance.ShowPanel<UIRightClickMenuView>(UIName.UIRightClickMenuView, info);
    }
}