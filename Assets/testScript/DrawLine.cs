using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private string shaderName = "HDRP/Lit";
    void Start()
    {
        // 获取LineRenderer组件
        LineRenderer lineRenderer = GetComponent<LineRenderer>();

        // 设置线段材质和纹理
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        // lineRenderer.texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        // lineRenderer.texture.Apply();

        // 设置线宽
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        // 设置线段颜色
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.blue;

        // 定义线段的两个顶点
        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(0f, 0f, 0f); // 起点
        positions[1] = new Vector3(10f, 0f, 0f); // 终点

        // 设置线段顶点
        lineRenderer.SetPositions(positions);
    }
}