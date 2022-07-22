using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AOE that can be cast through walls. Serves similarly to grenades
/// </summary>
public class FireballAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Fireball Action")]
    [SerializeField] private int aoeSize = 2;
    [SerializeField] private int damage = 30;
    private GridCell target;
    private List<GridCell> targetCells = new List<GridCell>();
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
        SetTrigger?.Invoke("Fireball");
        unit.PlaySound("fireball");
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
        AIDamageVars vars = new AIDamageVars(damage, 50, 30, 25);
        int score = unit.AOEScoring(GetTargets(targetCell), vars, -1, -30, 2/maxCooldown);
        return score + base.GetScore(actionList, unitCell, targetCell);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var fireball = effectsManager.GetFireball();
        fireball.transform.position = GetTargetPosition();
        fireball.transform.rotation = Quaternion.identity;
        CallLog($"{unit.GetName()} summoned a fireball");
        foreach(var targetable in GetTargets(target))
            targetable.Damage(unit, damage, false);
    }

    public void AnimationEnd()
    {
        ActionFinish(targetCells);
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = "Summon a fireball that hurts\nanything within";
        tooltip.altText = "Switch to focus action";
        tooltip.damageText = damage.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Fireball";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    private IEnumerable<ITargetable> GetTargets(GridCell targetCell)
    {
        targetCells.Clear();
        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, true))
        {
            ITargetable targetable = levelGrid.GetTargetable(cell);
            if(targetable == null) continue;
            Vector3 dir = targetable.GetWorldPosition() - targetCell.GetWorldPosition();
            if(Physics.Raycast(targetCell.GetWorldPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            targetCells.Add(cell);
            yield return targetable;
        } 
    }
    #endregion
}
