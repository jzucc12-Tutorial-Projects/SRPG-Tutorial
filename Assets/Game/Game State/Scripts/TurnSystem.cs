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
    private bool isPlayerTurn = true;
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
        IncrementTurn?.Invoke(isPlayerTurn);
        IncrementTurnLate?.Invoke(isPlayerTurn);    
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
        isPlayerTurn = !isPlayerTurn;
        IncrementTurn?.Invoke(isPlayerTurn);
        IncrementTurnLate?.Invoke(isPlayerTurn);    

        string turnText = isPlayerTurn ? "Player" : "Enemy";
        ActionLogListener.Publish($"It's now the {turnText}'s turn");
    }

    public int GetTurnCount() => turnNumber;
    #endregion
}
