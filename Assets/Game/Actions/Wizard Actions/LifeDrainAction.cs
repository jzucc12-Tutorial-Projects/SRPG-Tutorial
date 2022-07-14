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
    /// <summary>
    /// Priotizes lower health enemies and when its health is low
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        //Set up
        int score = -3 * maxCooldown;
        Unit targetUnit = targetCell.GetUnit();

        //Accuracy score
        int accuracy = accuracySO.CalculateAccuracy(unit, unitCell.GetWorldPosition(), target);
        score += accuracy - 100;

        //Target score
        int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage * unit.GetDamageMod());
        score += 100 - hpDiff/2;
        if(hpDiff <= 0)
            score += 120;

        float hpPercent = unit.GetHealthPercentage();
        if(hpPercent > 0.7f) score += 0;
        else if(hpPercent > 0.5f) score += Mathf.RoundToInt(1 / hpPercent);
        else if(hpPercent > 0.25f) score += Mathf.RoundToInt(45 / hpPercent);
        else score += 500;
        
        return new EnemyAIAction(this, targetCell, score);
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
