using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using System;

namespace DM.RGEditor
{
    public class DMDynamicProperty
    {
        public string Name;
        public Type Type;  //�����ȡFieldType
    }
    public class DynamicCodeUtility
    {
        /// <summary>
        /// ��̬����cs�����ļ�
        /// </summary>
        /// <param name="className">����</param>
        /// <param name="properties">����</param>
        /// <param name="outputFile">���·��(.cs)</param>
        /// <param name="nameSpace">���������ռ�</param>
        /// <returns></returns>
        public static string CodeCompile(string className, DMDynamicProperty[] properties,string outputFile,string nameSpace="DM.RGEditor")
        {
            //�����������Ԫ
            CodeCompileUnit unit = new CodeCompileUnit();
            //�����ռ�
            CodeNamespace sampleNamespace = new CodeNamespace(nameSpace);
            //���������ռ�
            sampleNamespace.Imports.Add(new CodeNamespaceImport("UnityEngine"));
            //��Ķ���
            CodeTypeDeclaration Customerclass = new CodeTypeDeclaration(className);
            //ָ��Class
            Customerclass.IsClass = true;
            Customerclass.TypeAttributes = TypeAttributes.Public;
            //������������������ռ���
            sampleNamespace.Types.Add(Customerclass);
            //��Ӽ̳й�ϵ
            Customerclass.BaseTypes.Add(typeof(DMScriptableObject));
            //��������ռ�
            unit.Namespaces.Add(sampleNamespace);
            //�������(�ֶΣ�field.Attributes = MemberAttributes.Private;)
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
            //property.Comments.Add(new CodeCommentStatement("����Id����"));
            //property.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_Id")));
            //property.SetStatements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), "_Id"), new CodePropertySetValueReferenceExpression()));
            //Customerclass.Members.Add(property);
            //��ӷ�����ʹ��CodeMemberMethod)
            //��ӹ�����(ʹ��CodeConstructor)
            //��ӳ�����ڵ㣨ʹ��CodeEntryPointMethod��
            //����¼���ʹ��CodeMemberEvent) 
            //�������(ʹ�� CodeAttributeDeclaration)
            //Customerclass.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
            //���ɴ���
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

