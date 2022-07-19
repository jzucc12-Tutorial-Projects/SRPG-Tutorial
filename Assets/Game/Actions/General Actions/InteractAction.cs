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
    #endregion

    #region //Action selection
    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        List<GridCell> validCells = new List<GridCell>();
        
        foreach(var cell in GetRangeCells(unitCell))
        {
            var interactable = cell.GetInteractable();
            if(interactable == null) continue;
            validCells.Add(cell);
        }
        
        return validCells;
    }
    #endregion

    #region //Enemy action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        return 0;
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = "Interact with select objects";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Interact";
    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
