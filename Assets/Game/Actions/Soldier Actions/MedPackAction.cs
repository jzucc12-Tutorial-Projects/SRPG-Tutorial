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
        int healAmount = target.Heal(healingAmount);
        if(target == unit) CallLog($"{unit.GetName()} healed themself for {healAmount} hp");
        else CallLog($"{unit.GetName()} healed {target.GetName()} for {healAmount} hp");
        target = null;
        ActionFinish();
    }

    public void Resupply()
    {
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction()
    {
        return currentQuantity > 0 && base.CanSelectAction();
    }
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(this, cell, 0);
    }
    #endregion

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        base.SetUpToolTip();
        toolTip.effectText = $"Heal yourself for {healingAmount.ToString()} hp";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Bandages";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}