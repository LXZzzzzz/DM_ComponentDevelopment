using UnityEngine;

namespace UiManager
{
    public class CreatUI
    {
        public static GameObject LoadPanel(string UIName)
        {
            GameObject prefab = null/*AsstesManager.Instance.LoadAsset<GameObject>(Config.UIPath + UIName)*/;
            GameObject go = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, GameObject.Find("/Canvas").transform);
            go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            go.name = prefab.name;
            RectTransform goRT = go.GetComponent<RectTransform>();
            goRT.offsetMax = Vector2.zero;
            goRT.offsetMin = Vector2.zero;
            goRT.SetAsLastSibling();
            return go;
        }

        public static T LoadView<T>(string name, Transform parent) where T : BasePanel
        {
            GameObject uiv = null/*AsstesManager.Instance.LoadAsset<GameObject>(Config.UIPath + name)*/;

            GameObject go = Object.Instantiate(uiv, parent);
            go.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go.GetComponent<T>();
        }
    }
}
