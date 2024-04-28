using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("DMBuiltInScript/DMLayout")]
 public class DMLayout:MonoBehaviour
{
    public int Size = 1;        //容器自身尺寸
    public int ContainSize = 10;  //容器可容纳尺寸
    //public SetItem[] 自身类型 = new SetItem[]      //0-1000 地形类型(所属互斥，放置多选)   1001-1024 容器类型(所属互斥，放置互斥)  1025-99999 组件类型(所属多选，放置多选)
    //{
    //    new SetItem(1001,"非容器",false),
    //    new SetItem(1002,"双层容器",false)
    //};
    //public SetItem[] 放置类型 = new SetItem[]  //地形类型+容器类型
    //{
    //    new SetItem(1001,"水面类型",false),
    //    new SetItem(1002,"地面类型",false),
    //    new SetItem(1003,"Container",false),
    //    new SetItem(1004,"Non-Container",false)
    //};
}