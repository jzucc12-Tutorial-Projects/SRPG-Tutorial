using System;
using UnityEngine;

public class MeleeAction : TargetedAction, IAnimatedAction
{
    #region //State
    private ITargetable target = null;
    public static event Action OnMeleeStatic;
    #endregion

    #region //Animated Actions
    public event Action<IAnimatedAction> StartRotation;
    #endregion


    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetTargetableAtGridPosition(gridPosition);
        StartRotation?.Invoke(this);
        ActionStart(onFinish);
    }
    
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 150);
    }
    #endregion

    #region //Animated Action
    public void OnFacing() { }

    public void AnimationAct()
    {
        if(!isActive) return;
        OnMeleeStatic?.Invoke();
        target.Damage(100);
    }

    public void AnimationEnd()
    {
        if(!isActive) return;
        ActionFinish();
    }

    public AnimData GetAnimData() => new AnimData(target.GetWorldPosition(), "Melee");
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    #endregion
}