using System;
using UnityEngine;

/// <summary>
/// Actions that go on cooldown after use
/// </summary>
public abstract class CooldownAction : TargetedAction, ISupply
{
    #region //variables
    [Header("Cooldown action")]
    [SerializeField, Min(1)] protected int maxCooldown = 1;
    protected int currentCooldown = 0;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentCooldown = 0;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        TurnSystem.IncrementTurn += CooldownTick;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        TurnSystem.IncrementTurn -= CooldownTick;
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        currentCooldown = maxCooldown;
        base.TakeAction(gridCell, onFinish);
    }
    public void Resupply()
    {
        currentCooldown = 0;
    }

    public void ReduceCooldown(int amount)
    {
        currentCooldown = Mathf.Max(0, currentCooldown - amount);
    }

    private void CooldownTick(bool team1Turn)
    {
        if(team1Turn ^ unit.IsTeam1()) return;
        currentCooldown = Mathf.Max(0, currentCooldown - 1);
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction(int currentAP)
    {
        return currentCooldown == 0 && base.CanSelectAction(currentAP);
    }
    #endregion

    #region //Enemy AI Action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        if(actionList.HasAction<CooldownAction>()) return -25;
        return 0;
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        string sText = maxCooldown > 1 ? "s" : "";
        tooltip.cooldownText = $"{maxCooldown} turn{sText}";
    }
    #endregion

    #region //Getters
    public override int GetQuantity()
    {
        if(currentCooldown > 0) return currentCooldown;
        else return -1;
    }

    public float GetCooldownPercentLeft() => currentCooldown / maxCooldown;
    public int GetMaxCooldown() => maxCooldown;
    #endregion
}