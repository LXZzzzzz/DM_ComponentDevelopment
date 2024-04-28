using DM.IFS;
using UnityEngine;

/// <summary>
/// 模型手动添加操作项脚本
/// 不能放在DM.IFS里：OperItem中的数据打包不能还原
/// </summary>
[AddComponentMenu("DMBuiltInScript/DMOperationItem")]
public class DM0perationItem : MonoBehaviour
{
    /// <summary>
    /// Id 
    /// </summary>
    public int Id;
    /// <summary>
    /// 状态
    /// </summary>
    public int Status;
    /// <summary>
    /// 类型  
    /// </summary>
    public int Type;
    /// <summary>
    /// UI偏移量
    /// </summary>
    public Vector3 UIOffset;
    /// <summary>
    /// 内容事件
    /// </summary>
    public OperItem[] OperItems; 
    /// <summary>
    /// 操作类型: Toggle,Slider,Button
    /// </summary>
    public OperType OperType;
    /// <summary>
    /// 操作时间
    /// </summary>
    public float OperTime;
    /// <summary>
    /// 冷却时间
    /// </summary>
    public float CDTime;
    /// <summary>
    /// 操作者动画
    /// </summary>
    public int OperatorAnim;
    /// <summary>
    /// 操作者初始位置
    /// </summary>
    public Transform OperatorBeginPos;
    /// <summary>
    /// 操作返回道具
    /// </summary>
    public string ReturnKnapsackId = "Null";
    /// <summary>
    /// 操作距离
    /// </summary>
    public float OperDistance;
    /// <summary>
    /// 前置条件
    /// </summary>
    public PreCondition[] PreCondition;
    /// <summary>
    /// 条件：组件状态
    /// </summary>
    public int ComponentStatus = -1;
    /// <summary>
    /// 条件：操作者模式
    /// </summary>
    public string OperatorMode = "All";
    /// <summary>
    /// 条件：操作者技能
    /// </summary>
    public string OperatorSkill = "All";
    /// <summary>
    /// 是否可用
    /// </summary>
    public bool Usable = true;
}

/// <summary>
/// 前置条件
/// </summary>
[System.Serializable]
public class PreCondition
{
    /// <summary>
    /// 前置条件：操作项Id 组件名_xml.id
    /// </summary>
    public string Id = "Null";
    /// <summary>
    /// 前置条件：操作项状态
    /// </summary>
    public int Status;
    /// <summary>
    /// 前置条件：操作执行次数
    /// </summary>
    public int OperCount;
}

/// <summary>
/// 内容事件
/// </summary>
[System.Serializable]
public class OperItem
{
    /// <summary>
    /// 当前状态(触发事件)
    /// </summary>
    public int Status;
    /// <summary>
    /// 提示内容
    /// </summary>
    public string ShowText = "Null";
    /// <summary>
    /// 新状态
    /// </summary>
    public int NewStatus;
}