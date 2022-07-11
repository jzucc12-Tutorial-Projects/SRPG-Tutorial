using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resets the cooldown of an attached action
/// </summary>
public class FocusAction : BaseAction, IAltAction
{
    #region //Variables
    [Header("Focus Action")]
    [SerializeField] private CooldownAction cooldownAction = null;
    #endregion


    #region //Monobehaviour
    protected override void Start()
    {
        base.Start();
        cooldownAction.SetAltAction(this);
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        ActionStart(onFinish, gridCell);
        cooldownAction.Resupply();
        CallLog($"{unit.GetName()} focused on {cooldownAction.GetActionName()}");
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        var selected = cooldownAction as IOnSelectAction;
        if(selected == null) return;
        selected.OnSelected();
    }

    public void OnUnSelected()
    {
        var selected = cooldownAction as IOnSelectAction;
        if(selected == null) return;
        selected.OnUnSelected();
    }

    public override bool CanSelectAction(int currentAP)
    {
        return cooldownAction.GetQuantity() > 0 && currentAP > 0 && base.CanSelectAction(currentAP);
    }

    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return new List<GridCell>() { unitCell };
    }
    #endregion

    #region //Enemy AI
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell cell)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.effectText = $"Resets the cooldown of {cooldownAction.GetActionName()}";
        tooltip.altText = $"Switch back to {cooldownAction.GetActionName()}";
    }
    #endregion

    #region //Getters
    public BaseAction GetRootAction() => cooldownAction;
    public override string GetActionName() => $"Focus {cooldownAction.GetActionName()}";
    public override int GetAPCost(int currentAP) => currentAP;
    #endregion
}
