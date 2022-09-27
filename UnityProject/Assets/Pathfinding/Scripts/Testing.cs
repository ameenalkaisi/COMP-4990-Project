﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Testing : MonoBehaviour {
    
    [SerializeField] private PathfindingDebugStepVisual pathfindingDebugStepVisual;
    [SerializeField] private PathfindingVisual pathfindingVisual;
    private Pathfinding pathfinding;

    private void Start() {
        pathfinding = new Pathfinding(20, 10, GetComponent<Transform>().position);
        pathfindingDebugStepVisual.Setup(pathfinding.GetGrid());
        pathfindingVisual.SetGridMap(pathfinding.GetGrid());
    }

    private void Update() {
        /*if (Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Vector2Int localPos = pathfinding.GetGrid().GetLocalPosition(mouseWorldPosition);
            int x = localPos.x;
            int y = localPos.y;
            List<PathNode> path = pathfinding.FindPath_AStar(0, 0, x, y);
            if (path != null) {
                for (int i=0; i<path.Count - 1; i++) {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i+1].x, path[i+1].y) * 10f + Vector3.one * 5f, Color.green, 5f);
                }
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            Vector2Int localPos = pathfinding.GetGrid().GetLocalPosition(mouseWorldPosition);
            pathfinding.GetGrid().GetValue(localPos.x, localPos.y).SetIsWalkable(!pathfinding.GetGrid().GetValue(localPos.x, localPos.y).isWalkable);
        }*/
    }

}
