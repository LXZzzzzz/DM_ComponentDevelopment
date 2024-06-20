using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UiManager.IconShowPart;
using UnityEngine;
using Vectrosity;

public class TestLogic : MonoBehaviour
{
    private List<EquipBase> allEquip;
    private Dictionary<string, string> testDic;
    public Transform target;

    public VectorLine vl;
    public Canvas can;

    public ZiYuanBase obj;
    void Start()
    {
        allEquip = new List<EquipBase>();
        var eqs = GetComponentsInChildren<EquipBase>();
        for (int i = 0; i < eqs.Length; i++)
        {
            eqs[i].BObjectId = i.ToString();
            eqs[i].Init();
            allEquip.Add(eqs[i]);
        }

        MyDataInfo.sceneAllEquips = allEquip;

        testDic = new Dictionary<string, string>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventManager.Instance.EventTrigger<string, object>(Enums.EventType.ShowUI.ToString(), "IconShow", null);
            UIManager.Instance.ShowPanel<UIMap>(UIName.UIMap, new Vector2(18000,18000));
            UIManager.Instance.ShowPanel<UITopMenuView>(UIName.UITopMenuView, 1);
            UIManager.Instance.ShowPanel<UICommanderView>(UIName.UICommanderView, 1);
            EventManager.Instance.EventTrigger<string, object>(Enums.EventType.ShowUI.ToString(), "AttributeView", null);
            initLine();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 vec = new Vector3(1, 2, 3);
            Debug.Log((Vector2)vec);
        }
    }

    private List<Vector2> routePoints;
    private void initLine()
    {
        routePoints = new List<Vector2>();
        routePoints.Add(Vector2.zero);
        routePoints.Add(Vector2.one*10);
        vl = new VectorLine("Line", routePoints, 3, LineType.Continuous);
#if UNITY_EDITOR
        vl.SetCanvas(can);
#else
        vl.SetCanvas(UIManager.Instance.CurrentCanvans);
#endif
        vl.rectTransform.SetParent(can.transform);
        vl.rectTransform.localPosition = Vector3.zero;
        vl.rectTransform.localScale = Vector3.one;
        vl.active = true;
        vl.color = Color.cyan;
        vl.Draw();
        vl.active = true;
    }
}