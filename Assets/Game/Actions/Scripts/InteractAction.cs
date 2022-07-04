using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : TargetedAction
{
    private IInteractable target = null;


    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetInteractableAtGridPosition(gridPosition);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        target.Interact(unit, ActionFinish);
        target = null;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 10);
    }

    public override List<GridPosition> GetValidPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        
        foreach(var position in GetRangePositions(unit.GetGridPosition()))
        {
            var interactable = LevelGrid.instance.GetInteractableAtGridPosition(position);
            if(interactable == null) continue;
            validPositions.Add(position);
        }
        
        return validPositions;
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Interact";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
