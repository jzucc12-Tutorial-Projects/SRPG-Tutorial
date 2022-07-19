using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Ranged healing aoe for wizards
/// </summary>
public class LifeDrainAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Healing Wind Action")]
    [SerializeField] private int damage = 0;
    [SerializeField] private float healPercent = 0;
    [SerializeField, ScriptableObjectDropdown(typeof(AccuracySO))] private AccuracySO accuracySO = null;
    private Unit target = null;
    private EffectsManager effectsManager = null;
    private int damageDealt;
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
        target = gridCell.GetUnit();
        damageDealt = 0;
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Life Drain");
    }

    private void DrainHit()
    {
        unit.Heal(unit, Mathf.RoundToInt(damageDealt * healPercent));
        AnimationEnd();
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

    #region //Enemy AI
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        int score = 0;
        Unit targetUnit = targetCell.GetUnit();
        AIDamageVars vars = new AIDamageVars(damage, 65, 30, 20);
        score += unit.AccuracyDamageScoring(actionList.HasAction<SpinAction>(), targetUnit, vars, accuracySO, unitCell.GetWorldPosition());

        float myHPPercent = unit.GetHealthPercentage();
        score += Mathf.RoundToInt(12/myHPPercent);
        return score + base.GetScore(actionList, unitCell, targetCell);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        if(damageDealt != 0) return;
        //Calculate damage
        float hitModifier = accuracySO.DamageMult(unit, unit.GetWorldPosition(), target);
        damageDealt = (int)(damage * hitModifier * unit.GetDamageMod());

        //Damage infliction
        if(hitModifier == 0) StartCoroutine(AttackMissed());
        else 
        {
            CallLog($"{unit.GetName()} is draining {target.GetName()}'s life");
            target.Damage(unit, damageDealt);
            var lifeDrain = effectsManager.GetLifeDrain();
            var targetPosition = target.GetWorldPosition();
            targetPosition.y = lifeDrain.transform.position.y;

            lifeDrain.transform.position = targetPosition;
            lifeDrain.transform.rotation = Quaternion.identity;
            lifeDrain.SetUp(unit.GetWorldPosition(), DrainHit);
        }
    }

    private IEnumerator AttackMissed()
    {
        CallLog($"{unit.GetName()}'s life dain failed");
        yield return new WaitForSeconds(1f);
        AnimationEnd();
    }

    public void AnimationEnd()
    {
        ActionFinish();
        SetTrigger?.Invoke("Life Drain End");
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = $"Hurt an enemy and drains {healPercent*100}% of the damage";
        tooltip.accuracyText = $"{accuracySO.GetBaseAccuracy()} to hit, {accuracySO.GetCritChance()} to crit";
        tooltip.damageText = $"{damage} to hit, {damage * accuracySO.GetCritMult()} to crit.";
        tooltip.altText = "Switch to focus action";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Life Drain";
    public override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}
