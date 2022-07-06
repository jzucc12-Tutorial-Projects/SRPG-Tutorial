

/// <summary>
/// Holds basic information about a grid cell
/// </summary>
public class GridObject
{
    #region //Variables
    private GridSystem<GridObject> gridSystem = null;
    private GridCell gridCell = new GridCell();
    #endregion


    #region //Constructors
    public GridObject(GridSystem<GridObject> gridSystem, GridCell gridCell)
    {
        this.gridSystem = gridSystem;
        this.gridCell = gridCell;
    }
    #endregion

    #region //Getters
    public override string ToString()
    {
        string unitString = "";
        foreach(var unit in gridCell.GetUnits())
        {
            unitString += $"\n{unit}";
        }

        return $"{gridCell.ToString()}{unitString}";
    }
    #endregion
}