using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System;

namespace DM.RGEditor
{
    public class DMDynamicProperty
    {
        public string Name;
        public Type Type;  //反射获取FieldType
    }
    public class DynamicCodeUtility
    {
        /// <summary>
        /// 动态生成cs代码文件
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="properties">属性</param>
        /// <param name="outputFile">输出路径(.cs)</param>
        /// <param name="nameSpace">所在命名空间</param>
        /// <returns></returns>
        public static string CodeCompile(string className, DMDynamicProperty[] properties,string outputFile,string nameSpace="DM.RGEditor")
        {
            //代码编译器单元
            CodeCompileUnit unit = new CodeCompileUnit();
            //命名空间
            CodeNamespace sampleNamespace = new CodeNamespace(nameSpace);
            //引用命名空间
            sampleNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            //类的定义
            CodeTypeDeclaration Customerclass = new CodeTypeDeclaration(className);
            //指定Class
            Customerclass.IsClass = true;
            Customerclass.TypeAttributes = TypeAttributes.Public;
            //把这个类放在这个命名空间下
            sampleNamespace.Types.Add(Customerclass);
            //添加继承关系
            Customerclass.BaseTypes.Add(typeof(DMScriptableObject));
            //添加命名空间
            unit.Namespaces.Add(sampleNamespace);
            //添加属性(字段，field.Attributes = MemberAttributes.Private;)
            for (int i = 0; i < properties.Length; i++)
            {
                CodeMemberField field = new CodeMemberField(properties[i].Type, properties[i].Name);
                field.Attributes = MemberAttributes.Public;
                Customerclass.Members.Add(field);
            }
            //CodeMemberProperty property = new CodeMemberProperty();
            //property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            //property.Name = "Id";
            //property.HasGet = true;
            //property.HasSet = true;
            //property.Type = new CodeTypeReference(typeof(System.String));
            //property.Comments.Add(new CodeCommentStatement("这是Id属性"));
            //property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_Id")));
            //property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_Id"), new CodePropertySetValueReferenceExpression()));
            //Customerclass.Members.Add(property);
            //添加方法（使用CodeMemberMethod)
            //添加构造器(使用CodeConstructor)
            //添加程序入口点（使用CodeEntryPointMethod）
            //添加事件（使用CodeMemberEvent) 
            //添加特征(使用 CodeAttributeDeclaration)
            //Customerclass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
            //生成代码
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.BlankLinesBetweenMembers = true;
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(outputFile))
            {
                provider.GenerateCodeFromCompileUnit(unit, sw, options);
            }
            return outputFile;
        }
    }
}

