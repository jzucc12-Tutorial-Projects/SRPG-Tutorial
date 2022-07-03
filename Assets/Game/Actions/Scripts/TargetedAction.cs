using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetedAction : BaseAction
{
    #region //Variables
    [Header("Targeted Actions")]
    [SerializeField] private bool requiresTargetable = true;
    [SerializeField] private bool isHealing = false;
    [SerializeField] protected int actionRange = 1;
    [Tooltip("How close to the target you must be facing to perform the action")] [SerializeField, MinMax(0, 1)] private float facingLimit = 0.9f;
    [Tooltip("Minimum wait to start action if unit is already facing its target")] [SerializeField, Min(0)] private float waitTime = 0;
    [SerializeField] private LayerMask obstacleLayer = 0;
    #endregion
    

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        ActionStart(onFinish);
        StartCoroutine(FaceTarget(GetTargetPosition(), OnFacing));
    }
    
    protected IEnumerator FaceTarget(Vector3 target, Action onFacing)
    {
        Vector3 aimDir = (target - unit.GetWorldPosition()).normalized;

        float dT = 0;
        while(!aimDir.AlmostFacing(transform.forward, facingLimit))
        {
            yield return null;
            unit.Rotate(aimDir);
            dT += Time.deltaTime;
        }

        if(dT < waitTime) yield return new WaitForSeconds(waitTime - dT);
        OnFacing();

        while(unit.Rotate(aimDir))
            yield return null;
    }

    protected abstract Vector3 GetTargetPosition();

    protected abstract void OnFacing();
    #endregion

    #region //Getting positions
    public override List<GridPosition> GetValidPositions()
    {
        return GetTargetedPositions(unit.GetGridPosition());
    }

    public int GetTargetsAtPosition(GridPosition position)
    {
        return GetTargetedPositions(position).Count;
    }

    public List<GridPosition> GetTargetedPositions(GridPosition attackerPosition)
    {
        List<GridPosition> validPositions = new List<GridPosition>();

        foreach(var position in GetRangePositions(attackerPosition))
        {
            if(requiresTargetable)
            {
                var target = LevelGrid.instance.GetTargetableAtGridPosition(position);
                if(target == null) continue;
                if(!target.CanBeTargeted(unit, isHealing)) continue;
            }

            validPositions.Add(position);
        }

        return validPositions;
    }

    public List<GridPosition> GetRangePositions(GridPosition attackerPosition)
    {
        List<GridPosition> validPositions = new List<GridPosition>();

        foreach(var position in LevelGrid.instance.CheckGridRange(attackerPosition, actionRange, circularRange, includeSelf))
        {
            Vector3 targetPosition = LevelGrid.instance.GetWorldPosition(position);
            Vector3 attackerWorldPosition = LevelGrid.instance.GetWorldPosition(attackerPosition);
            Vector3 aimDir = (targetPosition - attackerWorldPosition).normalized;
            bool hit = Physics.Raycast(unit.GetTargetPosition(), aimDir,
                            Vector3.Distance(unit.GetTargetPosition(), targetPosition),
                            obstacleLayer);

            if(hit) continue;
            validPositions.Add(position);
        }

        return validPositions;
    }
    #endregion

    #region //Getters
    public int GetRange() => actionRange;
    #endregion
}