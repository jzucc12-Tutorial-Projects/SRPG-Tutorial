using System;
using UnityEngine;

public class MedPackAction : TargetedAction
{
    #region //Variables
    [Header("Med Pack")]
    [SerializeField] private int healingAmount = 50;
    [SerializeField] private int maxQuantity = 1;
    private int currentQuantity = 1;
    private Unit target = null;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetUnitAtGridPosition(gridPosition);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        currentQuantity--;
        int healAmount = target.Heal(healingAmount);
        if(target == unit) CallLog($"{unit.GetName()} healed themselves for {healAmount} health");
        target = null;
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction()
    {
        return currentQuantity > 0 && base.CanSelectAction();
    }
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 0);
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Bandages";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}