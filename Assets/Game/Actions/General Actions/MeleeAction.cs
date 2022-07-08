using System;
using UnityEngine;

/// <summary>
/// Unit can melee
/// </summary>
public class MeleeAction : TargetedAction, IAnimatedAction
{
    #region //Variables
    [Header("Melee Action")]
    [SerializeField] private int damage = 70;
    private ITargetable target = null;
    public static event Action OnMeleeStatic;
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion


    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell.GetTargetable();
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Melee");
    }
    #endregion
    
    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(this, cell, 150);
    }
    #endregion

    #region //Animated Action

    public void AnimationAct()
    {
        OnMeleeStatic?.Invoke();
        CallLog($"{unit.GetName()} struck {target.GetName()} for {damage} damage");
        target.Damage(damage);
    }

    public void AnimationEnd()
    {
        ActionFinish();
    }
    #endregion

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        base.SetUpToolTip();
        toolTip.effectText = "Strike a nearby target";
        toolTip.damageText = $"{damage}";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}