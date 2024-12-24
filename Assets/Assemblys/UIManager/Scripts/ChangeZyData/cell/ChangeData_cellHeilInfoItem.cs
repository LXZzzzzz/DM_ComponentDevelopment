using UnityEngine;
using UnityEngine.UI;

public class ChangeData_cellHeilInfoItem : DMonoBehaviour
{
    /// <summary>
    /// 飞机下拉菜单
    /// </summary>
    public Dropdown Dropdown_heli;

    /// <summary>
    /// 燃油滑条
    /// </summary>
    public Slider Slider_fuel;

    /// <summary>
    /// 燃油最小，最大值
    /// </summary>
    public Text Text_fuelMin, Text_fuelMax;
    
    
    /// <summary>
    /// 载重滑条
    /// </summary>
    public Slider Slider_load;

    /// <summary>
    /// 载重最小，最大值
    /// </summary>
    public Text Text_loadMin, Text_loadMax;

    /// <summary>
    /// 维护周期
    /// </summary>
    public InputField InputField_cycle;
}
