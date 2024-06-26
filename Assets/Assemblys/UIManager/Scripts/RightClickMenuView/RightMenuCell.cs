using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RightMenuCell : DMonoBehaviour
{
    public void Init(SkillData skillInfo, UnityAction<SkillType> clickCb)
    {
        gameObject.SetActive(skillInfo.isUsable);
        GetComponentInChildren<Text>().text = skillInfo.skillName;
        GetComponentInChildren<Button>().onClick.AddListener(() => clickCb(skillInfo.SkillType));
        GetComponentInChildren<Button>().onClick.AddListener(() => UIManager.Instance.HidePanel(UIName.UIRightClickMenuView.ToString()));
    }
}