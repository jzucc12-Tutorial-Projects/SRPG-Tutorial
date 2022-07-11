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
    [SerializeField] private GameObject healingFX = null;
    private GridCell target;
    private MouseWorld mouseWorld = null;
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
        SetTrigger?.Invoke("isShooting");
        //SetTrigger?.Invoke("Healing Wind");
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
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell cell)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        Instantiate(healingFX, GetTargetPosition(), Quaternion.identity);
        CallLog($"{unit.GetName()} summoned a healing aura");
        foreach(var cell in levelGrid.CheckGridRange(target, aoeSize, circularRange, true))
        {
            Unit targetUnit = cell.GetUnit();
            if(targetUnit == null) continue;
            Vector3 dir = targetUnit.GetWorldPosition() - GetTargetPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            int healAmount = targetUnit.Heal(healing);
            CallLog($"{targetUnit.GetName()} was healed for {healAmount} hp");
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
