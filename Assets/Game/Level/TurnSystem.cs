using System;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem instance { get; private set; }
    private int turnNumber = 1;
    public event Action IncrementTurn;

    
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        IncrementTurn?.Invoke();
    }

    public void NextTurn()
    {
        turnNumber++;
        IncrementTurn?.Invoke();
    }

    public int GetTurnCount() => turnNumber;
}
