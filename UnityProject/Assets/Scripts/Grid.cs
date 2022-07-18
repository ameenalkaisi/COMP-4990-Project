using UnityEngine;
using CodeMonkey.Utils;

// grid simply does the actions on the Grid
// creation still on the developer
// this class draws a grid, and lets you place gameobjects inside of it
public class Grid<T>
{
    private int width, height;
    private T[,] gridArray;

    private Vector3 originPosition;

    private float cellSize;

    private TextMesh[,] debugTextArray;

    // creates the grid and draws it
    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); ++x)
        {
            for (int y = 0; y < gridArray.GetLength(1); ++y)
            {
				// comment below out when actually deploying this is just for debugging
                debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(),
                        null,
                        GetWorlPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f,
                        10, Color.white, TextAnchor.MiddleCenter);

				// these draw the borders
				// not permanent however
                Debug.DrawLine(GetWorlPosition(x, y), GetWorlPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorlPosition(x, y), GetWorlPosition(x + 1, y), Color.white, 100f);
            }
			// draw borders at the end to reduce drawing a line twice on edges
            Debug.DrawLine(GetWorlPosition(0, height), GetWorlPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorlPosition(width, 0), GetWorlPosition(width, height), Color.white, 100f);
        }
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int convertedPos = GetLocalPosition(worldPosition);

        SetValue(convertedPos.x, convertedPos.y, value);
    }

	// will throw exception if out of bounds
    public T GetValue(int x, int y)
    {
		return gridArray[x, y];
    }

    public T GetValue(Vector3 worldPosition)
    {
        Vector2Int convertedPos = GetLocalPosition(worldPosition);
        return GetValue(convertedPos.x, convertedPos.y);
    }

    private Vector2Int GetLocalPosition(Vector3 worldPosition)
    {
        Vector2Int result = new Vector2Int();
        result.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        result.y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

        return result;
    }

    private Vector3 GetWorlPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }
}
