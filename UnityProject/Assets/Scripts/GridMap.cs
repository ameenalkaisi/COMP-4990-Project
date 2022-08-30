using UnityEngine;
using CodeMonkey.Utils;
using System;

// grid simply does the actions on the Grid
// creation still on the developer
// this class draws a grid, and lets you place gameobjects inside of it
public class GridMap<T>
{
    private int width, height;
    private T[,] gridArray;

    private Vector3 originPosition;

    private float cellSize;

    private TextMesh[,] debugTextArray;

    public bool showDebug = true;
    public int fontSize = 36;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int y;
    }

    public GridMap(int width, int height, float cellSize, Vector3 originPosition, Func<GridMap<T>, int, int, T> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new T[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }

        // output X under it if it is not a walkable cell
        DrawDebug();
    }

    public void TriggerGridObjectChanged(int x, int y)
    {
        OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
    }

    public void DrawDebug()
    {
        if (!showDebug)
            return;

        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < gridArray.GetLength(0); ++x)
        {
            for (int y = 0; y < gridArray.GetLength(1); ++y)
            {
                // attaches a piece of text to each cell
                debugTextArray[x, y] = UtilsClass.CreateWorldText(gridArray[x, y].ToString(),
                        null,
                        GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) * 0.5f,
                        fontSize, Color.white, TextAnchor.MiddleCenter);

                // these draw the borders
                // not permanent however
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

                OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
                {
                    debugTextArray[eventArgs.x, eventArgs.y].text = gridArray[eventArgs.x, eventArgs.y].ToString();
                };
            }
            // draw borders at the end to reduce drawing a line twice on edges
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    public void SetValue(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();

            TriggerGridObjectChanged(x, y);
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

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public Vector2Int GetLocalPosition(Vector3 worldPosition)
    {
        Vector2Int result = new Vector2Int();
        result.x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        result.y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);

        if (result.x >= 0 && result.y >= 0 && result.x < width && result.y < height)
            return result;
        else return new Vector2Int(0, 0); // temporary should probably throw exception or something
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }
}
