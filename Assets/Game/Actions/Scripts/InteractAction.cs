using System;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    [SerializeField] private int interactRange = 1;


    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        var interactable = LevelGrid.instance.GetInteractableAtGridPosition(gridPosition);
        base.TakeAction(gridPosition, onFinish);
        interactable.Interact(ActionFinish);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 10);
    }

    public override List<GridPosition> GetValidPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        
        foreach(var position in LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), interactRange, false))
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
    public int GetRange() => interactRange;
    #endregion
}
