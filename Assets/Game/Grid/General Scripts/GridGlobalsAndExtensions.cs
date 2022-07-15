using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constants associated with the grid
/// </summary>
public static class GridGlobals
{
    public const int cellSize = 2; //Multiplier for grid cell conversion into world space
    public const float raycastLength = 3.1f; //Used to finding objects on the grid
    public static readonly LayerMask unitMask = 1 << 7; //Unit collider
    public static readonly LayerMask targetableMask = 1 << 7 | 1 << 9; //Unit collider or level object
    public static readonly LayerMask interactableMask = 1 << 9; //Level object
    public static readonly LayerMask obstacleMask = 1 << 8; //Obstacle
    public static readonly LayerMask blockWalkingMask = 1 << 8 | 1 << 9; //Level object or Obstacle
    public static readonly LayerMask noTargetMask = 1 << 11;
}

/// <summary>
/// Extensions specifically for the grid
/// </summary>
public static class GridExtensions
{
    /// <summary>
    /// Convert world space to grid space
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public static GridCell GetGridCell(this Vector3 worldPosition)
    {
        return new GridCell(
            Mathf.RoundToInt(worldPosition.x / GridGlobals.cellSize),
            Mathf.RoundToInt(worldPosition.z / GridGlobals.cellSize));
    }

    /// <summary>
    /// Convert grid grid space to world space
    /// </summary>
    /// <param name="gridCell"></param>
    /// <returns></returns>
    public static Vector3 GetWorldPosition(this GridCell gridCell)
    {
        return new Vector3(gridCell.x, 0, gridCell.z) * GridGlobals.cellSize;
    }

    /// <summary>
    /// Returns the world space location of the closest grid cell
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <returns></returns>
    public static Vector3 PlaceOnGrid(this Vector3 currentPosition)
    {
        return GetWorldPosition(GetGridCell(currentPosition));
    }

    /// <summary>
    /// Finds the number of cells between the start and end
    /// </summary>
    /// <param name="startCell"></param>
    /// <param name="endCell"></param>
    /// <param name="ignoreDiagonals"></param>
    /// <returns></returns>
    public static int GetGridDistance(this GridCell startCell, GridCell endCell, bool ignoreDiagonals = true)
    {
        GridCell gridDistance = startCell - endCell;
        int xDistance = Mathf.Abs(gridDistance.x);
        int zDistance =  Mathf.Abs(gridDistance.z);
        if(ignoreDiagonals) 
            return  xDistance + zDistance;
        else
        {
            int remainder = Mathf.Abs(xDistance - zDistance);
            return Mathf.Min(xDistance, zDistance) + remainder;
        }
    }

    /// <summary>
    /// Finds the x,z offset between the start and end cell
    /// </summary>
    /// <param name="startCell"></param>
    /// <param name="endCell"></param>
    /// <param name="ignoreDiagonals"></param>
    /// <returns></returns>
    public static GridCell GetOffset(this GridCell startCell, GridCell endCell)
    {
        return endCell - startCell;
    }

    public static bool HasAnyUnit(this GridCell gridCell)
    {
        return GetUnit(gridCell) != null;
    }
    public static Unit GetUnit(this GridCell gridCell)
    {
        return GetComponentAtCell<Unit>(gridCell, GridGlobals.unitMask);
    }
    public static List<Unit> GetUnits(this GridCell gridCell)
    {
        return GetComponentsAtCell<Unit>(gridCell, GridGlobals.unitMask);
    }
    public static IInteractable GetInteractable(this GridCell gridCell)
    {
        return GetComponentAtCell<IInteractable>(gridCell, GridGlobals.interactableMask);
    }
    public static ITargetable GetTargetable(this GridCell gridCell)
    {
        return GetComponentAtCell<ITargetable>(gridCell, GridGlobals.targetableMask);
    }
    public static bool HasObstacle(this GridCell gridCell)
    {
        return Raycast(gridCell, GridGlobals.obstacleMask);
    }
    public static bool CantTarget(this GridCell gridCell)
    {
        return Raycast(gridCell, GridGlobals.noTargetMask);
    }
    public static bool HasHighObstacle(this GridCell gridCell)
    {
        return HighRaycast(gridCell, GridGlobals.obstacleMask, out RaycastHit hit);
    }
    public static bool IsWalkable(this GridCell gridCell)
    {
        return GetComponentAtCell<Collider>(gridCell, GridGlobals.blockWalkingMask) == null;
    }

    public static T GetComponentAtCell<T>(this GridCell gridCell, LayerMask mask)
    {
        if(Raycast(gridCell, mask, out RaycastHit hit))
            return hit.collider.GetComponent<T>();
        else 
            return default(T);
    }

    public static List<T> GetComponentsAtCell<T>(this GridCell gridCell, LayerMask mask)
    {
        var list = new List<T>();
        var hits = Physics.RaycastAll(gridCell.GetWorldPosition()+Vector3.down, Vector3.up*GridGlobals.raycastLength, GridGlobals.raycastLength, mask);
        foreach(var hit in hits)
        {
            if(hit.collider.TryGetComponent<T>(out T component))
            {
                list.Add(component);
            }
        }
        return list;
    }

    private static bool Raycast(GridCell gridCell, LayerMask mask)
    {
        return Raycast(gridCell, mask, out RaycastHit hit);
    }

    private static bool Raycast(GridCell gridCell, LayerMask mask, out RaycastHit hit)
    {
        return Physics.Raycast(gridCell.GetWorldPosition()+Vector3.down, Vector3.up*GridGlobals.raycastLength, out hit, GridGlobals.raycastLength, mask);
    }

    /// <summary>
    /// High raycast that checks if a full height obstacle only is present
    /// </summary>
    /// <param name="gridCell"></param>
    /// <param name="mask"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    private static bool HighRaycast(GridCell gridCell, LayerMask mask, out RaycastHit hit)
    {
        float length = 1.25f;
        return Physics.Raycast(gridCell.GetWorldPosition()+(length+2)*Vector3.up, Vector3.down*length, out hit, length, mask);
    }
}