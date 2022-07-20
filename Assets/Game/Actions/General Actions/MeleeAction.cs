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
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        ITargetable target = targetCell.GetTargetable();
        AIDamageVars vars = new AIDamageVars(damage, 100, 20, 10);
        if(actionList.GetAggression() > 5) vars.SetNonUnitValues(10, 5);
        return unit.DamageScoring(target, vars);
    }
    #endregion

    #region //Animated Action

    public void AnimationAct()
    {
        OnMeleeStatic?.Invoke();
        unit.PlaySound("melee");
        target.Damage(unit, damage, false);
    }

    public void AnimationEnd()
    {
        ActionFinish();
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = "Strike a nearby target";
        tooltip.damageText = $"{damage}";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    public override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}