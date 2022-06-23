using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    #region //Variables
    public static LevelGrid instance { get; private set; }
    private GridSystem gridSystem = null;
    [SerializeField] private Transform gridDebugObjectPrefab = null;
    public event Action OnAnyUnitMove;
    #endregion


    #region //Monobehaviour
    void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else 
        {
            gridSystem = new GridSystem(10, 10, 2);
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            instance = this;
        }
    }
    #endregion

    #region //Unit adjustment
    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition).AddUnit(unit);
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
    {
        gridSystem.GetGridObject(gridPosition).RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(GridPosition oldPosition, GridPosition newPosition, Unit unit)
    {
        RemoveUnitAtGridPosition(oldPosition, unit);
        AddUnitAtGridPosition(newPosition, unit);
        OnAnyUnitMove?.Invoke();
    }
    #endregion

    #region //Validation
    public IEnumerable<GridPosition> CheckGridRange(GridPosition origin, int offset, bool circularRange = true)
    {
        for(int x = -offset; x <= offset; x++)
        {
            for(int z = -offset; z <= offset; z++)
            {
                GridPosition offsetPosition = new GridPosition(x,z);
                GridPosition testPosition = origin + offsetPosition;
                if(!LevelGrid.instance.IsValidPosition(testPosition)) continue;
                int testDistance = circularRange ? Mathf.Abs(x) + Mathf.Abs(z) : 0;
                if(testDistance > offset) continue;
                yield return testPosition;
            }
        }
    }

    public bool IsValidPosition(GridPosition gridPosition) => gridSystem.IsValidPosition(gridPosition);
    public bool HasAnyUnit(GridPosition gridPosition)
    {
        var obj = gridSystem.GetGridObject(gridPosition);
        return obj.HasAnyUnit();
    }
    #endregion

    #region //Getters
    public List<Unit> GetUnitsAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetUnits();
    public Unit GetUnitAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetUnit();
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);
    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();
    #endregion
}
