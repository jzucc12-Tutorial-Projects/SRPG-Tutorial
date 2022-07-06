using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit can interact with certain objects
/// </summary>
public class InteractAction : TargetedAction
{
    private IInteractable target = null;


    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell.GetInteractable();
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        target.Interact(unit, ActionFinish);
        target = null;
    }

    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(cell, 10);
    }

    public override List<GridCell> GetValidCells()
    {
        List<GridCell> validCells = new List<GridCell>();
        
        foreach(var cell in GetRangeCells(unit.GetGridCell()))
        {
            var interactable = cell.GetInteractable();
            if(interactable == null) continue;
            validCells.Add(cell);
        }
        
        return validCells;
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Interact";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
