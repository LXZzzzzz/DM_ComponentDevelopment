using System.Collections.Generic;
using System.IO;
using System.Xml;
using DataTranfsers;
using Enums;
using Newtonsoft.Json;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;
using EventType = Enums.EventType;

public class UISubjectiveRatingView : BasePanel
{
    private Dropdown roles;
    private List<int> roleLevels;
    private List<string> roleIds;
    private Transform ratingItemsParent;
    private RatingCell ratingCell;
    private Dictionary<int, List<SubjectiveRatingData>> subjectiveRatingDatas;
    private List<RatingCell> ratings;
    private ScoreStatistics scores;

    public override void Init()
    {
        base.Init();
        subjectiveRatingDatas = new Dictionary<int, List<SubjectiveRatingData>>();
        loadTrainsData();
        roles = GetControl<Dropdown>("dp_ChangeCC");
        ratingCell = GetComponentInChildren<RatingCell>(true);
        roles.options.Clear();
        roleLevels = new List<int>();
        roleIds = new List<string>();
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            if (MyDataInfo.playerInfos[i].ClientLevel == -1) continue;
            roles.options.Add(new Dropdown.OptionData(MyDataInfo.playerInfos[i].ClientLevelName));
            roleLevels.Add(MyDataInfo.playerInfos[i].ClientLevel);
            roleIds.Add(MyDataInfo.playerInfos[i].RoleId);
        }

        scores = new ScoreStatistics();
        scores.thirdZhyScores = new List<SecondZhyScore>();
        for (int i = 0; i < roleIds.Count; i++)
        {
            scores.thirdZhyScores.Add(new SecondZhyScore() { roleId = roleIds[i] });
        }

        ratingItemsParent = GetControl<ScrollRect>("ScrollRectPrefab").content;
        roles.onValueChanged.AddListener(showRoleTrains);
        GetControl<Button>("sure").onClick.AddListener(saveLogic);
        GetControl<Button>("close").onClick.AddListener(() => Close(UIName.UISubjectiveRatingView));
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        if (roleLevels.Count > 0)
        {
            roles.value = 0;
            showRoleTrains(0);
        }
    }

    public override void HideMe()
    {
        base.HideMe();
    }

    private void loadTrainsData()
    {
        string filePath = Path.Combine(Application.dataPath, "MapLib", "XmlData", "SubjectiveRatingData.xml");

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

        // 遍历所有节点
        foreach (XmlNode bookNode in root.ChildNodes)
        {
            string id = bookNode.Attributes["id"].Value;

            SubjectiveRatingData srd = new SubjectiveRatingData();
            srd.gameScene = new List<int>();
            var itemDatas = bookNode["GameScene"]?.InnerText.Split('_');
            for (int i = 0; i < itemDatas.Length; i++)
            {
                if (int.TryParse(itemDatas[i], out int sceneId)) srd.gameScene.Add(sceneId);
            }

            srd.trainingRole = bookNode["TrainingRole"]?.InnerText;
            if (int.TryParse(bookNode["RoleId"]?.InnerText, out int itemId)) srd.roleId = itemId;
            srd.trainingPoint = bookNode["TrainingPoint"]?.InnerText;
            srd.associationType = bookNode["AssociationType"]?.InnerText;
            srd.scoringCriteria = bookNode["ScoringCriteria"]?.InnerText;

            if (!subjectiveRatingDatas.ContainsKey(srd.roleId))
                subjectiveRatingDatas.Add(srd.roleId, new List<SubjectiveRatingData>());
            subjectiveRatingDatas[srd.roleId].Add(srd);
        }
    }

    private void showRoleTrains(int id)
    {
        var roleRatingData = subjectiveRatingDatas[roleLevels[id]];
        //取到该等级角色的评分模板，展示出来
        if (ratings == null) ratings = new List<RatingCell>();
        ratings.ForEach(a => a.OnDelete());
        ratings.ForEach(a => Destroy(a.gameObject));
        ratings.Clear();
        for (int i = 0; i < roleRatingData?.Count; i++)
        {
            if (!roleRatingData[i].gameScene.Contains(MyDataInfo.gameScene)) continue;
            var ratingItem = Instantiate(ratingCell, ratingItemsParent);
            ratingItem.OnInit(roleRatingData[i], roleIds[id]);
            ratingItem.gameObject.SetActive(true);
            ratings.Add(ratingItem);
        }
    }

    private void saveLogic()
    {
        ratings.ForEach(a => a.GetScore(ref scores));
        //拿完分数，再算出每个端的分数，存起来发给cc;
        string jsonData = JsonConvert.SerializeObject(scores);
        // string dataStr = AESUtils.Encrypt(jsonData);
        EventManager.Instance.EventTrigger(EventType.scorseToCc.ToString(), jsonData);
        EventManager.Instance.EventTrigger(EventType.ShowTipUI.ToString(), $"完成{roles.options[roles.value].text}的主观评分");
    }
}

public class SubjectiveRatingData
{
    /// <summary>
    /// 展示场景
    /// </summary>
    public List<int> gameScene;

    /// <summary>
    /// 训练角色
    /// </summary>
    public string trainingRole;

    /// <summary>
    /// 角色ID
    /// </summary>
    public int roleId;

    /// <summary>
    /// 训练点
    /// </summary>
    public string trainingPoint;

    /// <summary>
    /// 关联训练点类型
    /// </summary>
    public string associationType;

    /// <summary>
    /// 评分规则
    /// </summary>
    public string scoringCriteria;
}