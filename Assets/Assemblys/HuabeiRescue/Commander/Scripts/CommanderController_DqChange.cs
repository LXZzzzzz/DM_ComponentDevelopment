using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using DM.IFS;
using Enums;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;
using UnityEngine.Events;
using EventType = Enums.EventType;

public partial class CommanderController
{
    private object currentChooseGo;
    private string[] tianqiInfo, fengliInfo;

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

        if (MyDataInfo.sceneAllEquips != null && MyDataInfo.sceneAllEquips.Count > 0)
            OnCameraContral(1, MyDataInfo.sceneAllEquips[0].transform);
        //地形实际是 0=> 晴天，1=> 多云，2=> 阴天，3=> 雨天，4=> 雪天，5=> 浓雾，6=> 薄雾
        tianqiInfo = new[] { "晴天", "多云", "阴", "雾", "雷阵雨", "小雨", "中雨", "大雨", "暴雨" };
        fengliInfo = new[] { "无方向微风", "风力1-2级", "风力3-4级", "风力5-6级", "风力7-8级", "狂风9-10级", "狂风10级以上" };
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
            OnCameraContral(1, itemEquip.transform);
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView", itemEquip);
        }
        else if (itemZiyuan != null)
        {
            itemZiyuan.isChooseMe = true;
            currentChooseGo = itemZiyuan;
            OnCameraContral(1, itemZiyuan.transform);
            EventManager.Instance.EventTrigger<string, object>(EventType.ShowUI.ToString(), "AttributeView",
                itemZiyuan);
        }
    }

    public void OnAskTaskExecute()
    {
        if (MyDataInfo.MyLevel == 1)
        {
            //弹窗询问总指挥是否同意任务执行
            EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), "申请任务执行",
                () => { OnSendSkillInfo((int)MessageID.SendAgreeTaskExecute, ""); });
        }
    }

    public void OnSetZyInfo(string info)
    {
        var strs = info.Split('_');
        var itemZy = sceneAllzy.Find(x => string.Equals(x.BobjectId, strs[0]));
        itemZy.ziYuanName = strs[1];
        itemZy.latAndLon = new Vector2(float.Parse(strs[2]), float.Parse(strs[3]));
        var dataPos = LongLat2Pos(itemZy.latAndLon);
        var posY = GetCurrentGroundHeight(dataPos);
        itemZy.transform.position = new Vector3(dataPos.x, posY, dataPos.z);
    }

    public void OnSetEquipData(string info)
    {
        var equipInfos = info.Split(':');

        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            for (int j = 0; j < equipInfos.Length; j++)
            {
                var adatas = equipInfos[j].Split('_');
                if (string.Equals(MyDataInfo.sceneAllEquips[i].BObjectId, adatas[0]))
                {
                    MyDataInfo.sceneAllEquips[i].textInfo = equipInfos[j];
                    break;
                }
            }
        }
    }

    public void OnSetEquipShow(string info)
    {
        var equips = info.Split(':');
        List<string> infos = new List<string>();
        for (int i = 0; i < equips.Length; i++)
        {
            if (string.IsNullOrEmpty(equips[i])) continue;
            //记录场景中要显示的直升机id
            infos.Add(equips[i]);
        }

        for (int i = 0; i < MyDataInfo.sceneAllEquips.Count; i++)
        {
            MyDataInfo.sceneAllEquips[i].gameObject.SetActive(infos.Contains(MyDataInfo.sceneAllEquips[i].BObjectId));
        }
    }

    public void OnSetPersonInfo(string data)
    {
        //把机组和保障人员数据拆开
        var cellsInfo = data.Split(';');
        MyDataInfo.BeUsedJizus = new List<string>();
        MyDataInfo.BeUsedBaozhangs = new List<string>();

        //所有机组信息
        var jizuInfos = cellsInfo[0].Split(':');
        for (int i = 0; i < jizuInfos.Length; i++)
        {
            if (string.IsNullOrEmpty(jizuInfos[i])) continue;
            var ajz = jizuInfos[i].Split('_');
            if (int.Parse(ajz[1]) == 1) MyDataInfo.BeUsedJizus.Add(ajz[0]);
        }

        //所有保障组信息
        var baozhangInfos = cellsInfo[1].Split(':');
        for (int i = 0; i < baozhangInfos.Length; i++)
        {
            if (string.IsNullOrEmpty(baozhangInfos[i])) continue;
            var abz = baozhangInfos[i].Split('_');
            if (int.Parse(abz[1]) == 1) MyDataInfo.BeUsedBaozhangs.Add(abz[0]);
        }
    }

    public void OnAskForReturn(string info)
    {
        Debug.LogError("收到的请求数据" + info);
        var data = info.Split('_');
        string equipName = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, data[0])).name;
        Receive_TextMsgRecord($"{equipName}机长申请返航");

        if (MyDataInfo.MyLevel != 2) return;
        switch (int.Parse(data[1]))
        {
            case 1:
                EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(),
                    $"{equipName}申请返修，是否同意",
                    () => { OnSendSkillInfo((int)MessageID.SendAgreeReturn, info); });
                break;
            case 2:
                EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(),
                    $"{equipName}申请返航，是否同意",
                    () => { OnSendSkillInfo((int)MessageID.SendAgreeReturn, info); });
                break;
        }
    }

    public void OnChangeTianQi(string param)
    {
        string[] infos = param.Split('_');
        int tqInfo = int.Parse(infos[0]);
        int flInfo = int.Parse(infos[1]);
        if (MyDataInfo.MyLevel == 3)
            EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(),
                $"当前天气：{tianqiInfo[tqInfo]}，{fengliInfo[flInfo]}");

        Receive_TextMsgRecord($"特情信息：天气发生变化 {tianqiInfo[tqInfo]}，{fengliInfo[flInfo]}");

        GetWeathersByIndex(tqInfo);

        EventManager.Instance.EventTrigger(EventType.TransferTianqiData.ToString(), $"{tianqiInfo[tqInfo]}，{fengliInfo[flInfo]}");
    }

    public void OnReceiveRwghwc(string param)
    {
        string equipName = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, param)).name;
        Receive_TextMsgRecord($"{equipName}任务规划完成");
        if (MyDataInfo.MyLevel != 2) return;
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), $"{equipName}任务规划完成");
        if (!MyDataInfo.TaskPlanningCompletedPersons.Contains(param))
            MyDataInfo.TaskPlanningCompletedPersons.Add(param);
    }

    public void OnShowRwghData(string param)
    {
        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 2);
        EventManager.Instance.EventTrigger(EventType.LoadPathPlanningData.ToString(), param);
        StartCoroutine(WaitAndPrint());
    }

    private IEnumerator WaitAndPrint()
    {
        // 等待1秒
        yield return new WaitForSeconds(1f);


        EventManager.Instance.EventTrigger(EventType.SwitchMapModel.ToString(), 3);
    }

    public void OnDiscoverNewDisaster(string param)
    {
        string equipName = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, param)).name;
        Receive_TextMsgRecord($"{equipName}机长上报新灾情");

        if (MyDataInfo.MyLevel == 2)
        {
            EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), $"机长发现新灾情，是否处理",
                () => { OnSendSkillInfo((int)MessageID.SendAgreeDiscoverNewDisaster, ""); });
        }
    }

    /// <summary>
    /// 通过索引切换天气 index最大为 6
    /// </summary>
    /// <param name="index">0=> 晴天，1=> 多云，2=> 阴天，3=> 雨天，4=> 雪天，5=> 浓雾，6=> 薄雾 </param>
    public void GetWeathersByIndex(int index)
    {
        int newindex = 0;
        GameObject expanse = GameObject.Find("Expanse Sky");
        if (expanse == null) return;
        Transform weather = expanse.transform.Find("Weathers");
        if (weather != null)
        {
            for (int i = 0; i < weather.childCount; i++)
            {
                weather.GetChild(i).gameObject.SetActive(false);
            }

            switch (index)
            {
                case 0:
                case 1:
                case 2:
                    newindex = index;
                    break;
                case 3:
                    newindex = 5;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    newindex = 3;
                    break;
            }

            weather.GetChild(newindex).gameObject.SetActive(true);
        }
    }

    public List<string> Getweathers()
    {
        List<string> listWeather = new List<string>();
        GameObject expanse = GameObject.Find("Expanse Sky");
        if (expanse == null) return listWeather;
        Transform weather = expanse.transform.Find("Weathers");
        if (weather != null)
        {
            for (int i = 0; i < weather.childCount; i++)
            {
                listWeather.Add(weather.GetChild(i).name);
            }
        }

        return listWeather;
    }

    public void OnChangeEquipInfo(string data)
    {
        var infos = data.Split('_');
        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, infos[0]));
        (itemEquip as IDqChangePart).SetOilAndLoad(float.Parse(infos[1]), float.Parse(infos[2]));
    }

    public void OnChangeZiyuanInfo(string data)
    {
        var infos = data.Split('_');
        ZyVariableDataBase itemData = null;
        var item = sceneAllzy.Find(x => string.Equals(x.BobjectId, infos[0]));
        switch (item.ZiYuanType)
        {
            case ZiYuanType.Supply:
                itemData = new SupplyVariableData() { oilNum = float.Parse(infos[1]) };
                break;
            case ZiYuanType.GoodsPoint:
                itemData = new GoodsPointVariableData() { goodsNum = float.Parse(infos[1]) };
                break;
        }

        item.SetVariableData(itemData);
    }
}