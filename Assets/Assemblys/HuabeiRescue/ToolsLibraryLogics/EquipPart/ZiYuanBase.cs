using System.Collections.Generic;

namespace ToolsLibrary.EquipPart_Logic
{
    public abstract class ZiYuanBase : DMonoBehaviour
    {
        private List<string> _beUsedCommanderIds;

        public List<string> beUsedCommanderIds => _beUsedCommanderIds;

        public void SetBeUsedComs(List<string> data)
        {
            _beUsedCommanderIds = data;
        }
    }
}