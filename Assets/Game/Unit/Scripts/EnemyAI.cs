using System;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    #region //Timers
    private float maxTime = 1;
    private float timer = 0;
    #endregion

    private TurnSystem turnSystem = null;
    private UnitManager unitManager = null;

    #region //State
    private bool isPlayerTurn = true;
    private enum State
    {
        WaitingForTurn,
        TakingTurn,
        Busy
    }
    private State currentState = State.WaitingForTurn;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        unitManager = FindObjectOfType<UnitManager>();
    }

    private void OnEnable()
    {
        TurnSystem.IncrementTurn += OnTurnChange;
    }

    private void OnDisable()
    {
        TurnSystem.IncrementTurn += OnTurnChange;
    }

    private void Start()
    {
        timer = maxTime;
    }

    void Update()
    {
        if(isPlayerTurn) return;

        switch(currentState)
        {
            case State.WaitingForTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if(timer <= 0) 
                {
                    if(TryEnemyActions(SetStateTakingTurn))
                        currentState = State.Busy;
                    else
                        turnSystem.NextTurn();
                }
                break;
            case State.Busy:
                break;
        }
    }
    #endregion

    #region //Turns
    private void OnTurnChange(bool isPlayerTurn)
    {
        this.isPlayerTurn = isPlayerTurn;
        if(isPlayerTurn) return;
        currentState = State.TakingTurn;
        timer = maxTime;
    }

    private void SetStateTakingTurn()
    {
        timer = 1f;
        currentState = State.TakingTurn;

    }
    #endregion

    #region //Action taking
    private bool TryEnemyActions(Action onActionComplete)
    {
        foreach(var enemy in unitManager.GetEnemyList())
        {
            if(!TryTakeAction(enemy, onActionComplete)) continue;
            return true;
        }
        return false;
    }

    private bool TryTakeAction(Unit enemy, Action onActionComplete)
    {
        EnemyAIAction enemyAction = null;
        BaseAction selectedAction = null;

        foreach(var action in enemy.GetActions())
        {
            if(!enemy.CanTakeAction(action)) continue;
            if(enemyAction == null) 
            {
                enemyAction = action.GetBestAIAction();
                selectedAction = action;
            }
            else
            {
                var testAction = action.GetBestAIAction();
                if(testAction != null && testAction.actionValue > enemyAction.actionValue)
                {
                    enemyAction = action.GetBestAIAction();
                    selectedAction = action;
                }
            }
        }

        if(selectedAction == null) return false;
        if(!enemy.TryTakeAction(selectedAction)) return false;
        selectedAction.TakeAction(enemyAction.gridCell, onActionComplete);
        return true;
    }
    #endregion
}