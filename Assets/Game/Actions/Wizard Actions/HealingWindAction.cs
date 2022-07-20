using System;
using System.Collections.Generic;
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
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        int score = 0;
        int numTargets = 0;

        foreach(var targetUnit in GetTargets(targetCell))
        {
            int unitScore = 0;
            float hpPercent = targetUnit.GetHealthPercentage();
            if(hpPercent == 1) continue; 
            float factor = Mathf.Lerp(10, 25, actionList.GetAggression()/10f);
            unitScore += Mathf.RoundToInt(factor/hpPercent);
            if(!targetUnit.IsAI()) unitScore *= -1;
            if(unitScore > 0) numTargets++;
            score += unitScore;
        }

        if(numTargets == 1) score *= Mathf.RoundToInt(2f/maxCooldown);
        return score + base.GetScore(actionList, unitCell, targetCell);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var wind = effectsManager.GetHealingWind();
        wind.transform.position = GetTargetPosition();
        wind.transform.rotation = Quaternion.identity;
        CallLog($"{unit.GetName()} summoned a healing vortex");
        unit.PlaySound("healing wind");
        foreach(var targetUnit in GetTargets(target))
        {
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
    public override string GetActionName() => "Healing Vortex";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();

    public IEnumerable<Unit> GetTargets(GridCell targetCell)
    {
        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, true))
        {
            Unit targetUnit = cell.GetUnit();
            if(targetUnit == null) continue;
            Vector3 dir = targetUnit.GetWorldPosition() - targetCell.GetWorldPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            yield return targetUnit;
        }
    }
    #endregion
}
