using System.Collections.Generic;
using DM.IFS;
using DM.Entity;
using UnityEngine;

namespace 测试飞机
{
    public class RTSInstructionMgr:RTSInstruction
    {
        void Awake()
        {
            List<EnumDescription> drop = new List<EnumDescription>();
            EnumDescription enum1 = new EnumDescription(1,"选项1");
            EnumDescription enum2 = new EnumDescription(2, "选项2");
            EnumDescription enum3 = new EnumDescription(3, "选项3");
            drop.Add(enum1);drop.Add(enum2);drop.Add(enum3);

            Instructor = new List<Instruction>() {
               new Instruction()
               {
                   Name="保障",
                   Event=1001,
                   PreStatus=0,
                   ExcuteMode=InsExcuteMode.Delay,
                   ExpendMode=InsExpendMode.Spend,
                   Properties=new List<DynamicProperty> () {
                       new InputFloatProperty("保障时间",30)
                   },
               },
               new Instruction()
               {
                   Name="飞行",
                   Event=1002,
                   PreStatus=1,
                   ExcuteMode=InsExcuteMode.Time,
                   ExpendMode=InsExpendMode.Auto,
                   TargetMode=InsTargetMode.Position,
                   Properties=new List<DynamicProperty> () {
                       new InputFloatUnitProperty("飞行高度",100,"M"),
                       new InputFloatUnitProperty("飞行速度",300,"KM/H")
                   },
                   AddDefault=true,
               },
               new Instruction()
               {
                   Name="盘旋",
                   Event=1003,
                   PreStatus=2,
                   ExcuteMode=InsExcuteMode.Immediately,
                   Properties=new List<DynamicProperty> () {
                       new InputFloatUnitProperty("盘旋速度",100,"KM/H"),
                       new InputFloatUnitProperty("盘旋半径",300,""),
                       new InputFloatUnitProperty("升降速率",5,"m/s"),
                       new InputFloatUnitProperty("离地高度",300,"m")
                   },
                   AddDefault=true,
               },
                new Instruction()
               {
                   Name="着水",
                   Event=1004,
                   PreStatus=2,
                   ExcuteMode=InsExcuteMode.Delay,
                   TargetMode=InsTargetMode.Position,
                   Properties=new List<DynamicProperty> () {
                       new InputIntUnitProperty("飞行高度",100,"M"),
                       new InputIntUnitProperty("下降速度",200,"KM/H"),
                       new InputIntUnitProperty("着水半径",300,"m"),
                   },
               },
                 new Instruction()
               {
                   Name="空投",
                   Event=1005,
                   PreStatus=2,
                   ExcuteMode=InsExcuteMode.Time,
                   TargetMode=InsTargetMode.Target,
                   Properties=new List<DynamicProperty> () {
                       new InputIntUnitProperty("空投物资",100,"T"),
                       new InputIntUnitProperty("飞行速度",300,"KM/H"),
                       new InputIntUnitProperty("离地高度",50,"m")
                   },
               },
                new Instruction()
                {
                    Name="测试",
                    Event=1006,
                    PreStatus=-1,
                    Properties=new List<DynamicProperty>() {
                        //new ButtonProperty("Button"),
                        //new ButtonInputStrProperty("ButtonInput","text","按钮",101),
                        new InputIntProperty("InputInt",11),
                        new InputIntUnitProperty("InputIntUnit",12,"个"),
                        new InputIntUnitLimitProperty("InputIntUnitLimit",12,"元",10,20),
                        new InputFloatProperty("InputFloat",11.1f),
                        new InputFloatUnitProperty("InputFloatUnit",12.1f,"个"),
                        new InputFloatUnitLimitProperty("InputFloatUnitLimit",12.111f,"元",5,25),
                        new DropDownProperty() { Name="这是一个Button"},
                        new InputStringProperty("InputString","idddddddd"),
                        new LabelFormatProperty("LabelFormat","sdfewrewrwer"),
                        new ToggleProperty("Toggle",true),
                        new DropDownProperty("dropDown",drop,1),
                        new DropDownSceneBObjectsProperty("DropDownSceneBObjects"),
                        new DropDownSceneSelectProperty("DropDownSceneSelect"),
                        new SliderProperty("Slider",15.3f,10.1f,20.2f),
                        new IntSliderProperty("IntSlider",5,1,10),
                        new OpenFileDialogProperty("OpenFileDialog","d:\\测试"),
                    },
                }
            };
        }

        public void InsHandler(Instruction ins)
        {
           
        }
    }
}
