using System;
using UnityEngine;

public class MeleeAction : TargetedAction, IAnimatedAction
{
    #region //Variables
    [Header("Melee Action")]
    private int damage = 70;
    private ITargetable target = null;
    public static event Action OnMeleeStatic;
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion


    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetTargetableAtGridPosition(gridPosition);
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Melee");
    }
    #endregion
    
    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 150);
    }
    #endregion

    #region //Animated Action

    public void AnimationAct()
    {
        OnMeleeStatic?.Invoke();
        CallLog($"{unit.GetName()} struck {target.GetTargetName()} for {damage} damage");
        target.Damage(damage);
    }

    public void AnimationEnd()
    {
        ActionFinish();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}