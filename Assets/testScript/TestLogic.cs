using System;
using System.Collections;
using System.Collections.Generic;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    private List<EquipBase> allEquip;
    private Dictionary<string, string> testDic;
    public Transform target;
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

        EventManager.Instance.AddEventListener<bool>(Enums.EventType.CameraSwitch.ToString(), testaaa);
        testDic = new Dictionary<string, string>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UIManager.Instance.ShowPanel<UIMap>(UIName.UIMap, new Vector2(18000,18000));
            UIManager.Instance.ShowPanel<UITopMenuView>(UIName.UITopMenuView, 1);
            UIManager.Instance.ShowPanel<UICommanderView>(UIName.UICommanderView, 1);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            
            Camera.main.transform.position = target.position + target.up * 20;
            Camera.main.transform.rotation = Quaternion.LookRotation(target.forward);
            Camera.main.transform.LookAt(target);
        }
    }

    private DMCameraControl.DMouseOrbit mo;
    private DMCameraControl.DMCameraViewMove cvm;
    private DMCameraControl.ThirdCameraControl tc;
    private void testaaa(bool isMove)
    {
        if (mo == null) mo = Camera.main.gameObject.AddComponent<DMCameraControl.DMouseOrbit>();
        if (cvm == null) cvm = Camera.main.gameObject.AddComponent<DMCameraControl.DMCameraViewMove>();
        if (tc == null) tc = Camera.main.gameObject.AddComponent<DMCameraControl.ThirdCameraControl>();

        cvm.enabled = isMove;
        mo.enabled = isMove;
    }
}