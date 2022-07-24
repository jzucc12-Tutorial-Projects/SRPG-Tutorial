using System;
using UnityEngine;

/// <summary>
/// Keeps track of the current turn and changing the turn
/// </summary>
public class TurnSystem : MonoBehaviour
{
    #region //Variables
    private int turnNumber = 1;
    public static event Action<bool> IncrementTurn;
    public static event Action<bool> IncrementTurnLate;
    private bool team1Turn = true;
    private bool canChangeTurns = true;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        UnitManager.GameOver += TurnOff;
    }

    private void OnDisable()
    {
        UnitManager.GameOver -= TurnOff;
    }

    private void Start()
    {
        IncrementTurn?.Invoke(team1Turn);
        IncrementTurnLate?.Invoke(team1Turn);    
    }
    #endregion

    #region //Turn Changing
    private void TurnOff()
    {
        canChangeTurns = false;
    }

    public void NextTurn()
    {
        if(!canChangeTurns) return;
        turnNumber++;
        team1Turn = !team1Turn;
        IncrementTurn?.Invoke(team1Turn);
        IncrementTurnLate?.Invoke(team1Turn);    

        string turnText;
        if(GameGlobals.TwoPlayer())
            turnText = team1Turn ? "Player 1" : "Player 2";
        else
            turnText = team1Turn ? "Player" : "Enemy";
        ActionLogListener.Publish($"It's now the {turnText}'s turn");
    }

    public int GetTurnCount() => turnNumber;
    #endregion
}
