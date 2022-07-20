using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resets the cooldown of an attached action
/// </summary>
public class FocusAction : BaseAction, IAltAction, IOnSelectAction
{
    #region //Variables
    [Header("Focus Action")]
    [SerializeField] private CooldownAction cooldownAction = null;
    private int reduceAmount = 0;
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
        cooldownAction.ReduceCooldown(reduceAmount);
        unit.PlaySound("resupply");
        CallLog($"{unit.GetName()} focused on {cooldownAction.GetActionName()}");
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        reduceAmount = GetAPCost(unit.GetAP());
    }

    public void OnUnSelected()
    {
        reduceAmount = 0;
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
    protected override int GetScore(EnemyAIActionList actionList, GridCell targetCell, GridCell unitCell)
    {
        float cooldownLeft = Mathf.Max(0, cooldownAction.GetCooldownPercentLeft());
        return Mathf.RoundToInt(cooldownLeft * 30 * actionList.GetAP());
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.costText = $"Uses 1AP per cooldown turn reduced.";
        tooltip.effectText = $"Reduces the cooldown of {cooldownAction.GetActionName()} by 1 per AP spent";
        tooltip.altText = $"Switch back to {cooldownAction.GetActionName()}";
    }
    #endregion

    #region //Getters
    public BaseAction GetRootAction() => cooldownAction;
    public override string GetActionName() => $"Focus {cooldownAction.GetActionName()}";
    public override int GetAPCost(int currentAP) => Mathf.Min(cooldownAction.GetCurrentCooldown(), currentAP);
    #endregion
}
