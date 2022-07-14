using System;
using UnityEngine;

/// <summary>
/// Ranged healing aoe for wizards
/// </summary>
public class HealingWindAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Healing Wind Action")]
    [SerializeField] private int aoeSize = 1;
    [SerializeField] private int healing = 30;
    private GridCell target;
    private MouseWorld mouseWorld = null;
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
        mouseWorld = FindObjectOfType<MouseWorld>();
        effectsManager = FindObjectOfType<EffectsManager>();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell;
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Healing Wind");
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.SetAOESize(aoeSize, false);
    }

    public void OnUnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.ResetAOE();
    }
    #endregion

    #region //Enemy AI
    /// <summary>
    /// Priotizes healing moare allies.
    /// Less points if there is an enemy.
    /// Likes hitting supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        int score = -3 * maxCooldown;

        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, true))
        {
            var targetUnit = cell.GetUnit();
            if(targetUnit == null) continue;

            float hpPercent = targetUnit.GetHealthPercentage();
            if(hpPercent > 0.7f) score += 10;
            else if(hpPercent > 0.5f) score += Mathf.RoundToInt(15 / hpPercent);
            else if(hpPercent > 0.25f) score += Mathf.RoundToInt(30 / hpPercent);
            else score += Mathf.RoundToInt(45 / hpPercent);
            if(!targetUnit.IsAI()) score *= -1;
        }

        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var wind = effectsManager.GetHealingWind();
        wind.transform.position = GetTargetPosition();
        wind.transform.rotation = Quaternion.identity;
        CallLog($"{unit.GetName()} summoned a healing aura");
        foreach(var cell in levelGrid.CheckGridRange(target, aoeSize, circularRange, true))
        {
            Unit targetUnit = cell.GetUnit();
            if(targetUnit == null) continue;
            Vector3 dir = targetUnit.GetWorldPosition() - GetTargetPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            targetUnit.Heal(unit, healing);
        }
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
        tooltip.effectText = $"Summons an aura that heals friend\nand foe for up to {healing} hp";
        tooltip.altText = "Switch to focus action";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Healing Wind";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
