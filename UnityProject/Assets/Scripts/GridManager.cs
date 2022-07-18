using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class GridManager : MonoBehaviour
{
	private Grid<int> grid;
	private Transform objectTransform;

	public int width, height;
	public float cellSize;

    // Start is called before the first frame update
    void Start()
    {
		objectTransform = GetComponent<Transform>();
		grid = new Grid<int>(width, height, cellSize, objectTransform.position);
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetMouseButtonDown(0))
			grid.SetValue(UtilsClass.GetMouseWorldPosition(), 10);
		else if(Input.GetMouseButtonDown(1))
			Debug.Log(grid.GetValue(UtilsClass.GetMouseWorldPosition()));
    }
}
