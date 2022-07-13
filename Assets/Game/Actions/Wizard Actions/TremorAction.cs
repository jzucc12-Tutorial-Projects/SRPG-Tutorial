using System;
using UnityEngine;

/// <summary>
/// Self AOE for wizards
/// </summary>
public class TremorAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Tremor Action")]
    [SerializeField] private int aoeSize = 2;
    [SerializeField] private int damage = 30;
    private GridCell target;
    private MouseWorld mouseWorld = null;
    public static Action TremorStarted;
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
        SetTrigger?.Invoke("Tremor");
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsEnemy()) return;
        mouseWorld.SetAOESize(aoeSize, true);
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
    /// Likes hitting supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        int score = -7 * maxCooldown;
        int numTargets = 0;

        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, false))
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
                if(targetUnit.IsEnemy()) continue;
                int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage);
                score += 50 - hpDiff/3;
            }
        }

        if(numTargets == 1) score = Mathf.RoundToInt(score * 0.7f);
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        TremorStarted?.Invoke();
        CallLog($"{unit.GetName()} made the world tremor");
        foreach(var cell in levelGrid.CheckGridRange(target, aoeSize, circularRange, true))
        {
            ITargetable targetable = cell.GetTargetable();
            if(targetable == null || !targetable.CanBeTargeted(unit, false)) continue;
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
        tooltip.effectText = $"Creates tremors around you that\nwill only hurt foes";
        tooltip.altText = "Switch to focus action";
        tooltip.damageText = damage.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Tremors";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
