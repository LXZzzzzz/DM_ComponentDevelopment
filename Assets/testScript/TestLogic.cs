using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using DG.Tweening;
using Newtonsoft.Json;
using ReportGenerate;
using ToolsLibrary;
using ToolsLibrary.EquipPart;
using ToolsLibrary.PathPart;
using UiManager;
using UnityEngine;
using Vectrosity;
using ToolsLibrary.ProgrammePart;
using UnityEngine.Events;
using UnityEngine.UI;
using Application = UnityEngine.Application;
using EventType = Enums.EventType;

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

    public string pathPlanningData;

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
        fp?.Init(5, 10, 30000, "1111111", "", "");

        myass = new List<AudioSource>();
        mywms = new List<WingMark>();

        // mywms = fj.transform.GetComponentsInChildren<WingMark>(true).ToList();

        // ttl.Init(4949);
    }

    public FirePointLogic fp;
    public float waterTime, mj;

    public GameObject fj;

    public string aa, bb;

    // public testTemplateLogic ttl;

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
            // UIManager.Instance.ShowPanel<UICommanderDirector>(UIName.UICommanderDirector, null);
            EventManager.Instance.EventTrigger<string, object>(Enums.EventType.ShowUI.ToString(), "AttributeView", null);
            // initLine();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            fp.waterPour(waterTime, mj, mj);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            fp.Init(5, 10, 30000, "1111111", "", "");
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
            ResultFireWaterOutData rfout = new ResultFireWaterOutData
            {
                任务结束时投水总量 = 12,
                任务结束时过火总面积 = 12,
                任务结束时燃烧面积 = 12,
                任务初始过火总面积 = 12,
                任务初始燃烧面积 = 12,
                开始投水时刻 = "时间", //增大检测范围，防止浮点误差
                取水点到投水点的最短路径 = 122,
                任务结束时刻 = 3600,
                总航程 = 1000f,
                直升机总架次 = 12,
                火场数量 = 12,
                取水点数量 = 12,
                任务结束时火场投水总重量 = 1000
            };
            PDFReport report = new PDFReport();
            EvalManage em = new EvalManage();
            var rfwd=em.EvalWaterCompute(JsonConvert.DeserializeObject<ResultFireWaterOutData>(aa), JsonConvert.DeserializeObject<ResultFireWaterSystemData>(bb));
            // report.CreateWaterMissionReport(System.DateTime.Now.ToString("HH_mm_ss"), "-效能评估报告", "mName", "mId", "mAbstract", rfwd, rfout, showAllOperatorInfos, heliWaterMegList, playerEquips, playerZiyuans, reportPlayers.Count, personAssData);

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

        if (Input.GetKeyDown(KeyCode.K))
        {
            EventManager.Instance.EventTrigger(Enums.EventType.SwitchMapModel.ToString(), 2);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.LogError(PathPointManager.Instance.PackedData());
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Msg_testPlan(pathPlanningData);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MsgReceive_CreatZaiqu(asldfjlsdj);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            EventManager.Instance.EventTrigger<string, UnityAction>(EventType.ShowTipUIAndCb.ToString(), "当前天气下雨，是否全部返航",
                () => { Debug.LogError("确认了拉萨的"); });
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GetOilConsumption(speedd, weightt);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log(FloatToDMS(45.56f));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            jiexiXml();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            CaptureScreenshot2(new Rect(new Vector2(0, 0), new Vector2(1080, 1080)));
        }

        if (isRunTimer) runTimer();

        if (routePoints != null)
        {
            // testPoint.anchoredPosition = routePoints[1];
        }
    }

    private void jiexiXml()
    {
        Debug.LogError(Enums.TrainsPintType.JZCompletePlan);
        Debug.LogError(Enums.TrainsPintType.JZCompletePlan.ToString());
        string filePath = Path.Combine(Application.streamingAssetsPath, "XmlData", "TrainPointData.xml");

        // 检查文件是否存在
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return;
        }

        // 读取文件内容
        string fileContent = File.ReadAllText(filePath);

        // 创建一个 XmlDocument 对象
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(fileContent);

        // 获取根节点
        XmlNode root = xmlDoc.DocumentElement;

        // 遍历所有 book 节点
        foreach (XmlNode bookNode in root.ChildNodes)
        {
            string id = bookNode.Attributes["id"].Value;

            string author = bookNode["author"]?.InnerText;
            string title = bookNode["title"]?.InnerText;
            string genre = bookNode["genre"]?.InnerText;
            string price = bookNode["price"]?.InnerText;
            string publishDate = bookNode["publish_date"]?.InnerText;
            string description = bookNode["description"]?.InnerText;

            // 输出信息
            Debug.Log($"Book ID: {id}");
            Debug.Log($"Author: {author}");
            Debug.Log($"Title: {title}");
            Debug.Log($"Genre: {genre}");
            Debug.Log($"Price: {price}");
            Debug.Log($"Publish Date: {publishDate}");
            Debug.Log($"Description: {description}");
            Debug.Log("----------");
        }
    }

    /// <summary>
    /// float 转度分秒
    /// </summary>
    /// <param name="coordinate"></param>
    /// <returns></returns>
    public Vector3 FloatToDMS(float coordinate)
    {
        // 获取度数（整数部分）
        int degrees = Mathf.FloorToInt(coordinate);

        // 获取分数（剩余的小数部分 * 60）
        float minutesDecimal = (Mathf.Abs(coordinate) - Mathf.Abs(degrees)) * 60;
        int minutes = Mathf.FloorToInt(minutesDecimal);

        // 获取秒数（剩余的小数部分 * 60）
        float secondsDecimal = (minutesDecimal - minutes) * 60;
        int seconds = Mathf.FloorToInt(secondsDecimal);

        // 返回格式化的度分秒字符串
        return new Vector3(degrees, minutes, seconds);
    }

    public float speedd, weightt;

    private float GetOilConsumption(float Speed, float Weight)
    {
        float Temperature = 1; //温度
        float Altitude = 3000; //高度
        double OilConsumption = 606.54742f - 3.56870f * Speed + 0.01127f * Weight - 0.32404f * Temperature - 0.09671f *
            Altitude + 0.01986f * Mathf.Pow(Speed, 2f) - 0.00014f * Speed * Weight - 0.01365f * Speed * Temperature -
            0.00015f * Speed * Altitude + 0.00024 * Weight * Temperature + 0.00001 * Weight * Altitude + 0.01545f *
            Mathf.Pow(Temperature, 2f) - 0.00015f * Temperature * Altitude + 0.00001f * Mathf.Pow(Altitude, 2f);

        Debug.LogError(OilConsumption);
        return (float)OilConsumption;
    }

    private void Msg_testPlan(string dataStr)
    {
        //把字符串解析为数据
        string deStr = AESUtils.Decrypt(dataStr);
        var allPathPoints = JsonConvert.DeserializeObject<List<PathPoint>>(deStr, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        Debug.Log(allPathPoints.Count);
    }

    public string asldfjlsdj;

    private void MsgReceive_CreatZaiqu(string dataStr)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.Converters.Add(new PolymorphicConverter_ZyVariableDataBase());
        var currentData = JsonConvert.DeserializeObject<CreatZaiquData>(dataStr, settings);
        Debug.Log(currentData.tempId);
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
        vl.rectTransform.localScale = Vector3.one * 10;
        vl.active = true;
        if (ColorUtility.TryParseHtmlString("#FF0000", out Color color))
            vl.color = color;
        vl.Draw();
        vl.active = true;
    }
    
    /// <summary>
    /// Captures the screenshot2.
    /// </summary>
    /// <returns>The screenshot2.</returns>
    /// <param name="rect">Rect.截图的区域，左下角为o点</param>
    Texture2D CaptureScreenshot2(Rect rect)
    {
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);//先创建一个的空纹理，大小可根据实现需要来设置
        screenShot.ReadPixels(rect, 0, 0);//读取屏幕像素信息并存储为纹理数据，
        screenShot.Apply();
        byte[] bytes = screenShot.EncodeToPNG();//然后将这些纹理数据，成一个png图片文件
    
        string filename = Application.streamingAssetsPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("截屏了一张图片: {0}", filename));
        //最后，我返回这个Texture2d对象，这样我们直接，所这个截图图示在游戏中，当然这个根据自己的需求的。
        return screenShot;
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