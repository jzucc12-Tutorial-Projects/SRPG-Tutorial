using System;
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
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsEnemy()) return;
        mouseWorld.SetAOESize(aoeSize, false);
    }

    public void OnUnSelected()
    {
        if(unit.IsEnemy()) return;
        mouseWorld.ResetAOE();
    }
    #endregion

    #region //Enemy AI
    /// <summary>
    /// Priotizes hitting more targets. Extra points for it being an enemy unit.
    /// Less points if friendly. Even less if it is itself
    /// Likes hitting supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        int score = -5 * maxCooldown;
        int numTargets = 0;

        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, true))
        {
            numTargets++;
            var targetable = cell.GetTargetable();
            if(targetable == null) continue;

            if(targetable is SupplyCrate)
                score += 15;
            else if(!(targetable is Unit))
                score += 5;
            else
            {
                Unit targetUnit = targetable as Unit;
                int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage);
                score += 50 - hpDiff/3;
                if(targetUnit.IsEnemy()) score *= -1;
                if(targetUnit == unit) score -= 50;
            }
        }

        if(numTargets == 1) score = Mathf.RoundToInt(score * 0.5f);
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var fireball = effectsManager.GetFireball();
        fireball.transform.position = GetTargetPosition();
        fireball.transform.rotation = Quaternion.identity;
        CallLog($"{unit.GetName()} summoned a fireball");
        foreach(var cell in levelGrid.CheckGridRange(target, aoeSize, circularRange, true))
        {
            ITargetable targetable = cell.GetTargetable();
            if(targetable == null) continue;
            Vector3 dir = targetable.GetWorldPosition() - GetTargetPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            targetable.Damage(unit, damage);
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
        tooltip.effectText = "Summon a fireball that hurts\nanything within";
        tooltip.altText = "Switch to focus action";
        tooltip.damageText = damage.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Fireball";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
