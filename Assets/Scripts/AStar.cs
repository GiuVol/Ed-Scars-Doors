using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AStar : MonoBehaviour
{
    /// <summary>
    /// Attribute <c>_aStar</c>
    /// Stores the value that refer to GameObject AStar
    /// </summary>
    public AstarPath _aStar;
    
    void Start()
    {
        MobAI.SetExtremeGraph(_aStar.astarData.gridGraph.center.x, _aStar.astarData.gridGraph.width);
    }
}
