using System;
using System.Collections.Generic;
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
        unit.PlaySound("tremor");
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.SetAOESize(aoeSize, true);
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
        AIDamageVars vars = new AIDamageVars(damage, 55, 35, 20);
        if(actionList.GetAggression() > 5) vars.SetNonUnitValues(15, 5);
        int score = unit.AOEScoring(GetTargets(targetCell), vars, 0, 0, 1);
        return score + base.GetScore(actionList, unitCell, targetCell);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        TremorStarted?.Invoke();
        CallLog($"{unit.GetName()} made the world tremor");
        foreach(var target in GetTargets(target))
        {
            target.Damage(unit, damage, false);
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
    public IEnumerable<ITargetable> GetTargets(GridCell targetCell)
    {
        foreach(var cell in levelGrid.CheckGridRange(targetCell, aoeSize, circularRange, true))
        {
            ITargetable targetable = cell.GetTargetable();
            if(targetable == null || !targetable.CanBeTargeted(unit, false)) continue;
            Vector3 dir = targetable.GetWorldPosition() - GetTargetPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            yield return targetable;
        }
    }
    #endregion
}
