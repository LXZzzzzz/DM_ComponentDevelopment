using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using ReportGenerate;
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

    public float speed;

    public testObjData to;
    public RectTransform testPoint;
    public HelicopterController hc;

    void Start()
    {
        allEquip = new List<EquipBase>();
        var eqs = GetComponentsInChildren<EquipBase>();
        for (int i = 0; i < eqs.Length; i++)
        {
            eqs[i].BObjectId = i.ToString();
            allEquip.Add(eqs[i]);
        }

        MyDataInfo.sceneAllEquips = allEquip;

        testDic = new Dictionary<string, string>();

        to.test = new testClass() { aaa = 20, bbb = 30 };
        fp.Init(5, 10, 30000, "1111111");
    }

    public FirePointLogic fp;
    public float waterTime, mj;

    public GameObject fj;

    public string aa, bb;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            EventManager.Instance.EventTrigger<string, object>(Enums.EventType.ShowUI.ToString(), "IconShow", null);
            UIManager.Instance.ShowPanel<UIMap>(UIName.UIMap, new Vector2(18000, 18000));
            UIManager.Instance.ShowPanel<UITopMenuView>(UIName.UITopMenuView, 1);
            UIManager.Instance.ShowPanel<UICommanderView>(UIName.UICommanderView, 1);
            EventManager.Instance.EventTrigger<string, object>(Enums.EventType.ShowUI.ToString(), "AttributeView", null);
            initLine();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            fp.waterPour(waterTime, mj, mj);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            fp.Init(5, 10, 30000, "1111111");
            float aa = 400 / mj;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            fp.getFireData(out float ghmj, out float rsmj, out float csghmj, out float csrsmj, out float tszl);
            Debug.LogError($"当前燃烧面积{rsmj}");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            fp.updateBA();
            // Dictionary<HeliData, List<HeliSortieData>> asd = new Dictionary<HeliData, List<HeliSortieData>>();
            // asd.Add(new HeliData() {Consumption = 100}, new List<HeliSortieData>() { new HeliSortieData() { EndMissonTime = 20 } });
            //
            // string qw = JsonConvert.SerializeObject(asd);
            // Debug.Log(qw);
            //
            // Dictionary<HeliData, List<HeliSortieData>> er = JsonConvert.DeserializeObject<Dictionary<HeliData, List<HeliSortieData>>>(qw);
            // return;
            EvalManage em = new EvalManage();
            em.EvalWaterCompute(JsonConvert.DeserializeObject<ResultFireWaterOutData>(aa), JsonConvert.DeserializeObject<ResultFireWaterSystemData>(bb));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fp.OnStart();
        }

        if (isRunTimer) runTimer();

        if (routePoints != null)
        {
            // testPoint.anchoredPosition = routePoints[1];
        }
    }

    private float timer, timeDuration, skillProgress;
    private bool isRunTimer;

    private void openTimer(float duration)
    {
        timer = 0;
        timeDuration = duration;
        skillProgress = 0;
        isRunTimer = true;
    }

    private void runTimer()
    {
        timer += Time.deltaTime * speed;
        skillProgress = timer / timeDuration;
        Debug.LogError(skillProgress);
        if (timer >= timeDuration)
        {
            // 计时结束，执行相关操作
            skillProgress = 1;
            timer = 0; // 重置计时器
            isRunTimer = false;
        }
    }

    private List<Vector2> routePoints;

    private void initLine()
    {
        routePoints = new List<Vector2>();
        routePoints.Add(Vector2.zero);
        routePoints.Add(Vector2.one * 30);
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

public class testaaa
{
    public float aaa;

    public static explicit operator testaaa(string jsonString)
    {
        return JsonConvert.DeserializeObject<testaaa>(jsonString);
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}