using UnityEngine;

public class testTemplateLogic : MonoBehaviour
{
    public int testNum;

    public void Init(int intValue)
    {
        testNum = intValue;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log(testNum);
        }
    }
}