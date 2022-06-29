using System;
using System.Collections.Generic;

public class InteractAction : TargetedAction
{
    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        var interactable = LevelGrid.instance.GetInteractableAtGridPosition(gridPosition);
        ActionStart(onFinish);
        interactable.Interact(ActionFinish);
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
    #endregion
}
