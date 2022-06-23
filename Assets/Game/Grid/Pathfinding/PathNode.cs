public class PathNode
{
    #region // Variables
    private GridPosition gridPosition = new GridPosition();
    private PathNode previousNode = null;
    private int gCost = 0;
    private int hCost = 0;
    private bool isWalkable = true;
    #endregion

    #region //Constructor
    public PathNode(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }
    #endregion

    #region //Setters
    public void SetPreviousNode(PathNode node) => previousNode = node;
    public void ResetPreviousNode() => previousNode = null;
    public bool SetIsWalkable(bool walkable) => isWalkable = walkable;
    public void SetGCost(int cost) => gCost = cost;
    public void SetHCost(int cost) => hCost = cost;
    #endregion

    #region //Getters
    public PathNode GetPreviousNode() => previousNode;
    public GridPosition GetGridPosition() => gridPosition;
    public bool IsWalkable() => isWalkable;
    public int GetFCost() => gCost + hCost;
    public int GetGCost() => gCost;
    public int GetHCost() => hCost;
    public override string ToString()
    {
        return gridPosition.ToString();
    }
    #endregion
}
