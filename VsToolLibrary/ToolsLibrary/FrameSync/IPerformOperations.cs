using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsLibrary.FrameSync
{
    //需要执行操作的对象需要实现这个接口
    public interface IPerformOperations
    {
        void OnPerformOperation(SyncDataBase data);
    }
}
