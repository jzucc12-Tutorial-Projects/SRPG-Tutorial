using System.Collections.Generic;
using UnityEngine;

public abstract class TargetedAction : BaseAction
{
    #region //Variables
    [Header("Targeted Actions")]
    [SerializeField] private bool requiresTargetable = true;
    [SerializeField] protected int actionRange = 1;
    [SerializeField] private LayerMask obstacleLayer = 0;
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
                if(!target.CanBeTargeted(unit)) continue;
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
            float shoulderHeight = 1.7f;
            bool hit = Physics.Raycast(attackerWorldPosition + Vector3.up * shoulderHeight, aimDir,
                            Vector3.Distance(attackerWorldPosition, targetPosition),
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