using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    #region //Movement variables
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float rotateSpeed = 4;
    [SerializeField] private float threshold = 0.025f;
    private List<Vector3> positionList = new List<Vector3>();
    private int positionIndex = 0;
    #endregion

    #region //Events
    public event Action StartMoving;
    public event Action StopMoving;
    #endregion


    #region //Monobehaviour
    private void Update()
    {
        if(!isActive) return;
        bool moving = Move();
        if(!moving) 
        {
            if(++positionIndex >= positionList.Count)
            {
                LevelGrid.instance.SetTargetableAtGridPosition(unit.GetGridPosition(), unit);
                ActionFinish();
                StopMoving?.Invoke();
            }
        }
    }
    #endregion

    #region //Movement
    private bool Move()
    {
        var targetPosition = positionList[positionIndex];
        if(Mathf.Abs((targetPosition - transform.position).sqrMagnitude) <= threshold * threshold) return false;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        Rotate(moveDirection);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        return true;
    }

    public void Rotate(Vector3 targetDirection)
    {
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, rotateSpeed * Time.deltaTime);
    }

    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        LevelGrid.instance.SetTargetableAtGridPosition(unit.GetGridPosition(), null);
        var gridList = Pathfinding.instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        positionList = new List<Vector3>();
        foreach(var grid in gridList)
            positionList.Add(LevelGrid.instance.GetWorldPosition(grid));

        positionIndex = 0;
        StartMoving?.Invoke();
        ActionStart(onFinish);
    }
    #endregion

    #region //Validation
    public override bool IsValidAction(GridPosition gridPosition)
    {
        return GetValidPositions().Contains(gridPosition);
    }

    public override List<GridPosition> GetValidPositions()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitPosition = unit.GetGridPosition();

        foreach(var position in LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), maxMoveDistance, circularRange, includeSelf))
        {
            if(unitPosition == position) continue;
            if(LevelGrid.instance.HasAnyUnit(position)) continue;
            if(!Pathfinding.instance.GetIsWalkable(position)) continue;
            if(!Pathfinding.instance.HasWalkablePath(unitPosition, position)) continue;
            if(Pathfinding.instance.GetPathLength(unitPosition, position) > maxMoveDistance) continue;
            validGridPositionList.Add(position);
        }

        return validGridPositionList;
    }
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        var count = unit.GetAction<ShootAction>().GetTargetsAtPosition(position);
        return new EnemyAIAction(position, count * 10 + 1);
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Move";
    #endregion
}