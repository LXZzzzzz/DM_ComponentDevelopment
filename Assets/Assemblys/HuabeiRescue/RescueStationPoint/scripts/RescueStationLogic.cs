using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

//安置点和医院共用，逻辑一样
public class RescueStationLogic : ZiYuanBase, IRescueStation
{
    private float firstGoodsTime;
    private float totalWeight;
    private int totalPerson;

    private float perPersonNeedGoodsWeight;
    private int maxPersonNum;

    public void Init(string id, int zyType, float goodsWeight, int personNum, string colorCode)
    {
        base.Init(id, 50, colorCode);
        ZiYuanType = (ZiYuanType)zyType;
        firstGoodsTime = 0;
        totalWeight = 0;
        totalPerson = 0;
        perPersonNeedGoodsWeight = goodsWeight;
        maxPersonNum = personNum;
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
        firstGoodsTime = 0;
        totalWeight = 0;
        totalPerson = 0;
    }

    public void goodsPour(float weight)
    {
        if (firstGoodsTime < 1)
        {
            firstGoodsTime = MyDataInfo.gameStartTime;
        }

        totalWeight += weight;
        Debug.LogError($"安置点被投放了{totalWeight}物资");
    }

    public int placementOfPersonnel(int personNum)
    {
        int itemPersonNum = maxPersonNum - totalPerson;
        if (itemPersonNum > personNum)
        {
            //证明当前安置点还能容纳下所有伤员
            totalPerson += personNum;
            Debug.LogError($"安置点被安置了{totalPerson}人");
            return personNum;
        }
        else
        {
            //安置点不足以安置下所有伤员
            totalPerson = maxPersonNum;
            Debug.LogError($"安置点被安置了{totalPerson}人");
            return itemPersonNum;
        }
    }

    public void getResData(out float firstTime, out float totalWeight, out int totalPerson)
    {
        firstTime = this.firstGoodsTime;
        totalPerson = this.totalPerson;
        totalWeight = this.totalWeight;
    }

    public bool getTaskProgress(out int currentPersonNum, out int maxPersonNum, out float currentGoodsNum, out float maxGoodsNum)
    {
        currentPersonNum = totalPerson;
        maxPersonNum = this.maxPersonNum;
        currentGoodsNum = totalWeight;
        maxGoodsNum = perPersonNeedGoodsWeight * totalPerson;

        return totalPerson >= this.maxPersonNum && totalWeight >= perPersonNeedGoodsWeight * this.maxPersonNum;
    }
}