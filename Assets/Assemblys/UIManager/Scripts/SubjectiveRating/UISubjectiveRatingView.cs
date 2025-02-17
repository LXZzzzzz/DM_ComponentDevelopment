using System.Collections.Generic;
using System.IO;
using System.Xml;
using ToolsLibrary;
using UiManager;
using UnityEngine;
using UnityEngine.UI;

public class UISubjectiveRatingView : BasePanel
{
    private Dropdown roles;
    private List<int> roleIds;
    private Transform ratingItemsParent;
    private RatingCell ratingCell;
    private Dictionary<int, List<SubjectiveRatingData>> subjectiveRatingDatas;
    private List<RatingCell> ratings;

    public override void Init()
    {
        base.Init();
        subjectiveRatingDatas = new Dictionary<int, List<SubjectiveRatingData>>();
        loadTrainsData();
        roles.options.Clear();
        roleIds = new List<int>();
        for (int i = 0; i < MyDataInfo.playerInfos.Count; i++)
        {
            roles.options.Add(new Dropdown.OptionData(MyDataInfo.playerInfos[i].PlayerName));
            roleIds.Add(MyDataInfo.playerInfos[i].ClientLevel);
        }

        roles.onValueChanged.AddListener(showRoleTrains);
    }

    public override void ShowMe(object userData)
    {
        base.ShowMe(userData);
        roles.value = 0;
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
            srd.secondaryTrainingPoint = bookNode["SecondaryTrainingPoint"]?.InnerText;
            srd.scoringCriteria = bookNode["ScoringCriteria"]?.InnerText;

            if (!subjectiveRatingDatas.ContainsKey(srd.roleId))
                subjectiveRatingDatas.Add(srd.roleId, new List<SubjectiveRatingData>());
            subjectiveRatingDatas[srd.roleId].Add(srd);
        }
    }

    private void showRoleTrains(int id)
    {
        var roleRatingData = subjectiveRatingDatas[roleIds[id]];
        //取到该等级角色的评分模板，展示出来
        if (ratings == null) ratings = new List<RatingCell>();
        ratings.ForEach(a => Destroy(a.gameObject));
        ratings.Clear();
        for (int i = 0; i < roleRatingData?.Count; i++)
        {
            if (!roleRatingData[i].gameScene.Contains(MyDataInfo.gameScene)) continue;
            var ratingItem = Instantiate(ratingCell, ratingItemsParent);
            ratingItem.Init(roleRatingData[i]);
            ratingItem.gameObject.SetActive(true);
            ratings.Add(ratingItem);
        }
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
    /// 关联一级训练点
    /// </summary>
    public string trainingPoint;

    /// <summary>
    /// 关联训练点类型
    /// </summary>
    public string associationType;

    /// <summary>
    /// 二级训练点描述
    /// </summary>
    public string secondaryTrainingPoint;

    /// <summary>
    /// 评分规则
    /// </summary>
    public string scoringCriteria;
}