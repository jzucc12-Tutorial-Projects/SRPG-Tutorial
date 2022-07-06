using System;
using UnityEngine;

/// <summary>
/// Keeps track of the current turn and changing the turn
/// </summary>
public class TurnSystem : MonoBehaviour
{
    private int turnNumber = 1;
    public static event Action<bool> IncrementTurn;
    private bool isPlayerTurn = true;


    private void Start()
    {
        IncrementTurn?.Invoke(isPlayerTurn);
    }

    public void NextTurn()
    {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;
        IncrementTurn?.Invoke(isPlayerTurn);

        string turnText = isPlayerTurn ? "Player" : "Enemy";
        ActionLogListener.Publish($"It's now the {turnText}'s turn");
    }

    public int GetTurnCount() => turnNumber;
}
