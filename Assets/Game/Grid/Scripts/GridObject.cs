using System.Collections.Generic;

public class GridObject
{
    #region //Variables
    private GridSystem<GridObject> gridSystem = null;
    private GridPosition gridPosition = new GridPosition();
    private List<Unit> units = new List<Unit>();
    private IInteractable interactable = null;
    #endregion


    #region //Constructors
    public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
    }
    #endregion

    #region //Setters
    public void AddUnit(Unit unit) => units.Add(unit);
    public void RemoveUnit(Unit unit) => units.Remove(unit);
    public void SetInteractable(IInteractable interactable) => this.interactable = interactable;
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
    public IInteractable GetInteractable() => interactable;
    #endregion
}
