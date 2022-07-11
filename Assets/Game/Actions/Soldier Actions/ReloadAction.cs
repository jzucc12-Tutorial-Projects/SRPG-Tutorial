using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alt action that reloads a given weapon
/// </summary>
public class ReloadAction : BaseAction, IAltAction, IOnSelectAction
{
    #region //Variables
    [SerializeField] private ShootAction shootAction = null;
    #endregion


    #region //Monobehaviour
    protected override void Start()
    {
        base.Start();
        shootAction.SetAltAction(this);
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        ActionStart(onFinish, gridCell);
        shootAction.Resupply();
        CallLog($"{unit.GetName()} reloaded their {shootAction.GetActionName()}");
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        shootAction.OnSelected();
    }

    public void OnUnSelected()
    {
        shootAction.OnUnSelected();
    }

    public override bool CanSelectAction(int currentAP)
    {
        return shootAction.GetClipPercent() != 1 && base.CanSelectAction(currentAP);
    }
    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return new List<GridCell>() { unitCell };
    }
    #endregion

    #region //Enemy action
    /// <summary>
    /// Highly prioritizes as ammo lowers
    /// </summary>
    /// <param name="targetCell"></param>
    /// <param name="unitCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell targetCell, GridCell unitCell)
    {
        float clipPercent = Mathf.Max(0.1f, shootAction.GetClipPercent());
        return new EnemyAIAction(this, unitCell, Mathf.RoundToInt(10 / clipPercent));
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.effectText = "Reload your weapon";
        tooltip.altText = "Switch back to firing";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => $"Reload {shootAction.GetActionName()}";
    public BaseAction GetRootAction() => shootAction;
    #endregion
}