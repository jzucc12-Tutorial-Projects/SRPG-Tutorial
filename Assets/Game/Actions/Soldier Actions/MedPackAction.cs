using System;
using UnityEngine;

/// <summary>
/// Unit can heal damage
/// </summary>
public class MedPackAction : TargetedAction, ISupply
{
    #region //Variables
    [Header("Med Pack Action")]
    [SerializeField] private int healingAmount = 50;
    [SerializeField] private int maxQuantity = 1;
    private int currentQuantity = 1;
    private Unit target = null;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        Resupply();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell.GetUnit();
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        currentQuantity--;
        target.Heal(unit, healingAmount);
        target = null;
        ActionFinish();
    }

    public void Resupply()
    {
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction(int currentAP)
    {
        return currentQuantity > 0 && base.CanSelectAction(currentAP);
    }
    #endregion

    #region //Enemy action
    /// <summary>
    /// Highly prioritzes as HP drops. Currently doesn't take nearby allies into account because
    /// you can't heal others.
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        float hpPercent = unit.GetHealthPercentage();
        if(hpPercent > 0.7f) return new EnemyAIAction(this, targetCell, 0);
        else if(hpPercent > 0.5f) return new EnemyAIAction(this, unitCell, Mathf.RoundToInt(1 / hpPercent));
        else if(hpPercent > 0.25f) return new EnemyAIAction(this, targetCell, Mathf.RoundToInt(45/hpPercent));
        else return new EnemyAIAction(this, targetCell, 500);
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = $"Heal you or an ally for up to {healingAmount.ToString()} hp";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Bandages";
    public override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}