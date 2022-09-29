using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridMap<PathNode> grid;
    public int x, y;
    public int gCost, hCost, fCost;
    public bool isWalkable;

    public PathNode cameFromNode;

    public PathNode(GridMap<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;

        isWalkable = true;
    }

    public void AStarCalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;

        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString()
    {
        return x + "," + y + (isWalkable ? "" : "\nX");
    }

    public void DijkstrasCalculateFCost()
    {
        fCost = gCost; //+ hCost;
    }

}
