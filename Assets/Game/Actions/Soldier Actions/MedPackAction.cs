using System;
using System.Collections.Generic;
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
        target = levelGrid.GetUnit(gridCell);
        unit.PlaySound("heal");
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        currentQuantity--;
        target.Heal(unit, healingAmount);
        ActionFinish(new List<GridCell>() { target.GetGridCell() });
        target = null;
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
        Unit targetUnit = levelGrid.GetUnit(targetCell);
        float myHPPercent = unit.GetHealthPercentage();
        if(myHPPercent == 1) return 0;

        float factor = Mathf.Lerp(20, 40, actionList.PassiveLevel()/10f);
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