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
        pathfinding = new Pathfinding(10, 10, GetComponent<Transform>().position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
            Debug.Log(gridPos);
            List<PathNode> path = pathfinding.FindPath_AStar(0, 0, gridPos.x, gridPos.y);
            Debug.Log(path);
            if(path != null)
            {
                for(int i = 0; i < path.Count - 1; ++i)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f, new Vector3(path[i + 1].x, path[i + 1].y));
                }
            }


        }




    }
}
