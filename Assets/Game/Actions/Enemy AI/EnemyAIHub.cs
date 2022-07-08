using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes enemy AI turn
/// </summary>
public class EnemyAIHub : MonoBehaviour
{
    #region //Cached components
    private TurnSystem turnSystem = null;
    private UnitManager unitManager = null;
    private List<Unit> enemies => new List<Unit>(unitManager.GetEnemyList());
    #endregion

    #region //Turn taking variables
    [SerializeField] private float startUpTime = 0.5f;
    [SerializeField] private float waitTime = 2;
    private bool isPlayerTurn = true;
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
    #endregion

    #region //Turns
    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(startUpTime);

        foreach(Unit enemy in enemies)
        {
            var ai = enemy.GetComponent<EnemyAI>();
            yield return StartCoroutine(ai.TakeTurn(waitTime));
        }

        turnSystem.NextTurn();
    }

    private void OnTurnChange(bool isPlayerTurn)
    {
        this.isPlayerTurn = isPlayerTurn;
        if(isPlayerTurn) return;
        StartCoroutine(EnemyTurn());
    }
    #endregion
}