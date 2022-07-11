using System;
using UnityEngine;

/// <summary>
/// Actions that go on cooldown after use
/// </summary>
public abstract class CooldownAction : TargetedAction, ISupply
{
    #region //variables
    [Header("Cooldown action")]
    [SerializeField] protected int maxCooldown = 1;
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

    private void CooldownTick(bool isPlayerTurn)
    {
        if(!isPlayerTurn ^ unit.IsEnemy()) return;
        currentCooldown = Mathf.Max(0, currentCooldown - 1);
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction(int currentAP)
    {
        return currentCooldown == 0 && base.CanSelectAction(currentAP);
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
    #endregion
}