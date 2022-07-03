using UnityEngine;

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