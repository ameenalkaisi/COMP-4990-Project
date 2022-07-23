using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridManager : MonoBehaviour
{
    private Pathfinding pathfinding;

	public int width, height;
	public float cellSize;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(10, 10, GetComponent<Transform>().position, 1.3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
            //Debug.Log(gridPos);
            
            List<PathNode> path = pathfinding.FindPath_AStar(0, 0, gridPos.x, gridPos.y);
            
            /*Debug.Log(path.Count);
            foreach(PathNode node in path)
            {
                Debug.Log(node);
            }*/
            if(path != null)
            {
                Color lineColor = new Color(Random.Range(0, 100) / 100f, Random.Range(0, 100) / 100f, Random.Range(0, 100) / 100f);
                float cellSize = pathfinding.GetGrid().GetCellSize();

                // get a random color only for this path, so that differentiating betweeen other paths is easier
                for(int i = 0; i < path.Count - 1; ++i)
                {
                    Vector3 firstPosition = pathfinding.GetGrid().GetWorlPosition(path[i].x, path[i].y);
                    Vector3 secondPosition = pathfinding.GetGrid().GetWorlPosition(path[i + 1].x, path[i + 1].y);
                    Debug.DrawLine(firstPosition + Vector3.one * cellSize / 2f, secondPosition + Vector3.one * cellSize / 2f, lineColor, 100f);
                }
            }


        }




    }
}
