using DM.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DM.RGEditor
{
    public class RGModelCreate
    {
        private static RGDataConfig scriptConfig;
        private static List<string> customClassPaths;

        public static void CreateScriptDLLAndDataOnPrefab(ModelAssetTemplate asset,string filePath,string signal="")
        {
            customClassPaths = new List<string>();
            scriptConfig = new RGDataConfig();
            if (asset.isAssignMain)  //指定主入口
                scriptConfig.MainClass = asset.MainCalss;
            //Assets路径
            FindCustomScriptsAndPaths(asset.Prefab, "");

            string componentPath = filePath + "/" + asset.AssetName;
            if (!Directory.Exists(componentPath))
                Directory.CreateDirectory(componentPath);

            if (asset.IsCreateScript)
            {
                string scriptPath = componentPath + "/script";
                if (!Directory.Exists(scriptPath))
                    Directory.CreateDirectory(scriptPath);
                //script和mdb
                string outputScriptFile =scriptPath+"/"+asset.AssetName + "-script-" + signal+".script";
                for (int i = 0; i < asset.OtherScriptPaths.Length; i++)
                {
                    if (!string.IsNullOrEmpty(asset.OtherScriptPaths[i])) //指定文件夹的其他脚本
                    {
                        //绝对路径
                        string[] otherScripts = Directory.GetFiles(asset.OtherScriptPaths[i], "*.cs", SearchOption.AllDirectories);
                        customClassPaths.AddRange(otherScripts);
                    }
                }              
                //包含Asset路径和绝对路径，脚本有可能是同一个，去重无法去掉，编译时会自动处理     
                customClassPaths = customClassPaths.Distinct().ToList(); //去重
           
                if (customClassPaths.Count > 0)
                {
                    BuildDLLUtility.Build(outputScriptFile, asset.ReferencePaths, customClassPaths.ToArray());
                    //scriptdata
                    IFS.XMLHelper.Serialize<RGDataConfig>(scriptConfig, outputScriptFile+".xml");
                }         
            }
        }
        private static void FindCustomScriptsAndPaths(GameObject go, string parentPath)
        {
            string path = string.IsNullOrEmpty(parentPath) ? go.name : parentPath + "/" + go.name;

            //获取该物体上的自定义脚本
            Component[] components = go.GetComponents(typeof(Component));
            foreach (Component component in components)
            {
                if (component.GetType().Assembly == typeof(MonoBehaviour).Assembly)
                {
                    continue;
                }
                //排除build-in内置脚本，排除插件脚本
                //只有继承DMonoBehaviour的脚本才会被打包出去
                //if (component.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                if (component.GetType().IsSubclassOf(typeof(DMonoBehaviour)))
                {
                    string className = component.GetType().Name;
                    string nameSpace = component.GetType().Namespace;
                    string classPath = GetScriptPath(className);
                    if (!customClassPaths.Contains(classPath))
                    {
                        customClassPaths.Add(classPath);
                    }
                       
                    //生成CustomScript实例数据
                    RGScript scriptData = new RGScript();
                    scriptData.SubPath = path;
                    scriptData.ClassName =string.IsNullOrEmpty(nameSpace)?className
                                          : nameSpace + "." + className;
                    scriptConfig.RGScripts.Add(scriptData);
                    FieldInfo[] fieldInfos = component.GetType().GetFields();
                    for (int i = 0; i < fieldInfos.Length; i++)
                    {
                        RGField scriptField = new RGField();
                        scriptField.Name = fieldInfos[i].Name;
                        scriptField.Type = fieldInfos[i].FieldType.ToString();
                        scriptField.Value = fieldInfos[i].GetValue(component)?.GetStringValue(scriptField.Type);
                        scriptData.RGFields.Add(scriptField);
                    }
                }
            }
            //递归遍历子物体
            foreach (Transform child in go.transform)
            {
                FindCustomScriptsAndPaths(child.gameObject, path);
            }
        }
        private static string GetScriptPath(string scriptName)
        {
            string[] paths = AssetDatabase.FindAssets(scriptName);
            for (int i = 0; i < paths.Length; i++)
            {
                string pathText = AssetDatabase.GUIDToAssetPath(paths[i]);
                if (pathText.EndsWith(".cs"))
                {
                    string aFirstName = pathText.Substring(pathText.LastIndexOf("/") + 1, (pathText.LastIndexOf(".") - pathText.LastIndexOf("/") - 1));  //文件名
                    if (aFirstName.Equals(scriptName))
                        return pathText;
                }
            }
            return null;
        }
        public static void CreateModelAssetBundleOnPrefab(ModelAssetTemplate asset,string filePath)
        {
            string modelPath = filePath + "/" + asset.AssetName+"/model";
            if (!Directory.Exists(modelPath))
                Directory.CreateDirectory(modelPath);

            //保存组件模型贴图资源
            string[] assetPaths = new string[]{
                          AssetDatabase.GetAssetPath(asset.Prefab.GetInstanceID()),
                          AssetDatabase.GetAssetPath(asset.Icon.GetInstanceID())};

            AssetBundleBuild[] abbs = new AssetBundleBuild[1];
            abbs[0].assetBundleName = asset.AssetName + "ab";
            abbs[0].assetNames = assetPaths;
            //todo 加载速度测试   //BuildAssetBundleOptions.ChunkBasedCompression
            BuildAssetBundleOptions options = asset.IsCompressModel ? 
                BuildAssetBundleOptions.None : BuildAssetBundleOptions.UncompressedAssetBundle;  
            BuildPipeline.BuildAssetBundles(modelPath, abbs,options, BuildTarget.StandaloneWindows);
        }
        public static void CreateModelTagOnPrefab(ModelAssetTemplate asset, string filePath,string signal="")
        {
            //生成数据
            BObject bObj = new BObject();
            //bObj.IsOn = true; //默认启用
            ComInfo info = new ComInfo();
            info.Name = asset.AssetName;
            info.Description = asset.Description;
            bObj.Info = info;

            ComTransform tran = new ComTransform(1);
            bObj.Transform = tran;
            //属性与放置标签
            bObj.Info.Layouts.SelfComponent = GetUseful(asset.listComponent);
            bObj.Info.Layouts.SelfContainer = GetUseful(asset.listContainer);
            if (bObj.Info.Layouts.SelfContainer.Count > 0)
                bObj.MarkProperties.Add(new ProContainer());
            bObj.Info.Layouts.LayoutComponent = GetUseful(asset.listLayoutComponent);
            bObj.Info.Layouts.LayoutContainer = GetUseful(asset.listLayoutContainer);
            bObj.Info.Layouts.LayoutTerrain = GetUseful(asset.listLayoutTerrain);
            bObj.Info.Layouts.Size = (int)asset.Size;
            //组件标签
            bObj.Info.Tags = GetUseful(asset.listTags);

            #region 特殊处理
            //AI
            TagItem path = bObj.Info.Tags.Con<TagItem>("5");
            if (path != null)
            {
                ProAIPath aiPath = new ProAIPath();
                bObj.MarkProperties.Add(aiPath);
            }
            //电线杆
            TagItem ppl = bObj.Info.Tags.Con<TagItem>("7");
            if (ppl != null)
            {
                ProPowerLine proPow = new ProPowerLine();
                bObj.MarkProperties.Add(proPow);
            }
            //评估端
            TagItem analysis = bObj.Info.Tags.Con<TagItem>("8");
            if (analysis != null && analysis.SubTags.Con<SubTagItem>("9") != null)
            {
                ProExam exams = new ProExam();
                bObj.MarkProperties.Add(exams);
            }
            //火源管理器
            TagItem fire = bObj.Info.Tags.Con<TagItem>("14");
            if (fire != null)
            {
                ProFire proFire = new ProFire();
                bObj.MarkProperties.Add(proFire);
            }
            //触发器效果的触发类型和效果类型
            bObj.AITrigger.Affect.TriggerTypes =GetUseful2(asset.listTriggers,true);
            bObj.AITrigger.Affect.AffectTypes = GetUseful2(asset.listAffects,true);

            TagItem tri = bObj.Info.Tags.Con<TagItem>("4");
            if (tri != null)
                bObj.AITrigger.Trigger.IsShow = true;
            #endregion

            //触发器触发的触发类型和效果类型
            bObj.AITrigger.Trigger.TriggerTypes = GetUseful2(asset.listTriggers,false);
            bObj.AITrigger.Trigger.AffectTypes =GetUseful2(asset.listAffects,false);

            //序列化保存
            string tagPath = filePath + "/" + asset.AssetName + "/config";
            if (!Directory.Exists(tagPath))
                Directory.CreateDirectory(tagPath);

            string outputTagPath = tagPath +"/TagConfig.data";
            bObj.Serialize(outputTagPath);
        }

        private static List<TagItem> GetUseful(List<RGTagItem> source)
        {
            List<TagItem> target = new List<TagItem>();
            foreach (RGTagItem item in source)
            {
                if (item.IsOn)
                {
                    TagItem tag = new TagItem();
                    tag.Id = item.Id;
                    tag.Name = item.Name;
                    tag.IsOn = true;
                    tag.BuiltIn = item.BuiltIn;
                    tag.SubFoldOut = true;
                    foreach (RGSubTagItem subItem in item.SubTags)
                    {
                        if (subItem.IsOn)
                        {
                            SubTagItem subTag = new SubTagItem();
                            subTag.Id = subItem.Id;
                            subTag.SubName = subItem.SubName;
                            subTag.IsOn = true;
                            subTag.IsModify = subItem.IsModify;
                            tag.SubTags.Add(subTag);
                        }
                    }
                    target.Add(tag);
                }
            }
            return target;
        }
        private static List<AffectTriggerItem> GetUseful2(List<RGAffectTriggerItem> source,bool isOnActive)
        {
            List<AffectTriggerItem> target = new List<AffectTriggerItem>();
            foreach (RGAffectTriggerItem item in source)
            {
                bool isOn=isOnActive?item.IsOn: true;
                if (isOn)
                {
                    AffectTriggerItem tag = new AffectTriggerItem();
                    tag.Id = item.Id;
                    tag.Name = item.Name;
                    tag.IsOn = item.IsOn;
                    tag.BuiltIn = item.BuiltIn;
                    target.Add(tag);
                }
            }
            return target;
        }
    }
}
