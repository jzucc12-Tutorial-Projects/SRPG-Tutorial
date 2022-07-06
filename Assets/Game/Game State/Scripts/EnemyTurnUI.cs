using UnityEngine;

/// <summary>
/// Displays UI covering the player buttons on the enemy's turn
/// </summary>
public class EnemyTurnUI : MonoBehaviour
{
    [SerializeField] private GameObject enemyTurnUI = null;


    private void OnEnable()
    {
        TurnSystem.IncrementTurn += UpdateUI;
    }

    private void OnDisable()
    {
        TurnSystem.IncrementTurn -= UpdateUI;
    }

    private void UpdateUI(bool isPlayerTurn)
    {
        enemyTurnUI.SetActive(!isPlayerTurn);
    }
}