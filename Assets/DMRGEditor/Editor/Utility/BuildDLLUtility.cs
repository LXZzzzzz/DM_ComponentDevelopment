using Microsoft.CSharp;
using System.CodeDom.Compiler;
using UnityEngine;
using System.IO;

namespace DM.RGEditor
{
    public class BuildDLLUtility
    {
       /// <summary>
       /// 编译指定的.cs文件为dll
       /// </summary>
       /// <param name="outputPath">输出路径(包含文件名)</param>
       /// <param name="references">添加引用</param>
       /// <param name="codes">cs文件路径集(绝对路径)</param>
        public static void Build(string outputPath,string[] references,string[] codes)
        {
            // 编译选项
            CompilerParameters compilerParams = new CompilerParameters();
            compilerParams.GenerateExecutable = false;
            compilerParams.GenerateInMemory = false;
            compilerParams.OutputAssembly = outputPath;
            compilerParams.IncludeDebugInformation = true;
            compilerParams.TreatWarningsAsErrors = false;
            // 添加引用
            for (int i = 0; i < references.Length; i++)
            {
                compilerParams.ReferencedAssemblies.Add(references[i]);
            }
            //编译生成
            var provider = new CSharpCodeProvider();
            var compile = provider.CompileAssemblyFromFile(compilerParams, codes);
            if (compile.Errors.HasErrors)
            {
                Debug.LogError("----------------编译错误-----------------");
                foreach (CompilerError err in compile.Errors)
                {
                    Debug.LogError(err.FileName + " " + err.ErrorText);
                }
            }
        }
        /// <summary>
        /// 编译指定文件夹下的.cs文件为dll
        /// </summary>
        /// <param name="outputPath">输出路径(包含文件名)</param>
        /// <param name="references">添加引用</param>
        /// <param name="directory">指定文件夹</param>
        public static void Build(string outputPath, string[] references, string directory)
        {
            string[] codefiles = Directory.GetFiles(directory);
            Build(outputPath,references,codefiles);
        }
    } 
}
