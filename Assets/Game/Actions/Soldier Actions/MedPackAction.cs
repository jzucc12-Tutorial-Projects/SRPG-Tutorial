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
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        int score = 0;
        Unit targetUnit = targetCell.GetUnit();
        float myHPPercent = unit.GetHealthPercentage();
        if(myHPPercent == 1) return 0;

        float factor = Mathf.Lerp(10, 25, actionList.GetAggression()/10f);
        score += Mathf.RoundToInt(factor/myHPPercent);
        return score;
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