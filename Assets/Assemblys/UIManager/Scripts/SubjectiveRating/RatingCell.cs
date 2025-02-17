using UnityEngine;
using UnityEngine.UI;

public class RatingCell : DMonoBehaviour
{
    public Text roleName;
    public Text trainingDescribe;
    public Button showDataBtn;
    public Text scoringCriteria;
    public InputField score;
    public void Init(SubjectiveRatingData data)
    {
        roleName.text = data.trainingRole;
        trainingDescribe.text = data.secondaryTrainingPoint;
        scoringCriteria.text = data.scoringCriteria;
        showDataBtn.onClick.AddListener(onShowData);
        score.onValueChanged.AddListener(onCheckValue);
    }

    public void GetScore()
    {
        //返回分数，默认10分
    }

    private void onShowData()
    {
        //打开数据展示
    }

    private void onCheckValue(string v)
    {
        if (int.TryParse(v,out int a))
        {
            score.text = Mathf.Clamp(a, 0, 10).ToString();
        }
    }
}
