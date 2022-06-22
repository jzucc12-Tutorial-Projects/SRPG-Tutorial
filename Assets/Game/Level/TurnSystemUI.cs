using TMPro;
using UnityEngine;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI turnNumberText = null;


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
        turnNumberText.text = $"Turn Number: {TurnSystem.instance.GetTurnCount()}";
    }
}
