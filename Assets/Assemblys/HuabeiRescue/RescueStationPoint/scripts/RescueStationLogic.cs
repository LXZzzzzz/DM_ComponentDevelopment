using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class RescueStationLogic : ZiYuanBase, IRescueStation
{
    private float firstGoodsTime;
    private float totalWeight;
    private int totalPerson;

    private float needGoodsWeight;
    private int maxPersonNum;

    public void Init(string id, float goodsWeight, int personNum)
    {
        base.Init(id, 50);
        ZiYuanType = ZiYuanType.RescueStation;
        firstGoodsTime = 0;
        totalWeight = 0;
        totalPerson = 0;
        needGoodsWeight = goodsWeight;
        maxPersonNum = personNum;
    }

    protected override void OnReset()
    {
        firstGoodsTime = 0;
        totalWeight = 0;
        totalPerson = 0;
    }

    public void goodsPour(float weight)
    {
        if (firstGoodsTime < 0)
        {
            firstGoodsTime = MyDataInfo.gameStartTime;
        }

        totalWeight += weight;
        Debug.LogError($"安置点被投放了{totalWeight}物资");
    }

    public void placementOfPersonnel(int personNum)
    {
        totalPerson += personNum;
        Debug.LogError($"安置点被安置了{totalPerson}人");
    }

    public void getResData(out float firstTime, out float totalWeight, out int totalPerson)
    {
        firstTime = this.firstGoodsTime;
        totalPerson = this.totalPerson;
        totalWeight = this.totalWeight;
    }

    public bool getTaskProgress()
    {
        return totalPerson >= maxPersonNum && totalWeight >= needGoodsWeight;
    }
}