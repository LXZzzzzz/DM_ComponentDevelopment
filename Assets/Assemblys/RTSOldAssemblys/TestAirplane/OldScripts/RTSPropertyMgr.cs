using System.Collections.Generic;
using DM.IFS;
using DM.Entity;
using UnityEngine;

namespace 测试飞机
{
    public class RTSPropertyMgr: RTSProperty
    {
        void Awake()
        {
            //Functions只支持LabelProperty类型的属性
            Functions = new List<DynamicProperty>() {
                new LabelProperty("搜"),
                new LabelProperty("救"),
                new LabelProperty("送"),
                new LabelProperty("测"),
            };

            Properties = new List<DynamicProperty>() {
                new InputFloatUnitProperty("起飞保障时间",30,"min"),
                new InputFloatUnitProperty() {Name="飞行最大实用航程",Value=4000,Unit="km",Group="Group1"},
                new InputFloatUnitProperty() {Name="水面最大实用航程",Value=2160,Unit="海里",Group="Group1"},
                new InputFloatUnitProperty("最大升限",7620,"m"),
                new InputFloatUnitProperty("最大抗浪高",2,"m"),
                new InputFloatUnitProperty("升降率",4.5f,"m/s"),
                new InputFloatUnitProperty() {Name="巡航速度",Value=300,Unit="km/h",Group="Group2"},
                new InputFloatUnitProperty() {Name="备份燃油时耗",Value=45,Unit="min",Group="Group2"},
            };

            List<DynamicProperty> guangdian = new List<DynamicProperty>() {
                new ToggleProperty("光电探测",false),
                new InputIntUnitProperty("广电视角场",60,"°"),
                new InputIntUnitProperty("红外视角场",60,"°"),
                new InputIntProperty("广电转塔水平范围",-90),
                 new InputIntProperty("广电转塔俯仰范围",-110),
            };
            List<DynamicProperty> wuxiandian = new List<DynamicProperty>() {
                new ToggleProperty("无线电探测",false),
                new InputIntUnitProperty("信号接收",85,"海里"),
                new InputIntUnitProperty("测距侧向距离",54,"海里"),
            };
            List<DynamicProperty> guangdian2 = new List<DynamicProperty>() {
                new ToggleProperty("光电探测",false),
                new InputIntUnitProperty("广电视角场",60,"°"),
                new InputIntUnitProperty("红外视角场",60,"°"),
                new InputIntProperty("广电转塔水平范围",-90),
                 new InputIntProperty("广电转塔俯仰范围",-110),
                 new ToggleProperty("测试Toggle",true),
            };
            List<DynamicProperty> wuxiandian2 = new List<DynamicProperty>() {
                new ToggleProperty("无线电探测",false),
                new InputIntUnitProperty("信号接收",85,"海里"),
                new InputIntUnitProperty("测距侧向距离",54,"海里"),
            };
            DynamicGroup group1 = new DynamicGroup("光电探测",guangdian);
            DynamicGroup group2 = new DynamicGroup("无线电探测",wuxiandian);
            Devices.Add(group1);Devices.Add(group2);
            

            Others = new List<DynamicProperty>() {
                new IntSliderProperty("机载承重",1700,0,2000),
                new InputIntUnitProperty() { Name="可载人员",Value=30,Unit="人",Group="group1"},
                new InputIntUnitProperty() { Name="机载救生艇",Value=1,Unit="艘",Group="group1"},
                new InputIntProperty() { Name="空投物资",Value=4,Group="group1"},
                new InputIntUnitProperty() { Name="机载然后",Value=2260,Unit="kg",Group="group2"},
                new InputIntUnitProperty() { Name="水面救生艇",Value=4,Unit="艘",Group="group2"},
            };
        }
    }
}
