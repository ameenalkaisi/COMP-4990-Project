using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;

    private GridMap<PathNode> grid;
    private List<PathNode> openList, closedList;

    public Pathfinding(int width, int height, Vector3 originPosition, float cellSize = 10f)
    {
        grid = new GridMap<PathNode>(width, height, cellSize, originPosition,
            (GridMap<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public GridMap<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath_Dijkstras(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

         // set up g and f values
        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.DijkstrasCalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        //startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.DijkstrasCalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
                return CalculatePath(endNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;

                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost + CalculateDistanceCost(neighbourNode, startNode); 
                    //neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.DijkstrasCalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        // out of nodes on open list, no path exists
        return null;
    }

    public List<PathNode> FindPathWithSnapshots_Dijkstras(int startX, int startY, int endX, int endY, PathfindingDebugStepVisual pathNodeStepVisual) 
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();
    
        // set up g and f values
        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.DijkstrasCalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        //startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.DijkstrasCalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                List<PathNode> finalPath = CalculatePath(endNode);
                pathNodeStepVisual.TakeSnapshotFinalPath(grid, finalPath);

                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            pathNodeStepVisual.TakeSnapshot(grid, currentNode, openList, closedList);
            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    //neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.DijkstrasCalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        // out of nodes on open list, no path exists
        return null;
    }

    public List<PathNode> FindPath_AStar(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();
    
        // set up g and f values
        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.AStarCalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.AStarCalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
                return CalculatePath(endNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.AStarCalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        // out of nodes on open list, no path exists
        return null;
    }

    public List<PathNode> FindPathWithSnapshots_AStar(int startX, int startY, int endX, int endY, PathfindingDebugStepVisual pathNodeStepVisual)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();
    
        // set up g and f values
        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetValue(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.AStarCalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.AStarCalculateFCost();
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                List<PathNode> finalPath = CalculatePath(endNode);
                pathNodeStepVisual.TakeSnapshotFinalPath(grid, finalPath);

                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            pathNodeStepVisual.TakeSnapshot(grid, currentNode, openList, closedList);

            foreach(PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;
                if(!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.AStarCalculateFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        // out of nodes on open list, no path exists
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();
        for(int i = -1; i <= 1; i += 2)
        {
            if (currentNode.x + i >= 0 && currentNode.x + i < grid.GetWidth())
                neighbourList.Add(grid.GetValue(currentNode.x + i, currentNode.y));

            if (currentNode.y + i >= 0 && currentNode.y + i < grid.GetHeight())
                neighbourList.Add(grid.GetValue(currentNode.x, currentNode.y + i));
        }

        return neighbourList;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while(currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }

        path.Reverse();
        return path;
    }

    /*private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);

        return MOVE_STRAIGHT_COST * Mathf.Abs(xDistance - yDistance);
    }*/

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        return Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2)));
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for(int i = 1; i < pathNodeList.Count; ++i)
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathNodeList[i];

        return lowestFCostNode;
    }
}
