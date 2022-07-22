using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Takes enemy AI turn
/// </summary>
public class EnemyAIHub : MonoBehaviour
{
    #region //Cached components
    private TurnSystem turnSystem = null;
    private UnitManager unitManager = null;
    public static event Action<Unit> StartNewEnemy;
    #endregion

    #region //Turn taking variables
    [SerializeField] private float startUpTime = 0.5f;
    [SerializeField] private float waitTime = 2;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
        unitManager = FindObjectOfType<UnitManager>();
        if(GameGlobals.TwoPlayer()) enabled = false;
    }

    private void OnEnable()
    {
        TurnSystem.IncrementTurn += OnTurnChange;
    }

    private void OnDisable()
    {
        TurnSystem.IncrementTurn -= OnTurnChange;
    }
    #endregion

    #region //Turns
    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(startUpTime);

        var units = GameGlobals.IsTeam1AI() ? unitManager.GetTeam1List() : unitManager.GetTeam2List();
        var enemies = new List<Unit>();
        foreach(var unit in units)
            enemies.Add(unit);

        foreach(Unit enemy in enemies)
        {
            StartNewEnemy?.Invoke(enemy);
            var ai = enemy.GetComponent<EnemyAI>();
            yield return StartCoroutine(ai.TakeTurn(waitTime));
        }

        turnSystem.NextTurn();
    }

    private void OnTurnChange(bool team1Turn)
    {
        if(!GameGlobals.IsAI(team1Turn)) return;
        StartCoroutine(EnemyTurn());
    }
    #endregion
}