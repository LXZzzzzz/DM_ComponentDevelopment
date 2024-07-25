using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UnityEngine;

public class AirPortPointLogic : ZiYuanBase, IAirPort
{
    private List<Vector3> prestorePoints;
    public List<string> allDockingAircraft;

    public void Init(int widthSpacing, int heightSpacing, int widthCount, int heightCount, string id)
    {
        base.Init(id, 50);
        ZiYuanType = ZiYuanType.Airport;
        allDockingAircraft = new List<string>();
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);

        int startX = 0;
        int startY = 0;
        int horizontalSpacing = widthSpacing;
        int verticalSpacing = heightSpacing;
        int width = widthCount; // 矩形的横向扩散点数（不包括起始点）
        int height = heightCount; // 矩形的纵向扩散点数（不包括起始点）

        RectangularSpread rectangularSpread = new RectangularSpread(startX, startY, horizontalSpacing, verticalSpacing, width, height);
        List<(int x, int y)> points = rectangularSpread.GeneratePoints();
        prestorePoints = new List<Vector3>();
        foreach (var point in points)
        {
            if (point.Equals((0, 0))) continue;
            prestorePoints.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z) + transform.right * point.x + transform.forward * point.y);
            // Debug.Log($"({point.x}, {point.y})");
            // GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // go.transform.position = new Vector3(point.x, 0, point.y);
        }
    }

    public bool checkComeIn()
    {
        return prestorePoints.Count > allDockingAircraft.Count;
    }

    public void comeIn(string equipId)
    {
        allDockingAircraft.Add(equipId);
        MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId)).isDockingAtTheAirport = true;
        for (int i = 0; i < allDockingAircraft.Count; i++)
        {
            GameObject itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, allDockingAircraft[i])).gameObject;
            itemEquip.transform.rotation = transform.rotation;
            itemEquip.transform.position = prestorePoints[i];
        }
    }

    public List<string> GetAllEquips()
    {
        return allDockingAircraft;
    }

    public void goOut(string equipId)
    {
        //起飞指定飞机，并把他从机场移除出去
        allDockingAircraft.Remove(equipId);
        var itemEquip = MyDataInfo.sceneAllEquips.Find(x => string.Equals(x.BObjectId, equipId));
        itemEquip.isDockingAtTheAirport = false;
        itemEquip.transform.position = new Vector3(transform.position.x, itemEquip.transform.position.y, transform.position.z);
    }

    private void desAir(string idid)
    {
        if (allDockingAircraft.Contains(idid))
            allDockingAircraft.Remove(idid);
    }

    public override void OnStart()
    {
    }

    protected override void OnReset()
    {
        allDockingAircraft.Clear();
        EventManager.Instance.AddEventListener<string>(Enums.EventType.DestoryEquip.ToString(), desAir);
    }
}


public class RectangularSpread
{
    private int startX;
    private int startY;
    private int horizontalSpacing;
    private int verticalSpacing;
    private int width; // 矩形的宽度
    private int height; // 矩形的高度

    public RectangularSpread(int startX, int startY, int horizontalSpacing, int verticalSpacing, int width, int height)
    {
        this.startX = startX;
        this.startY = startY;
        this.horizontalSpacing = horizontalSpacing;
        this.verticalSpacing = verticalSpacing;
        this.width = width;
        this.height = height;
    }

    public List<(int x, int y)> GeneratePoints()
    {
        List<(int x, int y)> points = new List<(int x, int)>();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // 计算点的坐标并添加到列表中
                int x = startX + j * horizontalSpacing;
                int y = startY + i * verticalSpacing;
                points.Add((x, y));
            }
        }

        return points;
    }
}