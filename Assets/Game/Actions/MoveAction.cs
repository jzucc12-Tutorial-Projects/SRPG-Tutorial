using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    #region //Unit variables
    [SerializeField] private Animator animator = null;
    #endregion

    #region //Movement variables
    [SerializeField] private int maxMoveDistance = 4;
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float rotateSpeed = 4;
    [SerializeField] private float threshold = 0.025f;
    private Vector3 targetPosition = Vector3.zero;
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
        animator.SetBool("isWalking", moving);
        isActive = moving;
        if(!moving) onActionFinish?.Invoke();
    }
    #endregion

    #region //Movement
    private bool Move()
    {
        if(Mathf.Abs((targetPosition - transform.position).sqrMagnitude) <= threshold * threshold) return false;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        return true;
    }

    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        onActionFinish = onFinish;
        isActive = true;
        targetPosition = LevelGrid.instance.GetWorldPosition(gridPosition);
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

        for(int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for(int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetPosition = new GridPosition(x,z);
                GridPosition testPosition = unitPosition + offsetPosition;
                if(!LevelGrid.instance.IsValidPosition(testPosition)) continue;
                if(unitPosition == testPosition) continue;
                if(LevelGrid.instance.HasAnyUnit(testPosition)) continue;
                validGridPositionList.Add(testPosition);
            }
        }

        return validGridPositionList;
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Move";
    #endregion
}