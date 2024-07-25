using System;
using System.Collections.Generic;
using UnityEngine;

public class TestAirPort : MonoBehaviour
{
    void Start()
    {
        int startX = 0;
        int startY = 0;
        int horizontalSpacing = 10;
        int verticalSpacing = 10;
        int width = 4; // 矩形的横向扩散点数（不包括起始点）
        int height = 3; // 矩形的纵向扩散点数（不包括起始点）

        RectangularSpread rectangularSpread = new RectangularSpread(startX, startY, horizontalSpacing, verticalSpacing, width, height);
        List<(int x, int y)> points = rectangularSpread.GeneratePoints();

        foreach (var point in points)
        {
            Console.WriteLine($"({point.x}, {point.y})");
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.position = new Vector3(transform.position.x, 0, transform.position.z) + transform.right * point.x + transform.forward * point.y;
        }
    }
}

public class RectangularSpreadaa
{
    private int startX;
    private int startY;
    private int horizontalSpacing;
    private int verticalSpacing;
    private int width; // 矩形的宽度
    private int height; // 矩形的高度

    public RectangularSpreadaa(int startX, int startY, int horizontalSpacing, int verticalSpacing, int width, int height)
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