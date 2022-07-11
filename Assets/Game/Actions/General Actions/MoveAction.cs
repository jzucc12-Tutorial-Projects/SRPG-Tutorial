using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit can move
/// </summary>
public class MoveAction : BaseAction
{
    #region //Movement variables
    [Header("Move Action")]
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float threshold = 0.025f;
    [Tooltip("True if you don't want to count diagonals in range")] [SerializeField] protected bool circularRange = true;
    [Tooltip("True if the unit can target itself")] [SerializeField] protected bool includeSelf  = false;
    private List<Vector3> positionList = new List<Vector3>();
    private int positionIndex = 0;
    private Pathfinding pathfinder = null;
    private UnitManager unitManager = null;
    #endregion

    #region //Events
    public event Action StartMoving;
    public event Action StopMoving;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        pathfinder = FindObjectOfType<Pathfinding>();
        unitManager = FindObjectOfType<UnitManager>();
    }
    #endregion

    #region //Movement
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        var cellList = pathfinder.FindPath(unit.GetGridCell(), gridCell, out int pathLength);
        positionList = new List<Vector3>();
        foreach(var gp in cellList)
            positionList.Add(gp.GetWorldPosition());
            
        StartCoroutine(Move());
        ActionStart(onFinish, gridCell);
    }

    private IEnumerator Move()
    {
        StartMoving?.Invoke();
        positionIndex = 0;
        while(positionIndex < positionList.Count)
        {
            var targetPosition = positionList[positionIndex];
            while(!targetPosition.InRange(transform.position, threshold))
            {
                Vector3 moveDirection = (targetPosition - transform.position).normalized;
                unit.Rotate(moveDirection);
                transform.position += moveDirection * moveSpeed * Time.deltaTime;
                yield return null;
            }
            positionIndex++;
        }

        ActionFinish();
        StopMoving?.Invoke();
    }
    #endregion

    #region //Action selection
    public override bool CanTakeAction(GridCell gridCell)
    {
        return GetValidCells().Contains(gridCell);
    }

    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        List<GridCell> validCellList = new List<GridCell>();

        foreach(var cell in levelGrid.CheckGridRange(unit.GetGridCell(), maxMoveDistance, circularRange, includeSelf))
        {
            if(!cell.IsWalkable()) continue;
            if(cell.HasAnyUnit()) continue;
            if(!pathfinder.HasWalkablePath(unitCell, cell)) continue;
            if(pathfinder.GetPathLength(unitCell, cell) > maxMoveDistance) continue;
            validCellList.Add(cell);
        }

        return validCellList;
    }
    #endregion

    #region //Enemy Action
    /// <summary>
    /// Prioritizes not being seen by enemies and being further away
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        int score = 0;
        Vector3 newPosition = targetCell.GetWorldPosition();
        foreach(var player in unitManager.GetPlayerList())
        {
            var dir = (player.GetWorldPosition() - newPosition);
            if(Physics.Raycast(newPosition, dir, dir.magnitude, GridGlobals.obstacleMask))
                score += 25;
            score += targetCell.GetGridDistance(player.GetGridCell()); 
        }
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.effectText = "Move the selected unit";
        tooltip.rangeText = maxMoveDistance.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Move";
    #endregion
}