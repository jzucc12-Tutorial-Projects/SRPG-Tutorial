using System;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    #region //Variables
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private float spinAmount = 360f;
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

        if(currentAngle >= spinAmount)
        {
            isActive = false;
            onActionFinish?.Invoke();
        }
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition _gridPosition, Action onFinish)
    {
        isActive = true;
        currentAngle = 0;
        startSpinning = true;
        onActionFinish = onFinish;
    }

    public override List<GridPosition> GetValidPositions()
    {
        return new List<GridPosition>()  { unit.GetGridPosition() };
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Spin";
    #endregion
}