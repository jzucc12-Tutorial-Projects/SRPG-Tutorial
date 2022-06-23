using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNumberText = null;
    [SerializeField] private Button nextTurnButton = null;


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
        nextTurnButton.gameObject.SetActive(TurnSystem.instance.IsPlayerTurn());
        turnNumberText.text = $"Turn Number: {TurnSystem.instance.GetTurnCount()}";
    }
}
