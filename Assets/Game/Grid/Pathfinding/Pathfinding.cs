using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles pathfinding for the current level
/// </summary>
public class Pathfinding : MonoBehaviour
{
    #region //Grid Variables
    [SerializeField] private bool debugMode = false;
    private GridSystem<PathNode> gridSystem;
    #endregion

    #region //Pathfinding variables
    [SerializeField] private Transform pathNodePrefab = null;
    private const int COST_MULT = 10;
    private static readonly int STRAIGHT_COST = 1 * COST_MULT;
    private static readonly int DIAG_COST = (int)(1.4f * COST_MULT);
    #endregion


    #region //Monobehaviour
    private void Start()
    {
        var levelGrid = FindObjectOfType<LevelGrid>();
        int width = levelGrid.GetWidth();
        int height = levelGrid.GetHeight();
        gridSystem = new GridSystem<PathNode>(width, height, GridGlobals.cellSize, (GridSystem<PathNode> g, GridCell cell) => new PathNode(cell));
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridCell gridCell = new GridCell(x, z);
                Vector3 worldPosition = gridCell.GetWorldPosition();
                float offset = 5;
                var hit = Physics.Raycast(worldPosition + Vector3.down * offset, Vector3.up, 2 * offset, GridGlobals.obstacleMask);
            }
        }
        if(debugMode) gridSystem.CreateDebugObjects(transform, pathNodePrefab);
    }
    #endregion

    #region //Pathfinding
    public bool HasWalkablePath(GridCell startCell, GridCell endCell)
    {
        return FindPath(startCell, endCell, out int pathLength) != null;
    }

    public List<GridCell> FindPath(GridCell startCell, GridCell endCell, out int pathLength)
    {
        //Reset pathnodes
        foreach(var cell in gridSystem.GetGridCells())
        {
            PathNode pathNode = gridSystem.GetGridObject(cell);
            pathNode.SetGCost(int.MaxValue);
            pathNode.SetHCost(0);
            pathNode.ResetPreviousNode();
        }

        List<PathNode> openList = new List<PathNode>(); //Nodes open to searching
        List<PathNode> closeList = new List<PathNode>(); //Nodes closed to searching
        PathNode startNode = gridSystem.GetGridObject(startCell);
        PathNode endNode = gridSystem.GetGridObject(endCell);

        openList.Add(startNode);
        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startCell, endCell));

        //Search routine
        while(openList.Count > 0)
        {
            //Get next node
            PathNode currentNode = GetLowestFCost(openList);
            if(currentNode == endNode) 
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closeList.Remove(currentNode);

            //Determine valid neighbors
            foreach(var neighbor in GetNeighbors(currentNode))
            {
                if(closeList.Contains(neighbor)) continue;
                if(!neighbor.IsWalkable())
                {
                    closeList.Add(neighbor);
                    continue;
                }

                //Only set as neighbor if their score is lower than the current node
                int tentativeCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridCell(), neighbor.GetGridCell());
                if(tentativeCost < neighbor.GetGCost()) 
                {
                    neighbor.SetPreviousNode(currentNode);
                    neighbor.SetGCost(tentativeCost);
                    neighbor.SetHCost(CalculateDistance(neighbor.GetGridCell(), endCell));
                    if(!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }

        //No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridCell startCell, GridCell endCell)
    {
        GridCell gridDistance = startCell - endCell;
        int xDistance = Mathf.Abs(gridDistance.x);
        int zDistance =  Mathf.Abs(gridDistance.z);
        int totalDistance = xDistance + zDistance;
        // int remainder = Mathf.Abs(xDistance - zDistance); //COUNTS DIAGONAL AS 1 UNIT
        // return DIAG_COST * Mathf.Min(xDistance, zDistance) + remainder * STRAIGHT_COST; //COUNTS DIAGONAL AS 1 UNIT
        return totalDistance * STRAIGHT_COST; //COUNTS DIAGONAL AS 2 UNITS
    }

    private PathNode GetLowestFCost(List<PathNode> pathNodeList)
    {
        PathNode lowestFCost = pathNodeList[0];
        foreach(var node in pathNodeList)
        {
            if(node.GetFCost() >= lowestFCost.GetFCost()) continue;
            lowestFCost = node;
        }
        return lowestFCost;
    }
    
    private List<PathNode> GetNeighbors(PathNode pathNode)
    {
        List<PathNode> neighbors = new List<PathNode>();
        GridCell cell = pathNode.GetGridCell();
        
        //Straight paths
        neighbors.Add(GetNode(cell.x - 1, cell.z));
        neighbors.Add(GetNode(cell.x + 1, cell.z));
        neighbors.Add(GetNode(cell.x, cell.z - 1));
        neighbors.Add(GetNode(cell.x, cell.z + 1));

        //Diagonal paths
        neighbors.Add(GetNode(cell.x - 1, cell.z + 1));
        neighbors.Add(GetNode(cell.x + 1, cell.z - 1));
        neighbors.Add(GetNode(cell.x - 1, cell.z - 1));
        neighbors.Add(GetNode(cell.x + 1, cell.z + 1));

        neighbors.RemoveAll(x => x == null);
        return neighbors;
    }

    private PathNode GetNode(int x, int z)
    {
        if(x < 0 || x >= gridSystem.GetWidth()) return null;
        if(z < 0 || z >= gridSystem.GetHeight()) return null;
        return gridSystem.GetGridObject(new GridCell(x, z));
    }
    
    private List<GridCell> CalculatePath(PathNode endNode)
    {
        List<GridCell> cells = new List<GridCell>();
        PathNode currentNode = endNode;
        while(currentNode.GetPreviousNode() != null)
        {
            cells.Add(currentNode.GetGridCell());
            currentNode = currentNode.GetPreviousNode();
        }

        cells.Reverse();
        return cells;
    }
    #endregion

    #region //Getters
    public int GetPathLength(GridCell startCell, GridCell endCell)
    {
        FindPath(startCell, endCell, out int pathLength);
        return pathLength / COST_MULT;
    } 
    #endregion
}