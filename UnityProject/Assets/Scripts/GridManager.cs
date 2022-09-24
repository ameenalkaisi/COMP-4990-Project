using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using System.Linq;
using Unity.VisualScripting;

public class GridManager : MonoBehaviour
{
    public PathfindingVisual pathfindingVisual;
    public PathfindingDebugStepVisual pathfindingDebugVisual;

	public int width, height;
	public float cellSize;

    public int numberOfBlockedElements;

    private Pathfinding pathfinding;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();

        pathfinding = new Pathfinding(width, height, transform.position, cellSize);

        pathfindingDebugVisual.Setup(pathfinding.GetGrid());
        pathfindingVisual.SetGridMap(pathfinding.GetGrid());


        RandomizeBlockedElements(numberOfBlockedElements);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
            //Debug.Log(gridPos);

            pathfindingDebugVisual.ClearSnapshots();            
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

        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetCells();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            ResetCells();
            RandomizeBlockedElements(numberOfBlockedElements);
        }


    }

    // randomize blocked elements on the start, set averageDensity to a positive value to force algorithm to 
    // have different densities
    void RandomizeBlockedElements(int maxBlockedElements, float averageDensity = -1f)
    {
        List<Tuple<int, int>> remainingElements = new List<Tuple<int, int>>();
        GridMap<PathNode> grid = pathfinding.GetGrid();
        for (int i = 0; i < grid.GetWidth(); ++i)
            for (int j = 0; j < grid.GetHeight(); ++j)
                if(i != 0 || j != 0)
                    remainingElements.Add(new Tuple<int, int>(i, j));
        // remove (0, 0) from being picked

        if (averageDensity <= 0.0f)
        {
            // keep picking 2 elements until one of them is found not to be already picked
            // do this until specified maximum number of blocked elements
            for (int i = 0; i < maxBlockedElements; ++i) {
                int pickedI, pickedJ;
                Tuple<int, int> pickedPair;
                
                do
                {
                    pickedI = random.Next(0, grid.GetWidth());
                    pickedJ = random.Next(0, grid.GetHeight());
                    pickedPair = Tuple.Create<int, int>(pickedI, pickedJ);
                } while (!remainingElements.Contains(pickedPair) || remainingElements.Count == 0);

                grid.GetValue(pickedI, pickedJ).SetIsWalkable(false);
                Debug.Log(pickedI + ", " + pickedJ + " were set as non-walkable");
                remainingElements.Remove(pickedPair);
            }
        }



    }

    void ResetCells()
    {
        GridMap<PathNode> grid = pathfinding.GetGrid();
        for (int i = 0; i < grid.GetWidth(); ++i)
            for (int j = 0; j < grid.GetHeight(); ++j)
                grid.GetValue(i, j).SetIsWalkable(true);
    }


}
