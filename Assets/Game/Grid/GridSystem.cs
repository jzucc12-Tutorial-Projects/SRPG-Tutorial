using UnityEngine;

public class GridSystem
{
    #region //Variables
    private int width = 0;
    private int height = 0;
    private float cellSize = 0;
    private GridObject[,] gridObjectArray;
    #endregion


    #region //Constructors
    public GridSystem(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridObjectArray = new GridObject[width, height];

        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                gridObjectArray[x,z] = new GridObject(this, new GridPosition(x, z));
            }
        }
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

    public GridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }
    #endregion

    #region //Debug
    public void CreateDebugObjects(Transform debugPrefab)
    {
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x,z);
                var created = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                var debugObj = created.GetComponent<GridDebugObject>();
                debugObj.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }
    #endregion
}
