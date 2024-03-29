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
    [SerializeField, Min(0)] private int maxMoveDistance = 4;
    [SerializeField, Min(0)] private float moveSpeed = 4;
    [SerializeField, Min(0)] private float threshold = 0.025f;
    [SerializeField, Min(0)] private int freeMoves = 1;
    [SerializeField, Min(0)] private int totalMoves = 2;
    [SerializeField, Min(0)] private int costIncreasePerMove = 1;
    [Tooltip("True if you don't want to count diagonals in range")] [SerializeField] protected bool circularRange = true;
    [Tooltip("True if the unit can target itself")] [SerializeField] protected bool includeSelf  = false;
    private List<Vector3> positionList = new List<Vector3>();
    private int positionIndex = 0;
    private Pathfinding pathfinder = null;
    private UnitManager unitManager = null;
    private int currentMoves = 0;
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

    protected override void OnEnable()
    {
        base.OnEnable();
        TurnSystem.IncrementTurn += ResetMoves;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TurnSystem.IncrementTurn -= ResetMoves;
    }
    #endregion

    #region //Action performing
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
        GridCell startCell = unit.GetGridCell();
        currentMoves++;
        StartMoving?.Invoke();
        positionIndex = 0;
        
        unit.PlaySound("walking");
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
        unit.StopSound("walking");

        ActionFinish(new List<GridCell>() { startCell, unit.GetGridCell() });
        StopMoving?.Invoke();
    }
    #endregion

    #region //Action selection
    public override bool CanTakeAction(GridCell gridCell)
    {
        return GetValidCells().Contains(gridCell);
    }

    public override bool CanSelectAction(int currentAP)
    {
        return currentMoves < totalMoves && base.CanSelectAction(currentAP);
    }

    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        List<GridCell> validCellList = new List<GridCell>();

        foreach(var cell in levelGrid.CheckGridRange(unit.GetGridCell(), maxMoveDistance, circularRange, includeSelf))
        {
            if(!levelGrid.IsWalkable(cell)) continue;
            if(levelGrid.HasAnyUnit(cell)) continue;
            if(!pathfinder.HasWalkablePath(unitCell, cell)) continue;
            if(pathfinder.GetPathLength(unitCell, cell) > maxMoveDistance) continue;
            validCellList.Add(cell);
        }

        return validCellList;
    }

    private void ResetMoves(bool team1Turn)
    {
        if(team1Turn ^ unit.IsTeam1()) return;
        currentMoves = 0;
    }
    #endregion

    #region //Enemy Action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        if(actionList.GetAggression() == 0) return 0;

        int score = 0;
        List<Unit> enemies = new List<Unit>(unitManager.GetUnitList());
        enemies.RemoveAll(x => x.IsAI());

        score += GetPositionalScore(true, enemies, unitCell, targetCell) * actionList.GetAggression()/10;
        if(actionList.GetAggression() != 10) 
            score += GetPositionalScore(false, enemies, unitCell, targetCell) * actionList.PassiveLevel()/10;
        
        score /= enemies.Count;
        score -= 5 * actionList.ActionInListCount<MoveAction>();
        return score;
    }

    private int GetPositionalScore(bool aggressive, List<Unit> enemies, GridCell startCell, GridCell targetCell)
    {
        int score = 0;
        Vector3 newPosition = targetCell.GetWorldPosition();
        foreach(var enemy in enemies)
        {
            var dir = (enemy.GetWorldPosition() - newPosition);
            bool notSeen = Physics.Raycast(newPosition, dir, dir.magnitude, GridGlobals.obstacleMask);
            if(notSeen ^ aggressive)
                score += 40;

            int startDistance = pathfinder.GetPathLength(startCell, enemy.GetGridCell());
            int endDistance = pathfinder.GetPathLength(targetCell, enemy.GetGridCell());
            score += 5 * (endDistance - startDistance) * (aggressive ? -1 : 1); 
        }
        return score;
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.effectText = $"Move the selected unit. {totalMoves} per turn.";
        tooltip.rangeText = maxMoveDistance.ToString();

        string freeText = "";
        string thenText = freeMoves > 0 ? " then" : "";

        if(freeMoves == 1) 
            freeText += "First move is free.\n";
        else 
            freeText = freeText += $"First {freeMoves} moves are free.\n";
            
        tooltip.costText = $"{freeText}Cost{thenText} increases by {costIncreasePerMove} per move.";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Move";
    public override int GetAPCost(int currentAP)
    {
        return GetAPForMove(currentMoves);
    }
    public int GetAPForMove(int moveNumber)
    {
        if(moveNumber < freeMoves) return 0;
        int diffMoves = moveNumber - freeMoves;
        return costIncreasePerMove * (1 + diffMoves);
    }
    public override int GetQuantity() => totalMoves - currentMoves;
    #endregion
}