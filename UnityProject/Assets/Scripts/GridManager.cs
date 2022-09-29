using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using System;
using System.Linq;
using Unity.VisualScripting;
using JetBrains.Annotations;

public class GridManager : MonoBehaviour
{
    public PathfindingVisual pathfindingVisual;
    public PathfindingDebugStepVisual pathfindingDebugVisual;

	public int width, height;
	public float cellSize;

    public float obstacleDensity;
    public double minObstacleDistribution, maxObstacleDistribution;

    public int maxTriesForRandomness = 1000;

    private Pathfinding pathfinding;
    private System.Random random;

    // Start is called before the first frame update
    void Start()
    {
        random = new System.Random();

        pathfinding = new Pathfinding(width, height, transform.position, cellSize);

        pathfindingDebugVisual.Setup(pathfinding.GetGrid());
        pathfindingVisual.SetGridMap(pathfinding.GetGrid());

        RandomizeBlockedElements(obstacleDensity, minObstacleDistribution, maxObstacleDistribution);
    }

    /*
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
                Vector2Int? gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
                //Debug.Log(gridPos);

                pathfindingDebugVisual.ClearSnapshots();
                List<PathNode> path;
                if(gridPos.HasValue)
                    path = pathfinding.FindPathWithSnapshots_AStar(0, 0, gridPos.Value.x, gridPos.Value.y, pathfindingDebugVisual);

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
            } 
        }*/

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int? gridPos = pathfinding.GetGrid().GetLocalPosition(mouseWPos);
            //Debug.Log(gridPos);

            pathfindingDebugVisual.ClearSnapshots();
            List<PathNode> path;
            if(gridPos.HasValue)
                path = pathfinding.FindPathWithSnapshots_Dijkstras(0, 0, gridPos.Value.x, gridPos.Value.y, pathfindingDebugVisual);
            
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

            // if it is a valid node and not default constructed node, set is walkable
            if(focusedNode != null)
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
            RandomizeBlockedElements(obstacleDensity, minObstacleDistribution, maxObstacleDistribution);
        }


    }

    // randomize blocked elements on the start, set averageDensity to a positive value to force algorithm to 
    // have different densities
    void RandomizeBlockedElements(float obstacleDensity, double minObstacleDistribution = 0f, double maxObstacleDistribution = Double.MaxValue)
    {
        List<Tuple<int, int>> remainingElements = new List<Tuple<int, int>>();
        GridMap<PathNode> grid = pathfinding.GetGrid();

        int maxTries = maxTriesForRandomness;
        double calculatedObstacleDistribution;
        do
        {
            remainingElements.Clear();
            ResetCells();
            // remove (0, 0) from being picked
            for (int i = 0; i < grid.GetWidth(); ++i)
                for (int j = 0; j < grid.GetHeight(); ++j)
                    if (i != 0 || j != 0)
                        remainingElements.Add(new Tuple<int, int>(i, j));

            // keep picking 2 elements until one of them is found not to be already picked
            // do this until specified maximum number of blocked elements
            int blockedElementsCount = (int)Math.Floor(obstacleDensity * grid.GetWidth() * grid.GetHeight());
            for (int i = 0; i < blockedElementsCount; ++i)
            {
                int pickedI, pickedJ;
                Tuple<int, int> pickedPair;

                do
                {
                    pickedI = random.Next(0, grid.GetWidth());
                    pickedJ = random.Next(0, grid.GetHeight());
                    pickedPair = Tuple.Create<int, int>(pickedI, pickedJ);
                } while (!remainingElements.Contains(pickedPair) || remainingElements.Count == 0);

                grid.GetValue(pickedI, pickedJ).SetIsWalkable(false);
                remainingElements.Remove(pickedPair);
            }

            // calculate threshold
            calculatedObstacleDistribution = CalculateObstacleDistribution();
        } while ((minObstacleDistribution > calculatedObstacleDistribution 
                    || calculatedObstacleDistribution > maxObstacleDistribution)
                    //|| pathfinding.FindPath_AStar(0, 0, grid.GetWidth() - 1, grid.GetHeight() - 1) == null)
                && --maxTries > 0);

        Debug.Log(calculatedObstacleDistribution);
    }

    public List<PathNode> GetObstacleList()
    {
        List<PathNode> obstacleList = new List<PathNode>();

        GridMap<PathNode> grid = pathfinding.GetGrid();
        for (int i = 0; i < grid.GetWidth(); ++i)
        {
            for(int j = 0; j < grid.GetHeight(); ++j)
            {
                PathNode curElement = grid.GetValue(i, j);
                if (!curElement.isWalkable)
                    obstacleList.Add(curElement);
            }
        }

        return obstacleList;
    }

    public double CalculateObstacleDistribution()
    {
        // for each obstacle, loop over other obstacles
        List<PathNode> obstacleList = GetObstacleList();

        double sumDistances = 0f;
        for(int i = 0; i < obstacleList.Count; ++i)
        {
            for(int j = i; j < obstacleList.Count; ++j)
            {
                PathNode obstacleI = obstacleList[i];
                PathNode obstacleJ = obstacleList[j];

                sumDistances += Math.Sqrt(Math.Pow(obstacleI.x - obstacleJ.x, 2) + Math.Pow(obstacleI.y - obstacleJ.y, 2));
            }
        }

        return sumDistances / obstacleList.Count();
    }

    void ResetCells()
    {
        GridMap<PathNode> grid = pathfinding.GetGrid();
        for (int i = 0; i < grid.GetWidth(); ++i)
            for (int j = 0; j < grid.GetHeight(); ++j)
                grid.GetValue(i, j).SetIsWalkable(true);
    }


}
