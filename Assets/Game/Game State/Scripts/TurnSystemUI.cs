using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles UI for turn changer
/// </summary>
public class TurnSystemUI : MonoBehaviour
{
    #region //Variables
    [SerializeField] private TextMeshProUGUI turnNumberText = null;
    [SerializeField] private Button nextTurnButton = null;
    private TurnSystem turnSystem = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        turnSystem = FindObjectOfType<TurnSystem>();
    }

    private void OnEnable()
    {
        TurnSystem.IncrementTurn += UpdateUI;
        UnitActionSystem.ChangeBusy += HideUI;
    }

    private void OnDisable()
    {
        TurnSystem.IncrementTurn -= UpdateUI;
        UnitActionSystem.ChangeBusy -= HideUI;
    }
    #endregion

    #region //Updating UI
    private void UpdateUI(bool isTeam1)
    {
        HideUI(isTeam1);
        turnNumberText.text = $"Turn Number: {turnSystem.GetTurnCount()}";
    }

    private void HideUI(bool team1Turn)
    {
        nextTurnButton.gameObject.SetActive(!GameGlobals.IsAI(team1Turn));
    }
    #endregion
}
