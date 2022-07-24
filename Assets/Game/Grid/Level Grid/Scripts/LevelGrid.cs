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

    private void Start()
    {
        var cells = new List<GridCell>(gridSystem.GetGridCells());
        UpdateGrid(cells);
        foreach(var cell in gridSystem.GetGridCells())
        {
            var obj = gridSystem.GetGridObject(cell);
            obj.cantTarget = cell.CantTarget();

            if(cell.HasHighObstacle()) 
            {
                obj.hasObstacle = true;
                obj.hasHighObstacle = true;
            }
            else if(cell.HasObstacle())
                obj.hasObstacle = true;
        }
    }

    private void OnEnable()
    {
        BaseAction.OnAnyActionEndedEarly += UpdateGrid;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyActionEndedEarly -= UpdateGrid;
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

    public void UpdateGrid(List<GridCell> cells)
    {
        foreach(var cell in cells)
        {
            var obj = gridSystem.GetGridObject(cell);
            obj.ResetObject();
            obj.walkable = cell.IsWalkable();
            var objsInGrid = cell.GetAtCell();
            if(objsInGrid == null) continue;

            foreach(var objInGrid in objsInGrid)
            {
                if(objInGrid is Unit)
                    obj.myUnit = (Unit)objInGrid;
                if(objInGrid is ITargetable)
                    obj.targetable = (ITargetable)objInGrid;
                if(objInGrid is IInteractable)
                    obj.interactable = (IInteractable)objInGrid;
            }
        }
    }
    #endregion

    #region //Getters
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    public IEnumerable<GridCell> GetAllCells()
    {
        return gridSystem.GetGridCells();
    }

    public IEnumerable<GridCell> GetLine(GridCell origin, GridCell target)
    {
        if(target == origin) yield break;
        GridCell offset = target - origin;
        GridCell nextCell = origin + offset;
        while(IsValidCell(nextCell) && !nextCell.HasHighObstacle())
        {
            if(!nextCell.HasObstacle()) yield return nextCell;
            nextCell += offset;
        }
    }

    public bool HasAnyUnit(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).myUnit != null;
    }
    public Unit GetUnit(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).myUnit;
    }
    public IInteractable GetInteractable(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).interactable;
    }
    public ITargetable GetTargetable(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).targetable;
    }
    public bool HasObstacle(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).hasObstacle;
    }
    public bool HasHighObstacle(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).hasHighObstacle;
    }
    public bool CantTarget(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).cantTarget;
    }
    public bool IsWalkable(GridCell gridCell)
    {
        return gridSystem.GetGridObject(gridCell).walkable;
    }
    #endregion
}