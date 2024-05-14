using System.Collections.Generic;
using DM.IFS;
using DM.Entity;
using UnityEngine;

namespace 测试飞机
{
    public class RTSInformationMgr:RTSInformation
    {
        void Awake()
        {
            listDynamicPro1 = new List<DynamicProperty>(){
                new InputFloatUnitProperty("起飞保障时间",30,"min"),
                new InputFloatUnitProperty("飞行速度",0,"km/h"),
                new InputFloatUnitProperty("离地高度",0,"m"),
                new LabelProperty() {Name="机载救生艇 1/1",Group="Group1"},
                new LabelProperty() {Name="空投物资 5/5",Group="Group1"},
                new LabelProperty() {Name="救生机器人 1/1",Group="Group2"},
               new LabelProperty() {Name="机载人员 0/30",Group="Group2"},
            };

            listDynamicPro2 = new List<DynamicProperty>(){
                new SliderProperty("地面机载重量",2000,0,62000),
                new SliderProperty("水面机载重量",2000,0,54000),
                new SliderProperty("飞行机载重量",100,0,7620),
                new SliderProperty("行驶航程",400,0,50000),
                new SliderProperty("机载燃油",2260,0,2260),
            };

            List<DynamicProperty> guangdian = new List<DynamicProperty>() {
                new ToggleProperty("光电探测",false),
                new InputIntUnitProperty("光电视角场",60,"°"),
                new InputIntUnitProperty("红外视角场",60,"°"),
                new InputIntProperty("光电转塔水平范围",-90),
                 new InputIntProperty("光电转塔俯仰范围",-110),
            };
            List<DynamicProperty> wuxiandian = new List<DynamicProperty>() {
                new ToggleProperty("无线电探测",false),
                new InputIntUnitProperty("信号接收",85,"海里"),
                new InputIntUnitProperty("测距侧向距离",54,"海里"),
            };
            listDynamicGroup1 = new List<DynamicGroup>() {
                new DynamicGroup() { Name="光电探测",Elements=guangdian},
                new DynamicGroup() { Name="无线电探测",Elements=wuxiandian}
            };
        }
    }
}
