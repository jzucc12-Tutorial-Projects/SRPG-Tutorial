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


    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetUnitAtGridPosition(gridPosition);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        currentQuantity--;
        target.Heal(healingAmount);
        target = null;
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction() => currentQuantity > 0;
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