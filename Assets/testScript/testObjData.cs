using UnityEngine;

public class testObjData : MonoBehaviour
{
    public testClass test;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.LogError(test.aaa);
        }
    }
}

public class testClass
{
    public float aaa;
    public float bbb;
}