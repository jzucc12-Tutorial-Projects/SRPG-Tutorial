

/// <summary>
/// Holds basic information about a grid cell
/// </summary>
public class GridObject
{
    #region //Variables
    private GridSystem<GridObject> gridSystem = null;
    private GridCell gridCell = new GridCell();
    public Unit myUnit = null;
    public ITargetable targetable = null;
    public IInteractable interactable = null;
    public bool hasObstacle = false;
    public bool hasHighObstacle = false;
    public bool cantTarget = false;
    public bool walkable = true;
    #endregion


    #region //Constructors
    public GridObject(GridSystem<GridObject> gridSystem, GridCell gridCell)
    {
        this.gridSystem = gridSystem;
        this.gridCell = gridCell;
    }
    #endregion

    #region //Setters
    public void ResetObject()
    {
        myUnit = null;
        targetable = null;
        interactable = null;
        walkable = true;
    }
    #endregion
}