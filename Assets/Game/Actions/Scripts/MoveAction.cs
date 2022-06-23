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
    private Vector3 targetPosition = Vector3.zero;
    #endregion

    #region //Events
    public event Action StartMoving;
    public event Action StopMoving;
    #endregion


    #region //Monobehaviour
    protected override void Start()
    {
        base.Start();
        targetPosition = transform.position;
    }

    private void Update()
    {
        if(!isActive) return;
        bool moving = Move();
        if(!moving) 
        {
            ActionFinish();
            StopMoving?.Invoke();
        }
    }
    #endregion

    #region //Movement
    private bool Move()
    {
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
        targetPosition = LevelGrid.instance.GetWorldPosition(gridPosition);
        StartMoving?.Invoke();
        base.TakeAction(gridPosition, onFinish);
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

        foreach(var position in LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), maxMoveDistance))
        {
            if(unitPosition == position) continue;
            if(LevelGrid.instance.HasAnyUnit(position)) continue;
            validGridPositionList.Add(position);
        }

        return validGridPositionList;
    }
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        var count = unit.shootAction.GetTargetsAtPosition(position);
        return new EnemyAIAction(position, count * 10 + 1);
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Move";
    #endregion
}