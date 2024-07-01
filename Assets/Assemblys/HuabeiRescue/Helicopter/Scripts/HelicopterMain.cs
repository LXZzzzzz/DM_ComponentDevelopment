using System;
using System.Collections;
using System.Collections.Generic;
using DM.Entity;
using DM.IFS;
using UnityEngine;

public class HelicopterMain : ScriptManager
{
    public override void EditorModeInitialized()
    {
        base.EditorModeInitialized();
        sender.DebugMode = true;
    }

    private void Awake()
    {
        Properties = new DynamicProperty[]
        {
            new ToggleProperty("投水", true),
            new ToggleProperty("运送物资", true),
            new ToggleProperty("运送人员", true),
            new ToggleProperty("索降救援", true),
            new InputFloatUnitProperty("起飞前准备时间", 0.5f, "min"),
            new InputFloatUnitProperty("最大起飞重量", 10592, "kg"),
            new InputFloatUnitProperty("空机重量", 6980, "kg"),
            new InputFloatUnitProperty("最大航程", 800, "km"),
            new InputFloatUnitProperty("最大有效载荷", 3000, "kg"),
            new InputFloatUnitProperty("载油量", 3900, "kg"),
            new InputIntUnitProperty("最大载客量", 33, "人"),
            new InputFloatUnitProperty("最大载水量", 3000, "kg"),
            new InputFloatUnitProperty("最大时速", 273, "km/h"),
            new InputFloatUnitProperty("直升机巡航速度", 255, "km/h"),
            new InputFloatUnitProperty("直升机巡航高度", 300, "m"), //单位是不是错了
            new InputFloatUnitProperty("巡航油耗", 1101, "kg/h"),
            new InputFloatUnitProperty("爬升率", 66.6f, "km/h"),
            new InputFloatUnitProperty("爬升油耗", 2713.2f, "kg/h"),
            new InputFloatUnitProperty("悬停油耗", 880.8f, "kg/h"),
            new InputFloatUnitProperty("吊水重量", 3000, "kg"),
            new InputFloatUnitProperty("取水时间", 7, "min"),
            new InputFloatUnitProperty("洒水时间", 1, "min"),
            new InputFloatUnitProperty("加油时间", 20, "min"),
            new InputFloatUnitProperty("装载物资速率", 20, "kg/min"),
            new InputFloatUnitProperty("卸载物资速率", 20, "kg/min"),
            new InputFloatUnitProperty("空投物资速率", 20, "kg/min"),
            new InputFloatUnitProperty("落地装载人员速率", 20, "人/min"),
            new InputFloatUnitProperty("索降救人速率", 20, "人/min"),
            new InputFloatUnitProperty("安置伤员速率", 20, "人/min"),
            new InputFloatUnitProperty("补给时间", 20, "min"),
            new InputFloatUnitProperty("成年人平均重量", 70, "kg"),
            new InputFloatUnitProperty("单次洒水喷洒面积", 4200, "m²"),
            new InputFloatUnitProperty("直升机价格", 13000, "万元"),
            new InputFloatUnitProperty("最低每小时耗油量", 100, "kg/h")
        };
    }


    public override void RunModeInitialized(bool isRoomCreator, SceneInfo info)
    {
        base.RunModeInitialized(isRoomCreator, info);
        var logic = gameObject.transform.GetChild(0).gameObject.AddComponent<HelicopterController>();
        //进入运行模式后，将一些基础属性通过控制器传给飞机，
        logic.EquipIcon = info.PicBObjects[BObjectId];
        logic.isTS = (Properties[0] as ToggleProperty).Value;
        logic.isYSWZ = (Properties[1] as ToggleProperty).Value;
        logic.isYSRY = (Properties[2] as ToggleProperty).Value;
        logic.isSJJY = (Properties[3] as ToggleProperty).Value;
        logic.AttributeInfos = new List<string>();
        logic.myAttributeInfo = new HelicopterInfo();
        var fields = logic.myAttributeInfo.GetType().GetFields();
        for (int i = 4; i < 34; i++)
        {
            if (fields[i - 4].FieldType == typeof(Int32))
            {
                logic.AttributeInfos.Add((Properties[i] as InputIntUnitProperty).Value.ToString());
                fields[i - 4].SetValue(logic.myAttributeInfo, (Properties[i] as InputIntUnitProperty).Value);
            }
            else
            {
                logic.AttributeInfos.Add((Properties[i] as InputFloatUnitProperty).Value.ToString());
                fields[i - 4].SetValue(logic.myAttributeInfo, (Properties[i] as InputFloatUnitProperty).Value);
            }
        }

        logic.gameObject.SetActive(false);
    }
}