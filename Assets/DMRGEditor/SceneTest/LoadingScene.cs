using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    AsyncOperation oper;
	// Use this for initialization
	void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
       // StartCoroutine(LoadTerrainScene("test1"));
	}
    private void Update()
    {
        if (oper != null)
        {
            Debug.Log("加载进度："+oper.progress);
        }
    }
    public IEnumerator LoadTerrainScene(string name)
    {
        string path = Application.dataPath+"/" + name;
        string abPath = "file:///" + path + "/" + name + ".unity3d";
        //WWW www =WWW.LoadFromCacheOrDownload(abPath,0);
        WWW www = new WWW(abPath);
        yield return www;
        AssetBundle bundleTerrain = www.assetBundle;
        UnityEngine.SceneManagement.SceneManager.LoadScene(name,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
        yield return new WaitForSeconds(0.5f);
        bundleTerrain.Unload(false);
    }
    public AsyncOperation LoadTerrainSceneFromFile(string name)
    {
        string path = Application.dataPath + "/DMBundle/AssetBundle/" + name+"/"+name+".unity3d";
        AssetBundle bundleTerrain = AssetBundle.LoadFromFile(path);
        return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(name,
            UnityEngine.SceneManagement.LoadSceneMode.Single);
        //bundleTerrain.Unload(false);
    }
    public string sceneName = "";
    private void OnGUI()
    {
        sceneName=GUILayout.TextField(sceneName);
        if (GUILayout.Button("LoadSene"))
        {
            LoadTerrainSceneFromFile(sceneName);
        }
        //if (GUILayout.Button("test www"))
        //{
        //    StartCoroutine(LoadTerrainScene("test"));
        //}
        //if (GUILayout.Button("test1 www"))
        //{
        //    StartCoroutine(LoadTerrainScene("test1"));
        //}
        //if (GUILayout.Button("test file"))
        //{
        //    LoadTerrainSceneFromFile("test");
        //}
        //if (GUILayout.Button("test1 file"))
        //{
        //    LoadTerrainSceneFromFile("test1");
        //}
    }
}
