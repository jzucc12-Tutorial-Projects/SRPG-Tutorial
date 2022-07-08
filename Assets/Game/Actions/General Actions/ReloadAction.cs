using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Alt action that reloads a given weapon
/// </summary>
public class ReloadAction : BaseAction, IAltAction
{
    #region //Variables
    [SerializeField] private ShootAction shootAction = null;
    [SerializeField] private string displayText = "";
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
        ActionStart(onFinish);
        shootAction.Resupply();
        CallLog($"{unit.GetName()} reloaded their {shootAction.GetActionName()}");
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override void OnSelected()
    {
        shootAction.OnSelected();
    }

    public override void OnUnSelected()
    {
        shootAction.OnUnSelected();
    }

    public override bool CanSelectAction()
    {
        return shootAction.GetClipPercent() != 1 && base.CanSelectAction();
    }
    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return new List<GridCell>() { unitCell };
    }
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(this, cell, 0);
    }
    #endregion

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        base.SetUpToolTip();
        toolTip.effectText = "Reload your weapon";
        toolTip.altText = "Switch back to firing";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => displayText;
    public BaseAction GetRootAction() => shootAction;
    #endregion
}