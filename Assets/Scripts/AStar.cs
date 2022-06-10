using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class AStar : MonoBehaviour
{
    public AstarPath _aStar;
    // Start is called before the first frame update
    void Start()
    {
        MobAI.SetExtremeGraph(_aStar.astarData.gridGraph.center.x, _aStar.astarData.gridGraph.width);
    }
}
