using System.Collections.Generic;

public class GridObject
{
    #region //Variables
    private GridSystem gridSystem = null;
    private GridPosition gridPosition = new GridPosition();
    private List<Unit> units = new List<Unit>();
    #endregion


    #region //Constructors
    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }
    #endregion

    #region //Unit adjustment
    public void AddUnit(Unit unit) => units.Add(unit);
    public void RemoveUnit(Unit unit) => units.Remove(unit);
    #endregion

    #region //Getters
    public override string ToString()
    {
        string unitString = "";
        foreach(var unit in units)
        {
            unitString += $"\n{unit}";
        }

        return $"{gridPosition.ToString()}{unitString}";
    }
    public List<Unit> GetUnits() => units;
    public Unit GetUnit() => units.Count > 0 ? units[0] : null;
    public bool HasAnyUnit() => units.Count > 0;
    #endregion
}
