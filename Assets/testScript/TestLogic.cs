using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using ReportGenerate;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using UiManager;
using UnityEngine;
using Vectrosity;
using ToolsLibrary.ProgrammePart;

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

        myass = new List<AudioSource>();
        mywms = new List<WingMark>();

        mywms = fj.transform.GetComponentsInChildren<WingMark>(true).ToList();
    }

    public FirePointLogic fp;
    public float waterTime, mj;

    public GameObject fj;

    public string aa, bb;

    private List<AudioSource> myass;
    private List<WingMark> mywms;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MyDataInfo.gameState = GameState.GameStart;
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
            em.EvalMaterialCompute(JsonConvert.DeserializeObject<ResultMaterialPersonOutData>(aa), JsonConvert.DeserializeObject<ResultRescueSystemData>(bb), 19.14555f, 8.2088f);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            fp.OnStart();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            var anis = fj.transform.GetComponentsInChildren<Animation>();
            for (int i = 0; i < anis.Length; i++)
            {
                anis[i].Stop();
            }

            if (myass.Count == 0)
            {
                var ass = fj.transform.GetComponentsInChildren<AudioSource>();
                for (int i = 0; i < ass.Length; i++)
                {
                    if (ass[i].enabled) myass.Add(ass[i]);
                }
            }

            myass.ForEach(x => x.gameObject.SetActive(false));
            mywms.ForEach(x => x.gameObject.SetActive(x.mark == 0));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            var anis = fj.transform.GetComponentsInChildren<Animation>();
            for (int i = 0; i < anis.Length; i++)
            {
                anis[i].Play();
            }

            myass.ForEach(x => x.gameObject.SetActive(true));
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            ProgrammeDataManager.Instance.LoadProgramme("D:/DM/DM2.2.0D/DM_Data/MapLib/Scheme");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            EventManager.Instance.EventTrigger(Enums.EventType.ShowAMsgInfo.ToString(), "执行装载资源的操作");
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