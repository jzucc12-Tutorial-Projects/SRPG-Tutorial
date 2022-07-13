using System;
using UnityEngine;

/// <summary>
/// Base spell attack for Wizard. Similar to shooting.
/// </summary>
public class ManaBoltAction : TargetedAction, IAnimatedAction, IOnSelectAction
{
    #region //Mana bolt action
    [Header("Mana bolt Action")]
    [SerializeField] private int damage = 0;
    [SerializeField] private Transform manaBoltOrigin = null;
    [SerializeField] private AccuracySO accuracySO = null;
    private ITargetable target = null;
    private EffectsManager effectsManager = null;
    #endregion

    #region //Animated action
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion

    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        effectsManager = FindObjectOfType<EffectsManager>();
    }
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
        SetTrigger?.Invoke("Mana Bolt");
    }

    private void BoltHit()
    {
        //Calculate damage
        float hitModifier = accuracySO.DamageMult(unit, manaBoltOrigin.position, target);
        int damageDealt = (int)(damage * hitModifier * unit.GetDamageMod());

        //Damage infliction
        if(hitModifier == 0) CallLog($"{unit.GetName()} missed {target.GetName()}");
        target.Damage(unit, damageDealt);
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsEnemy()) return;
        AccuracyHub.ShowAccuracyUI(unit, GetTargetedCells(unit.GetGridCell()), accuracySO);
    }

    public void OnUnSelected()
    {
        if(unit.IsEnemy()) return;
        AccuracyHub.HideAccuracyUI();
    }
    #endregion

    #region //Enemy action
    /// <summary>
    /// Prioritizes units over other targets. Highly prioritizes if it kills.
    /// Drops priority with accuracy and increases with lower target hp.
    /// Aggressively destroys supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        //Set up
        int score = 0;
        ITargetable target = targetCell.GetTargetable();

        //Accuracy score
        int accuracy = accuracySO.CalculateAccuracy(unit, unitCell.GetWorldPosition(), target);
        score += accuracy - 100;

        //Target score
        Unit targetUnit = target as Unit;
        SupplyCrate crate = target as SupplyCrate;
        if(crate != null) score += 100;
        else if(targetUnit == null) score += 25;
        else
        {
            int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage * unit.GetDamageMod());
            score += 100 - hpDiff/2;
            if(hpDiff <= 0)
                score += 120;
        }
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var manaBolt = effectsManager.GetManaBolt();
        manaBolt.transform.position = manaBoltOrigin.position;
        manaBolt.transform.rotation = Quaternion.identity;
        var manaBoltTarget = target.GetWorldPosition();
        manaBoltTarget.y = manaBolt.transform.position.y;
        manaBolt.SetUp(manaBoltTarget, BoltHit);
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
        tooltip.effectText = $"Shoot a highly accurate bolt at a foe";
        tooltip.damageText = $"{damage} to hit, {damage * accuracySO.GetCritMult()} to crit";
        tooltip.accuracyText = $"{accuracySO.GetBaseAccuracy()} to hit, {accuracySO.GetCritChance()} to crit";
    }
    #endregion

    #region //Getters
    public override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    public override string GetActionName() => "Mana\nbolt";
    #endregion
}