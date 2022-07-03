using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    #region //Variables
    [Header("Spin Action")]
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private float spinAmount = 360f;
    [SerializeField, MinMax(0, 100)] private int accuracyDrop = 30;
    [SerializeField, Min(0)] private float damageBoost = 0.5f;
    private bool startSpinning = false;
    private float currentAngle = 0;
    #endregion
 
    
    #region //Monobehaviour
    private void Update()
    {
        if(!isActive) return;
        if(startSpinning)
        {
            float spinAdd = spinSpeed * Time.deltaTime;
            currentAngle += spinAdd;
            transform.eulerAngles += new Vector3(0, spinAdd, 0);
        }

        if(currentAngle >= spinAmount) ActionFinish();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition _gridPosition, Action onFinish)
    {
        currentAngle = 0;
        startSpinning = true;
        ActionStart(onFinish);
        unit.AddAccuracyMod(-accuracyDrop);
        unit.AddDamageMod(damageBoost);
    }
    #endregion

    #region //Action selection
    public override List<GridPosition> GetValidPositions()
    {
        return new List<GridPosition>()  { unit.GetGridPosition() };
    }
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 0);
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Spin";
    #endregion
}