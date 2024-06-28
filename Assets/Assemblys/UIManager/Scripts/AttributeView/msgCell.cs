using UnityEngine;
using UnityEngine.UI;

public class msgCell : DMonoBehaviour
{
    public void Init(string time, string msgInfo)
    {
        transform.GetChild(0).GetComponent<Text>().text = time;
        transform.GetChild(1).GetComponent<Text>().text = msgInfo;
    }
}