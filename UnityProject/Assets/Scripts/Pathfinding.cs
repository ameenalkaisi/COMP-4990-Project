using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;

    private GridMap<PathNode> grid;
    public List<PathNode> openList, closedList;

    public enum ALGORITHM_TYPES
    {
        ASTAR, DIJSKTRA, DEPTH_FIRST_SEARCH, BREADTH_FIRST_SEARCH
    }

    public Pathfinding(int width, int height, Vector3 originPosition, float cellSize = 10f)
    {
        grid = new GridMap<PathNode>(width, height, cellSize, originPosition,
            (GridMap<PathNode> g, int x, int y) => new PathNode(g, x, y));
    }

    public GridMap<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath_BreadthFirst(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        while(openList.Count != 0)
        {

            PathNode currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode)
                
                return CalculatePath(endNode);

            openList.Remove(currentNode);
            closedList.Add(currentNode);
        
        List<PathNode> neighbours = GetNeighbourList(currentNode);
        foreach (PathNode neighbour in neighbours)
            {
                if (closedList.Contains(neighbour) || !neighbour.isWalkable || openList.Contains(neighbour))
                    continue;

                neighbour.cameFromNode = currentNode;
                openList.Add(neighbour);
            }
        }
        return null;
    }

     public List<PathNode> FindPathWithSnapshots_BreadthFirst(int startX, int startY, int endX, int endY, PathfindingDebugStepVisual pathNodeStepVisual)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        while(openList.Count != 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            pathNodeStepVisual.TakeSnapshot(grid, currentNode, openList, closedList);

            if (currentNode == endNode)
            {
                List<PathNode> finalPath = CalculatePath(endNode);
                pathNodeStepVisual.TakeSnapshotFinalPath(grid, finalPath);
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

        
        List<PathNode> neighbours = GetNeighbourList(currentNode);
         foreach (PathNode neighbour in neighbours)
            {
                if (closedList.Contains(neighbour) || !neighbour.isWalkable || openList.Contains(neighbour))
                    continue;

                neighbour.cameFromNode = currentNode;
                openList.Add(neighbour);
            }
        }


        return null;
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

    public List<PathNode> FindPath_DepthFirst(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);


        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        Stack<PathNode> searchStack = new Stack<PathNode>();
        searchStack.Push(startNode);

        while(searchStack.Count > 0)
        {
            PathNode currentNode = searchStack.Pop();

            if (currentNode == endNode)
            {
                List<PathNode> finalPath = CalculatePath(endNode);
                return finalPath;
            }

            closedList.Add(currentNode);
            openList.Remove(currentNode);

            List<PathNode> neighbours = GetNeighbourList(currentNode);
            //neighbours.Reverse();

            // GetNeighbourList orders the list from left - up - right - bottom
            // find the first available node and go into it
            foreach (PathNode neighbour in neighbours)
            {
                if (closedList.Contains(neighbour) || !neighbour.isWalkable || openList.Contains(neighbour))
                    continue;

                neighbour.cameFromNode = currentNode;
                openList.Add(neighbour);
                searchStack.Push(neighbour);
            }
        }

        return null;

    }

    public List<PathNode> FindPathWithSnapshots_DepthFirst(int startX, int startY, int endX, int endY, PathfindingDebugStepVisual pathNodeStepVisual)
    {
        PathNode startNode = grid.GetValue(startX, startY);
        PathNode endNode = grid.GetValue(endX, endY);

        for(int i = 0; i < grid.GetWidth(); ++i)
        {
            for(int j = 0; j < grid.GetHeight(); ++j)
            {
                PathNode node = grid.GetValue(i, j);
                node.hCost = node.gCost = node.fCost = 0;
            }
        }

        openList = new List<PathNode>() { startNode };
        closedList = new List<PathNode>();

        Stack<PathNode> searchStack = new Stack<PathNode>();
        searchStack.Push(startNode);

        while(searchStack.Count > 0)
        {
            PathNode currentNode = searchStack.Pop();

            pathNodeStepVisual.TakeSnapshot(grid, currentNode, openList, closedList);

            if (currentNode == endNode)
            {
                List<PathNode> finalPath = CalculatePath(endNode);
                pathNodeStepVisual.TakeSnapshotFinalPath(grid, finalPath);
                return finalPath;
            }

            closedList.Add(currentNode);
            openList.Remove(currentNode);

            List<PathNode> neighbours = GetNeighbourList(currentNode);
           //neighbours.Reverse();

            // GetNeighbourList orders the list from left - up - right - bottom
            // find the first available node and go into it
            foreach (PathNode neighbour in neighbours)
            {
                if (closedList.Contains(neighbour) || !neighbour.isWalkable || openList.Contains(neighbour))
                    continue;

                neighbour.cameFromNode = currentNode;
                openList.Add(neighbour);
                searchStack.Push(neighbour);
            }
        }

        return null;
    }

    private Boolean isValidNode(int x, int y)
    {
        return x >= 0 && y >= 0 && x < grid.GetWidth() && y < grid.GetHeight();
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        // should be in order left, up, right, down
        if (isValidNode(currentNode.x - 1, currentNode.y))
            neighbourList.Add(grid.GetValue(currentNode.x - 1, currentNode.y));
        if (isValidNode(currentNode.x, currentNode.y + 1))
            neighbourList.Add(grid.GetValue(currentNode.x, currentNode.y + 1));
        if (isValidNode(currentNode.x + 1, currentNode.y))
            neighbourList.Add(grid.GetValue(currentNode.x + 1, currentNode.y));
        if (isValidNode(currentNode.x, currentNode.y - 1))
            neighbourList.Add(grid.GetValue(currentNode.x, currentNode.y - 1));

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
