using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using EventType = Enums.EventType;

public partial class CommanderController
{
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
                string airportId = (itemObj as DqChangePart).GetStopAtAirPort();
                IAirPort myAirPort = sceneAllzy.Find(x => string.Equals(x.BobjectId, airportId)) as IAirPort;
                if (myAirPort != null) myAirPort.comeIn(itemObj.BObjectId);
                else itemObj.isDockingAtTheAirport = false;
            }
        }
    }
}