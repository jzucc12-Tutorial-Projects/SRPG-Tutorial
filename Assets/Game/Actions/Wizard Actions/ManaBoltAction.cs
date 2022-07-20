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
    [SerializeField, ScriptableObjectDropdown(typeof(AccuracySO))] private AccuracySO accuracySO = null;
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
        target.Damage(unit, damageDealt, hitModifier > 1);
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsAI()) return;
        AccuracyHub.ShowAccuracyUI(unit, GetTargetedCells(unit.GetGridCell()), accuracySO);
    }

    public void OnUnSelected()
    {
        if(unit.IsAI()) return;
        AccuracyHub.HideAccuracyUI();
    }
    #endregion

    #region //Enemy action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        ITargetable target = targetCell.GetTargetable();
        AIDamageVars vars = new AIDamageVars(damage, 110, 20, 10);
        if(actionList.GetAggression() > 5) vars.SetNonUnitValues(10, 5);
        int score = unit.AccuracyDamageScoring(actionList.HasAction<SpinAction>(), target, vars, accuracySO, unitCell.GetWorldPosition());
        return score;
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
        unit.PlaySound("mana bolt");
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