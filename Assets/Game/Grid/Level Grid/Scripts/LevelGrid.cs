using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds the grid system for the current level
/// </summary>
public class LevelGrid : MonoBehaviour
{
    #region //Variables
    [SerializeField] private GridSystem<GridObject> gridSystemFactory = null;
    private GridSystem<GridObject> gridSystem;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        int width = gridSystemFactory.GetWidth();
        int height = gridSystemFactory.GetHeight();
        gridSystem = new GridSystem<GridObject>(width, height, GridGlobals.cellSize, 
                                (GridSystem<GridObject> g, GridCell gridCell) => new GridObject(g, gridCell));
    }
    #endregion

    #region //Validation
    public IEnumerable<GridCell> CheckGridRange(GridCell origin, int offset, bool circularRange = true, bool countSelf = false)
    {
        for(int x = -offset; x <= offset; x++)
        {
            for(int z = -offset; z <= offset; z++)
            {
                GridCell offsetCell = new GridCell(x,z);
                GridCell testCell = origin + offsetCell;
                if(!IsValidCell(testCell)) continue;
                if(!countSelf && testCell == origin) continue;
                int testDistance = circularRange ? Mathf.Abs(x) + Mathf.Abs(z) : 0;
                if(testDistance > offset) continue;
                yield return testCell;
            }
        }
    }

    public bool IsValidCell(GridCell gridCell) => gridSystem.IsValidCell(gridCell);
    #endregion

    #region //Getters
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public IEnumerable<GridCell> GetAllCells()
    {
        return gridSystem.GetGridCells();
    }

    public IEnumerable<GridCell> GetLine(GridCell origin, GridCell offset)
    {
        GridCell nextCell = origin;
        while(IsValidCell(nextCell) && !nextCell.HasHighObstacle())
        {
            if(!nextCell.HasObstacle()) yield return nextCell;
            nextCell += offset;
        }
    }
    #endregion
}