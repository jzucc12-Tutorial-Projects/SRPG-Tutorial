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
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        UnitActionSystem.UpdateUI += UpdateUI;
        UnitActionSystem.OnSelectedUnitChanged += CreateButtons;
    }

    private void OnDisable()
    {
        UnitActionSystem.UpdateUI -= UpdateUI;
        UnitActionSystem.OnSelectedUnitChanged -= CreateButtons;
    }
    #endregion

    #region //UI Updating
    private void UpdateUI(Unit unit, BaseAction action)
    {
        if(unit == null) return;
        UpdateActionPoints(unit);
        UpdateActionButtons(action);
    }

    private void UpdateActionPoints(Unit unit)
    {
        actionPointText.text = $"Action Points: {unit.GetActionPoints()}";
    }

    private void CreateButtons(Unit unit)
    {
        if(unit == null) return;
        buttons.Clear();
        foreach(Transform button in container)
            Destroy(button.gameObject);

        foreach(var action in unit.GetActions())
        {
            var button = Instantiate(buttonPrefab, container);
            button.SetAction(action);
            buttons.Add(button);
        }
    }

    public void UpdateActionButtons(BaseAction action)
    {
        foreach(var button in buttons)
            button.UpdateUI(action);
    }
    #endregion
}