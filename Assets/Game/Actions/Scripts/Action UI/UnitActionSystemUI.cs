using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    #region //Variables
    [SerializeField] private ActionButtonUI buttonPrefab = null;
    [SerializeField] private Transform container = null;
    [SerializeField] private TextMeshProUGUI actionPointText = null;
    private List<ActionButtonUI> buttons = new List<ActionButtonUI>();
    private Unit currentUnit = null;
    private BaseAction currentAction = null;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        UnitActionSystem.OnSelectedUnitChanged += ChangeUnit;
        UnitActionSystem.OnSelectedActionChanged += ChangeAction;
        UnitActionSystem.UpdateUI += UpdateUI;
        TurnSystem.instance.IncrementTurn += UpdateUI;
    }

    private void OnDisable()
    {
        UnitActionSystem.OnSelectedUnitChanged -= ChangeUnit;
        UnitActionSystem.OnSelectedActionChanged -= ChangeAction;
        UnitActionSystem.UpdateUI -= UpdateUI;
        TurnSystem.instance.IncrementTurn -= UpdateUI;
    }
    #endregion

    #region //UI Updating
    private void UpdateUI()
    {
        UpdateActionPoints();
        UpdateActionButtons();
    }

    private void ChangeUnit(Unit newUnit)
    {
        currentUnit = newUnit;
        UpdateActionPoints();
        CreateButtons();
    }

    private void UpdateActionPoints()
    {
        actionPointText.text = $"Action Points: {currentUnit.GetActionPoints()}";
    }

    private void CreateButtons()
    {
        buttons.Clear();
        foreach(Transform button in container)
            Destroy(button.gameObject);

        foreach(var action in currentUnit.GetActions())
        {
            var button = Instantiate(buttonPrefab, container);
            button.SetAction(action);
            buttons.Add(button);
        }
    }

    private void ChangeAction(BaseAction newAction)
    {
        currentAction = newAction;
        UpdateActionButtons();
    }

    public void UpdateActionButtons()
    {
        foreach(var button in buttons)
            button.UpdateUI(currentAction);
    }
    #endregion
}