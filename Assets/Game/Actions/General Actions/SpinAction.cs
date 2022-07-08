using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit can spin to increase damage but decrease accuracy
/// </summary>
public class SpinAction : BaseAction
{
    #region //Variables
    [Header("Spin Action")]
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private float spinAmount = 360f;
    [SerializeField, MinMax(0, 100)] private int accuracyDrop = 30;
    [SerializeField, Min(0)] private float damageBoost = 0.5f;
    #endregion
 

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        ActionStart(onFinish);
        unit.AddAccuracyMod(-accuracyDrop);
        unit.AddDamageMod(damageBoost);
        StartCoroutine(Spin());
        CallLog($"{unit.GetName()} did a 360");
    }

    private IEnumerator Spin()
    {
        float currentAngle = 0;
        while(currentAngle < spinAmount)
        {
            float spinAdd = spinSpeed * Time.deltaTime;
            currentAngle += spinAdd;
            transform.eulerAngles += new Vector3(0, spinAdd, 0);
            yield return null;
        }
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return new List<GridCell>()  { unitCell };
    }

    public override bool CanSelectAction()
    {
        return unit.GetAP() > 1;
    }
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(this, cell, 0);
    }
    #endregion

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        toolTip.effectText = "Do a 360 for style!";
        toolTip.costText = "All but 1AP";
        toolTip.damageText = $"+{(damageBoost*100).ToString()}% damage";
        toolTip.accuracyText = $"-{accuracyDrop.ToString()}% accuracy";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Spin";
    public override int GetAPCost()
    {
        return Mathf.Max(1, unit.GetAP() - 1);
    }
    #endregion
}