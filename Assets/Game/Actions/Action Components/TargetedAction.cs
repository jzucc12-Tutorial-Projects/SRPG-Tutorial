using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Actions that require a target outside of the the calling unit
/// </summary>
public abstract class TargetedAction : BaseAction
{
    #region //Variables
    [Header("Targeted Actions")]
    [Tooltip("True if you don't want to count diagonals in range")] [SerializeField] protected bool circularRange = true;
    [Tooltip("True if the unit can target itself")] [SerializeField] protected bool includeSelf  = false;
    [SerializeField] private bool requiresUnit = false;
    [SerializeField, HideIf("requiresUnit")] private bool requiresTargetable = true;
    [SerializeField] private bool targetAllies = false;
    [SerializeField] protected int actionRange = 1;
    [Tooltip("How close to the target you must be facing to perform the action")] [SerializeField, MinMax(0, 1)] private float facingLimit = 0.9f;
    [Tooltip("Minimum wait to start action if unit is already facing its target")] [SerializeField, Min(0)] private float waitTime = 0;
    [SerializeField] private LayerMask blockingLayer = 0;
    #endregion
    

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        ActionStart(onFinish, gridCell);
        if(GetTargetPosition() == unit.GetWorldPosition())
            OnFacing();
        else
            StartCoroutine(FaceTarget(GetTargetPosition(), OnFacing));
    }
    
    //Rotation to face the target
    protected IEnumerator FaceTarget(Vector3 target, Action onFacing)
    {
        Vector3 aimDir = (target - unit.GetWorldPosition().PlaceOnGrid()).normalized;
        float dT = 0;

        //Rotate close enough to initiate action
        while(!aimDir.AlmostFacing(transform.forward, facingLimit))
        {
            if(target.GetGridCell() == unit.GetGridCell()) break;
            yield return null;
            unit.Rotate(aimDir);
            dT += Time.deltaTime;
        }

        //Wait remaining time if target started facing the action already
        if(dT < waitTime) yield return new WaitForSeconds(waitTime - dT);
        OnFacing();

        //Finish rotation if needed
        while(unit.Rotate(aimDir))
            yield return null;
    }
    protected abstract void OnFacing();
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        string rangeText;
        if(actionRange > 0) rangeText = actionRange.ToString();
        else rangeText = "Self";
        tooltip.rangeText = rangeText;
    }
    #endregion

    #region //Getters
    public abstract Vector3 GetTargetPosition();

    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return GetTargetedCells(unitCell);
    }

    public int GetTargetsAtCell(GridCell cell)
    {
        return GetTargetedCells(cell).Count;
    }

    //Get all cells that the action can target
    public List<GridCell> GetTargetedCells(GridCell attackerCell)
    {
        List<GridCell> validCells = new List<GridCell>();

        foreach(var cell in GetRangeCells(attackerCell))
        {
            if(requiresUnit)
            {
                var targetUnit = cell.GetUnit();
                if(targetUnit == null) continue;
                if(!targetUnit.CanBeTargeted(unit, targetAllies)) continue;
            }
            else if(requiresTargetable)
            {
                var target = cell.GetTargetable();
                if(target == null) continue;
                if(!target.CanBeTargeted(unit, targetAllies)) continue;
            }

            validCells.Add(cell);
        }

        return validCells;
    }

    //Get all cells in range of the action
    public List<GridCell> GetRangeCells(GridCell attackerCell)
    {
        List<GridCell> validCells = new List<GridCell>();
        Vector3 attackerWorldPosition = unit.ConvertToShoulderHeight(attackerCell);

        foreach(var cell in levelGrid.CheckGridRange(attackerCell, actionRange, circularRange, includeSelf))
        {
            //Omit cells that have obstacles (needed for actions that aren't impeded by obstacles)
            if(cell.HasObstacle()) continue;

            Vector3 targetPosition = unit.ConvertToShoulderHeight(cell);
            Vector3 aimDir = (targetPosition - attackerWorldPosition).normalized;
            bool obstacleHit = Physics.Raycast(attackerWorldPosition, aimDir, out RaycastHit hit,
                            Vector3.Distance(attackerWorldPosition, targetPosition), 
                            blockingLayer);
            
            //Check to make sure target cells aren't skipped
            if(obstacleHit)
            {
                //Checks if the testing cell is the one that was hit.
                //If so, only skip if that cell does not have a targetable
                GridCell hitCell = hit.point.GetGridCell();
                if(cell != hitCell) continue;
                if(hitCell.GetTargetable() == null) continue;
            }

            validCells.Add(cell);
        }

        return validCells;
    }
    public int GetRange() => actionRange;
    #endregion
}