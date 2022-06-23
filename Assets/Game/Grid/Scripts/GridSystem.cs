using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    #region //Variables
    private int width = 0;
    private int height = 0;
    private float cellSize = 0;
    private TGridObject[,] gridObjectArray;
    #endregion


    #region //Constructors
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridObjectArray = new TGridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                gridObjectArray[x,z] = createGridObject(this, new GridPosition(x, z));
            }
        }
    }
    #endregion

    #region //Validation
    public bool IsValidPosition(GridPosition gridPosition)
    {
        if(gridPosition.x < 0) return false;
        if(gridPosition.z < 0) return false;
        if(gridPosition.x >= width) return false;
        return gridPosition.z < height;
    }
    #endregion

    #region Getters
    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize));
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public int GetWidth() => width;
    public int GetHeight() => height;
    #endregion

    #region //Debug
    public void CreateDebugObjects(Transform container, Transform debugPrefab)
    {
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                var created = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                created.transform.parent = container;
                var debugObj = created.GetComponent<GridDebugObject>();
                debugObj.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    #endregion
}
