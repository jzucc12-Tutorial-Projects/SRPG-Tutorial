using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds an array of grid objects
/// </summary>
/// <typeparam name="TGridObject"></typeparam>
[Serializable]
public class GridSystem<TGridObject>
{
    #region //Variables
    [SerializeField] private int width = 0;
    [SerializeField] private int height = 0;
    private float cellSize = 0;
    private TGridObject[,] gridObjectArray;
    #endregion


    #region //Constructors
    public GridSystem() { }

    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridCell, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridObjectArray = new TGridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                gridObjectArray[x,z] = createGridObject(this, new GridCell(x, z));
            }
        }
    }
    #endregion

    #region //Validation
    public bool IsValidCell(GridCell gridCell)
    {
        if(gridCell.x < 0) return false;
        if(gridCell.z < 0) return false;
        if(gridCell.x >= width) return false;
        return gridCell.z < height;
    }
    #endregion

    #region Getters
    public TGridObject GetGridObject(GridCell gridCell)
    {
        return gridObjectArray[gridCell.x, gridCell.z];
    }

    public int GetWidth() => width;
    public int GetHeight() => height;

    public IEnumerable<GridCell> GetGridCells()
    {
        for(int x = 0; x < GetWidth(); x++)
        {
            for(int z = 0; z < GetHeight(); z++)
            {
                yield return new GridCell(x,z);
            }
        }
    }
    #endregion

    #region //Debug
    public void CreateDebugObjects(Transform container, Transform debugPrefab)
    {
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridCell gridCell = new GridCell(x,z);
                var created = GameObject.Instantiate(debugPrefab, gridCell.GetWorldPosition(), Quaternion.identity);
                created.transform.parent = container;
                var debugObj = created.GetComponent<GridDebugObject>();
                debugObj.SetGridObject(GetGridObject(gridCell));
            }
        }
    }
    #endregion
}
