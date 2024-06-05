using ToolsLibrary.EquipPart;
using UnityEngine;

public class ZiYuan_WaterPoinnt : ZiYuanBase
{
    private void Start()
    {
        sender.LogError("取水点代码挂载成功");
        ZiYuanType = ZiYuanType.Waters;
    }
}
