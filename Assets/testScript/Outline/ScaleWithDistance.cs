using UnityEngine;

public class ScaleWithDistance : MonoBehaviour
{
    public Camera mainCamera; // 主相机
    public float maxDistance = 500f; // 最大距离
    private Vector3 initialScale; // 初始缩放比例

    public LayerMask initialLayer; // 初始的Layer
    public LayerMask clickedLayer; // 点击后的Layer

    private int initialLayerIndex; // 初始的Layer索引
    private int clickedLayerIndex; // 点击后的Layer索引

    private Vector3 dragStartPos; // 拖拽开始位置
    private bool isDragging = false; // 是否正在拖拽
    private bool ctrlPressed = false; // 是否按住了Ctrl键

    void Start()
    {
        // 如果没有设置主相机，默认使用主相机
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // 保存物体的初始缩放比例
        initialScale = transform.localScale;

        // 获取Layer索引
        initialLayerIndex = Mathf.RoundToInt(Mathf.Log(initialLayer.value, 2));
        clickedLayerIndex = Mathf.RoundToInt(Mathf.Log(clickedLayer.value, 2));

        // 设置物体及其子物体的初始Layer
        SetLayerRecursively(gameObject, initialLayerIndex);
    }

    void Update()
    {
        // 计算物体与主相机之间的距离
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);

        // 根据距离调整物体的缩放比例
        if (distance > maxDistance)
        {
            float scaleFactor = distance / maxDistance;
            transform.localScale = initialScale * scaleFactor;
        }
        else
        {
            transform.localScale = initialScale;
        }

        // 检测点击并改变Layer
        DetectClickAndChangeLayer();
        // 检测框选
        DetectBoxSelect();

        // 更新Ctrl键的状态
        ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
    }

    void DetectClickAndChangeLayer()
    {
        if (Input.GetMouseButtonDown(0)) // 检测鼠标左键点击
        {
            // 从鼠标位置发出射线
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 获取点击到的物体
                GameObject clickedObject = hit.collider.gameObject;

                // 只处理点击的对象是挂载此脚本的对象或其子对象
                if (IsDescendantOrSelf(clickedObject.transform, transform))
                {
                    if (!ctrlPressed)
                    {
                        // 如果没有按住Ctrl键，取消所有已选中的物体
                        SetLayerRecursively(gameObject, initialLayerIndex);
                    }

                    if (clickedObject.layer == initialLayerIndex)
                    {
                        // 切换到点击后的Layer
                        SetLayerRecursively(transform.gameObject, clickedLayerIndex);
                    }
                    else if (clickedObject.layer == clickedLayerIndex)
                    {
                        // 如果点击的是已经选中的物体，则取消选择
                        SetLayerRecursively(transform.gameObject, initialLayerIndex);
                    }
                }
                else if (!ctrlPressed)
                {
                    // 如果点击的不是当前对象或其子对象，则取消当前选择
                    SetLayerRecursively(gameObject, initialLayerIndex);
                }
            }
            else
            {
                // 未点击到任何对象时，将物体及其子物体的Layer改回初始Layer
                SetLayerRecursively(gameObject, initialLayerIndex);
            }
        }
    }

    void DetectBoxSelect()
    {
        if (Input.GetMouseButtonDown(0)) // 开始拖拽
        {
            isDragging = true;
            dragStartPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0)) // 结束拖拽
        {
            isDragging = false;
            SelectObjectsInDrag();
        }
    }

    void SelectObjectsInDrag()
    {
        // 获取拖拽结束位置
        Vector3 dragEndPos = Input.mousePosition;
        // 计算拖拽区域
        Rect selectRect = new Rect(
            Mathf.Min(dragStartPos.x, dragEndPos.x),
            Mathf.Min(dragStartPos.y, dragEndPos.y),
            Mathf.Abs(dragStartPos.x - dragEndPos.x),
            Mathf.Abs(dragEndPos.y - dragStartPos.y));

        // 检查物体的任意部分是否在选择框内
        if (IsAnyPartInScreenRect(transform, selectRect))
        {
            // 如果在选择框内，则改变Layer
            SetLayerRecursively(transform.gameObject, clickedLayerIndex);
        }
    }

    void OnGUI()
    {
        if (isDragging)
        {
            // 绘制拖拽框
            var rect = GetScreenRect(dragStartPos, Input.mousePosition);
            DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    // 递归设置物体及其子物体的Layer
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    // 获取屏幕矩形
    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    // 绘制屏幕矩形
    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    // 绘制屏幕矩形边框
    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    // 检查点击的对象是否是挂载此脚本对象的子对象
    bool IsDescendantOrSelf(Transform child, Transform parent)
    {
        if (child == parent)
            return true;

        Transform current = child.parent;
        while (current != null)
        {
            if (current == parent)
                return true;
            current = current.parent;
        }
        return false;
    }

    // 检查物体的任意部分是否在选择框内
    bool IsAnyPartInScreenRect(Transform objTransform, Rect selectRect)
    {
        foreach (Renderer renderer in objTransform.GetComponentsInChildren<Renderer>())
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(renderer.bounds.center);
            if (selectRect.Contains(screenPos, true))
            {
                return true;
            }
        }
        return false;
    }
}
