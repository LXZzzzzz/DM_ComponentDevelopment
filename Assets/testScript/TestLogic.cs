using System.Collections;
using System.Collections.Generic;
using UiManager;
using UnityEngine;

public class TestLogic : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            
            UIManager.Instance.ShowPanel<UIMap>(UIName.UIMap,null);
            UIManager.Instance.ShowPanel<UITopMenuView>(UIName.UITopMenuView, 1);
            UIManager.Instance.ShowPanel<UICommanderView>(UIName.UICommanderView,1);
        }
    }
}
