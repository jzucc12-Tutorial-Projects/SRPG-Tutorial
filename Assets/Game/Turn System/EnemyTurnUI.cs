using UnityEngine;

public class EnemyTurnUI : MonoBehaviour
{
    [SerializeField] private GameObject enemyTurnUI = null;


    private void OnEnable()
    {
        TurnSystem.instance.IncrementTurn += UpdateUI;
    }

    private void OnDisable()
    {
        TurnSystem.instance.IncrementTurn -= UpdateUI;
    }

    private void UpdateUI()
    {
        enemyTurnUI.SetActive(!TurnSystem.instance.IsPlayerTurn());
    }
}