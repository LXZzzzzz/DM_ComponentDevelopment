using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;

public partial class CommanderController
{
    private object currentChooseGo;

    public void Init()
    {
        for (int i = 0; i < allBObjects.Length; i++)
        {
            var tagItem = allBObjects[i].BObject.Info.Tags.Find(x => x.Id == 1010);
            if (tagItem != null && tagItem.SubTags.Find(y => y.Id == 7) != null)
            {
                var itemObj = allBObjects[i].transform.GetChild(0).GetComponent<EquipBase>();

                //1.把飞机记录到静态变量,把飞机放到指定节点下
                itemObj.transform.parent = MyDataInfo.SceneGoParent;
                itemObj.gameObject.name = allBObjects[i].BObject.Info.Name;
                itemObj.Init(itemObj, sceneAllzy);
                itemObj.gameObject.SetActive(true);
                EventManager.Instance.EventTrigger(EventType.CreatEquipCorrespondingIcon.ToString(), itemObj);
                MyDataInfo.sceneAllEquips.Add(itemObj);
                //2.将场景中的所有飞机都放到自己所属机场
                string airportId = (itemObj as IDqChangePart)?.GetStopAtAirPort();
                IAirPort myAirPort = sceneAllzy.Find(x => string.Equals(x.BobjectId, airportId)) as IAirPort;
                if (myAirPort != null) myAirPort.comeIn(itemObj.BObjectId);
                else itemObj.isDockingAtTheAirport = false;
            }
        }
    }

    private void OnChooseAGo(string id)
    {
        if (currentChooseGo != null)
        {
            if (currentChooseGo is EquipBase) ((EquipBase)currentChooseGo).isChooseMe = false;
            if (currentChooseGo is ZiYuanBase) ((ZiYuanBase)currentChooseGo).isChooseMe = false;
        }

        if (string.IsNullOrEmpty(id))
        {
            currentChooseGo = null;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", null);
            return;
        }

        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(id, x.BObjectId));
        var itemZiyuan = MyDataInfo.sceneAllZiYuan.Find(x => string.Equals(id, x.BobjectId));
        if (itemEquip != null)
        {
            itemEquip.isChooseMe = true;
            currentChooseGo = itemEquip;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", itemEquip);
        }
        else if (itemZiyuan != null)
        {
            itemZiyuan.isChooseMe = true;
            currentChooseGo = itemZiyuan;
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", itemZiyuan);
        }
    }

    public void OnAskTaskExecute()
    {
        if (MyDataInfo.MyLevel == 1)
        {
            //弹窗询问总指挥是否同意任务执行
            EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), "申请任务执行", () => { OnSendSkillInfo((int)MessageID.SendAgreeTaskExecute, ""); });
        }
    }

    public void OnChangeTianQi(int tqInfo)
    {
        //收到天气信息
        if (MyDataInfo.MyLevel == 2 && tqInfo > 0)
        {
            EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), "当前天气下雨，是否全部返航", () => { OnSendSkillInfo((int)MessageID.SendTurnBack, ""); });
        }
    }
}