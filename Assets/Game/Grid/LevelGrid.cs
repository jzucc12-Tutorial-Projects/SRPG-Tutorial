using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    #region //Variables
    public static LevelGrid instance { get; private set; }
    private GridSystem gridSystem = null;
    [SerializeField] private Transform gridDebugObjectPrefab = null;
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
    }
    #endregion

    #region //Getters
    public List<Unit> GetUnitsAtGridPosition(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).GetUnits();
    public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);
    #endregion
}
