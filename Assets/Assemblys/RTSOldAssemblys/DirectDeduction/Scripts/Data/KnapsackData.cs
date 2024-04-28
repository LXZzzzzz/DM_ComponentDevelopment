using System.Collections.Generic;

namespace DefaultRole
{
    public class ToolData
    {
        public string ToolId="";
        public string KnapSourceId="";
        public int KnapId;

        public ToolData()
        { }
        public ToolData(string str)
        {
            ToData(str);
        }
        public override string ToString()
        {
            return ToolId + "*" + KnapSourceId + "*" + KnapId;
        }
        public void ToData(string str)
        {
            string[] strs = str.Split('*');
            ToolId = strs[0];
            KnapSourceId = strs[1];
            KnapId =int.Parse(strs[2]);
        }
    }
    public class KnapsackData : DataBase
    {
        private List<ToolData> tools = new List<ToolData>();

        public void AddTool(ToolData tool)
        {
            tools.Add(tool);
            OnDataChanged("Add:"+tool.ToString());
        }
        public void RemoveTool(string toolId)
        {
            ToolData item=tools.Find(i => i.ToolId == toolId);
            tools.Remove(item);
            OnDataChanged("Remove:"+toolId);
        }
        public ToolData GetToolById(string toolId)
        {
            return tools.Find(i=>i.ToolId==toolId);
        }
        public int GetToolCount()
        {
            return tools.Count;
        }
        public override string ToString()
        {
            string data = "";
            for (int i = 0; i < tools.Count-1; i++)
            {
                data += tools[i].ToString();
                data += "_";
            }
            if(tools.Count>0)
               data += tools[tools.Count - 1].ToString();
            return data;
        }
        public override void ToData(string str)
        {
            if (string.IsNullOrEmpty(str)) return;
            string[] strs = str.Split('_');
            if (tools.Count < strs.Length) //增加物品项
            {
                for (int i = 0; i < strs.Length; i++)
                {
                    ToolData newTool = new ToolData(strs[i]);
                    if (tools.Find(k => k.KnapId == newTool.KnapId) == null)
                        AddTool(newTool);
                }
            }
            if (tools.Count > strs.Length)  //放下物品项
            {
                for (int i = 0; i < tools.Count; i++)
                {
                    if (!str.Contains(tools[i].ToString()))
                        RemoveTool(tools[i].ToolId);
                }
            }
        }
    }
}

