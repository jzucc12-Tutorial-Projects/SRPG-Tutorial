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
    /// <summary>
    /// Prioritizes units of other targetables. Highly prioritizes if the attack kills
    /// Aggressively targets supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        //Set up
        int score = 0;
        ITargetable target = targetCell.GetTargetable();

        //Target score
        Unit targetUnit = target as Unit;
        SupplyCrate crate = target as SupplyCrate;
        if(crate != null) score += 99;
        if(targetUnit == null) score += 50;
        else
        {
            int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage * unit.GetDamageMod());
            score += 100 - hpDiff/2;
            if(hpDiff <= 0)
                score += 100;
        }

        return new EnemyAIAction(this, targetCell, score);
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