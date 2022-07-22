using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridMap<PathNode> grid;
    public int x, y;
    public int gCost, hCost, fCost;

    public PathNode cameFromNode;

    public PathNode(GridMap<PathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public override string ToString()
    {
        return x + "," + y;
    }
}
