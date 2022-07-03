using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNumberText = null;
    [SerializeField] private Button nextTurnButton = null;


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
        nextTurnButton.gameObject.SetActive(isPlayerTurn);
        turnNumberText.text = $"Turn Number: {TurnSystem.instance.GetTurnCount()}";
    }
}
