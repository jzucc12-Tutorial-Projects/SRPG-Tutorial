using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding instance = null;

    #region //Grid Variables
    [SerializeField] private bool debugMode = false;
    [SerializeField] private LayerMask obstacleLayer = 0;
    private GridSystem<PathNode> gridSystem;
    private int width = 10;
    private int height = 10;
    private float cellSize = 2;
    #endregion

    #region //Pathfinding variables
    [SerializeField] private Transform pathNodePrefab = null;
    private const int STRAIGHT_COST = 10;
    private const int DIAG_COST = 14;
    private const int COST_MULT = 10;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        width = LevelGrid.instance.GetWidth();
        height = LevelGrid.instance.GetHeight();
        cellSize = LevelConstants.cellSize;
        gridSystem = new GridSystem<PathNode>(width, height, cellSize, (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.instance.GetWorldPosition(gridPosition);
                float offset = 5;
                var hit = Physics.Raycast(worldPosition + Vector3.down * offset, Vector3.up, 2 * offset, obstacleLayer);
                if(hit) GetNode(x,z).SetIsWalkable(false);
            }
        }
        if(debugMode) gridSystem.CreateDebugObjects(transform, pathNodePrefab);
    }
    #endregion

    #region //Pathfinding
    public List<GridPosition> FindPath(GridPosition startPosition, GridPosition endPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closeList = new List<PathNode>();
        PathNode startNode = gridSystem.GetGridObject(startPosition);
        PathNode endNode = gridSystem.GetGridObject(endPosition);
        openList.Add(startNode);

        for(int x = 0; x < gridSystem.GetWidth(); x++)
        {
            for(int z = 0; z < gridSystem.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = gridSystem.GetGridObject(gridPosition);
                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.ResetPreviousNode();
            }
        }

        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startPosition, endPosition));

        while(openList.Count > 0)
        {
            PathNode  currentNode = GetLowestFCost(openList);

            if(currentNode == endNode) 
            {
                pathLength = endNode.GetFCost();
                return CalculatePath(endNode);
            }
            openList.Remove(currentNode);
            closeList.Remove(currentNode);

            foreach(var neighbor in GetNeighbors(currentNode))
            {
                if(closeList.Contains(neighbor)) continue;
                if(!neighbor.IsWalkable())
                {
                    closeList.Add(neighbor);
                    continue;
                }

                int tentativeCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbor.GetGridPosition());
                if(tentativeCost < neighbor.GetGCost()) 
                {
                    neighbor.SetPreviousNode(currentNode);
                    neighbor.SetGCost(tentativeCost);
                    neighbor.SetHCost(CalculateDistance(neighbor.GetGridPosition(), endPosition));
                    if(!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }

        //No path found
        pathLength = 0;
        return null;
    }

    public int CalculateDistance(GridPosition startPosition, GridPosition endPosition)
    {
        GridPosition gridDistance = startPosition - endPosition;
        int xDistance = Mathf.Abs(gridDistance.x);
        int zDistance =  Mathf.Abs(gridDistance.z);
        int totalDistance = xDistance + zDistance;
        int remainder = Mathf.Abs(xDistance - zDistance);
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
        GridPosition gridPosition = pathNode.GetGridPosition();
        
        //Straight paths
        neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z));
        neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z));
        neighbors.Add(GetNode(gridPosition.x, gridPosition.z - 1));
        neighbors.Add(GetNode(gridPosition.x, gridPosition.z + 1));

        //Diagonal paths
        neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z + 1));
        neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z - 1));
        neighbors.Add(GetNode(gridPosition.x - 1, gridPosition.z - 1));
        neighbors.Add(GetNode(gridPosition.x + 1, gridPosition.z + 1));

        neighbors.RemoveAll(x => x == null);
        return neighbors;
    }

    private PathNode GetNode(int x, int z)
    {
        if(x < 0 || x >= width) return null;
        if(z < 0 || z >= height) return null;
        return gridSystem.GetGridObject(new GridPosition(x, z));
    }
    
    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<GridPosition> positions = new List<GridPosition>();
        PathNode currentNode = endNode;
        while(currentNode.GetPreviousNode() != null)
        {
            positions.Add(currentNode.GetGridPosition());
            currentNode = currentNode.GetPreviousNode();
        }

        positions.Reverse();
        return positions;
    }

    public bool HasWalkablePath(GridPosition startPosition, GridPosition endPosition)
    {
        return FindPath(startPosition, endPosition, out int pathLength) != null;
    }
    #endregion

    #region //Getters
    public bool GetIsWalkable(GridPosition gridPosition) => gridSystem.GetGridObject(gridPosition).IsWalkable();
    public void SetIsWalkable(GridPosition gridPosition, bool isWalkable) => gridSystem.GetGridObject(gridPosition).SetIsWalkable(isWalkable);
    public int GetPathLength(GridPosition startPosition, GridPosition endPosition)
    {
        FindPath(startPosition, endPosition, out int pathLength);
        return pathLength / COST_MULT;
    } 
    #endregion
}