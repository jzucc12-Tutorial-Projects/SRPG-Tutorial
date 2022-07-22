
/// <summary>
/// Holds all pathfinding information for a grid cell
/// </summary>
public class PathNode
{
    #region // Variables
    private GridCell gridCell = new GridCell();
    private PathNode previousNode = null;
    private int gCost = 0;
    private int hCost = 0;
    public bool walkable = false;
    #endregion


    #region //Constructor
    public PathNode(GridCell gridCell)
    {
        this.gridCell = gridCell;
    }
    #endregion

    #region //Setters
    public void SetPreviousNode(PathNode node) => previousNode = node;
    public void ResetPreviousNode() => previousNode = null;
    public void SetGCost(int cost) => gCost = cost;
    public void SetHCost(int cost) => hCost = cost;
    #endregion

    #region //Getters
    public bool IsWalkable() => walkable;
    public PathNode GetPreviousNode() => previousNode;
    public GridCell GetGridCell() => gridCell;
    public int GetFCost() => gCost + hCost;
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public override string ToString()
    {
        return gridCell.ToString();
    }
    #endregion
}
