using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointIconCell : IconCellBase
{
    //当前实体点的本地Id
    private string pointLocalId;
    
    
    //记录所有经过我的点
    private List<string> _allViaPointIds;

    public List<string> allViaPointIds => _allViaPointIds;
    
    //todo:这里考虑错了，该功能在取水点、补给点 等 都需要有，但是当前不需要做路径规划，暂时先这样，后期如果需要再看是否移动回去还是让其他类型点继承我
    public void AddAttachedPoint(string pointId)
    {
        if (_allViaPointIds == null)
            _allViaPointIds = new List<string>();
        _allViaPointIds.Add(pointId);
    }

    protected override IconInfoData GetBasicInfo()
    {
        return null;
    }
}