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
        ActionStart(onFinish, gridCell);
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
        ActionFinish(new List<GridCell> { unit.GetGridCell() } );
    }
    #endregion

    #region //Action selection
    public override List<GridCell> GetValidCells(GridCell unitCell)
    {
        return new List<GridCell>()  { unitCell };
    }

    public override bool CanSelectAction(int currentAP)
    {
        return currentAP > 1;
    }
    #endregion

    #region //Enemy Action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        if(UnityEngine.Random.Range(0,101) < 95) return UnityEngine.Random.Range(10, 31);
        return UnityEngine.Random.Range(25, 51);
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        tooltip.effectText = "Do a 360 for style!";
        tooltip.costText = "All but 1AP";
        tooltip.damageText = $"+{(damageBoost*100).ToString()}% damage";
        tooltip.accuracyText = $"-{accuracyDrop.ToString()}% accuracy";
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Spin";
    public override int GetAPCost(int currentAP)
    {
        return Mathf.Max(1, currentAP - 1);
    }
    public int GetAccuracyDrop() => -accuracyDrop;
    public float GetDamageBoost() => damageBoost;
    #endregion
}