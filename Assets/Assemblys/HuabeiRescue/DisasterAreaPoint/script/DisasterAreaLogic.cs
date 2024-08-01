using ToolsLibrary.EquipPart;
using UnityEngine;

public class DisasterAreaLogic : ZiYuanBase, IDisasterArea
{
    private int allPersonNum;

    private int currentRemainingPersonnel;

    //1:轻伤员；2：重伤员
    private int woundedPersonnelType;

    public void Init(string id, int personNum, int type)
    {
        base.Init(id, 50);
        ZiYuanType = ZiYuanType.DisasterArea;
        allPersonNum = personNum;
        currentRemainingPersonnel = allPersonNum;
        woundedPersonnelType = type;
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
        currentRemainingPersonnel = allPersonNum;
    }

    public void airdropGoods(float time, float squareMeasure)
    {
        //灾区接收物资，目前不用，物资给救助站
    }

    public int rescuePerson(int maxrescuePersonNum)
    {
        if (currentRemainingPersonnel > maxrescuePersonNum)
        {
            currentRemainingPersonnel -= maxrescuePersonNum;
            Debug.LogError($"灾区营救了{maxrescuePersonNum}人还剩{currentRemainingPersonnel}人");
            return maxrescuePersonNum;
        }
        else
        {
            int rpn = currentRemainingPersonnel;
            currentRemainingPersonnel = 0;
            Debug.LogError($"灾区营救了{rpn}人");
            return rpn;
        }
    }

    public bool getTaskProgress(out int currentNum, out int maxNum)
    {
        currentNum = allPersonNum - currentRemainingPersonnel;
        maxNum = allPersonNum;
        return currentRemainingPersonnel <= 0;
    }
}