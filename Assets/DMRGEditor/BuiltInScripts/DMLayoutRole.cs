using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimuEvent
{
    public int EventId;
    public string Param;
}
public class DMLayoutRole : MonoBehaviour
{
    public int Status = -1;
    public int Animation = -1;
    public int Skin = -1;
    public bool RunModeInInner = true;
    public List<SimuEvent> Events = new List<SimuEvent>();
    [HideInInspector]
    public string OccupiedId;
}
