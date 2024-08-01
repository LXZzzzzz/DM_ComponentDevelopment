using UnityEngine;
using UnityEngine.UI;

public class msgCell : DMonoBehaviour
{
    private string info;

    public void Init(string time, string msgInfo)
    {
        transform.GetChild(0).GetComponent<Text>().text = time;
        transform.GetChild(1).GetComponent<Text>().text = msgInfo;
        info = msgInfo;
    }

    public void ChangeController(string ctrlName)
    {
        if (ctrlName.Contains('总'))
        {
            gameObject.SetActive(true);
            return;
        }

        gameObject.SetActive(info.Contains(ctrlName));
    }
}