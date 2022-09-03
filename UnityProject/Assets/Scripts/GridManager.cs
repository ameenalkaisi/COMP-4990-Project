using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridManager : MonoBehaviour
{
    public PathfindingVisual pathfindingVisual;
    public PathfindingDebugStepVisual pathfindingDebugVisual;

	public int width, height;
	public float cellSize;

    private Pathfinding pathfinding;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(width, height, transform.position, cellSize);

        pathfindingDebugVisual.Setup(pathfinding.GetGrid());
        pathfindingVisual.SetGridMap(pathfinding.GetGrid());

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
            //Debug.Log(gridPos);
            
            List<PathNode> path = pathfinding.FindPathWithSnapshots_AStar(0, 0, gridPos.x, gridPos.y, pathfindingDebugVisual);
            
            /*Debug.Log(path.Count);
            foreach(PathNode node in path)
            {
                Debug.Log(node);
            }*/
            /* if(path != null)
            {
                Color lineColor = new Color(Random.Range(0, 100) / 100f, Random.Range(0, 100) / 100f, Random.Range(0, 100) / 100f);

                // get a random color only for this path, so that differentiating betweeen other paths is easier
                for(int i = 0; i < path.Count - 1; ++i)
                {
                    Vector3 firstPosition = pathfinding.GetGrid().GetWorldPosition(path[i].x, path[i].y);
                    Vector3 secondPosition = pathfinding.GetGrid().GetWorldPosition(path[i + 1].x, path[i + 1].y);
                    Debug.DrawLine(firstPosition + Vector3.one * cellSize / 2f, secondPosition + Vector3.one * cellSize / 2f, lineColor, 100f);
                }
            } */

        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            PathNode focusedNode = pathfinding.GetGrid().GetValue(mouseWPos);
            focusedNode.SetIsWalkable(!focusedNode.isWalkable);
            //pathfinding.GetGrid().DrawDebug();

            // perhaps in here, set pathfindingVisual's properties
            // let pathfindingVisual do all the mesh related work
        }




    }
}
