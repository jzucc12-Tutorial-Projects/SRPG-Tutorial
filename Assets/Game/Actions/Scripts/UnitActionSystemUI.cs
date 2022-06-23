using System;
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
        UnitActionSystem.instance.OnSelectedUnitChanged += OnUnitChange;
        UnitActionSystem.instance.OnSelectedActionChanged += ChangeSelectedAction;
        UnitActionSystem.instance.actionTaken += UpdateActionPoints;
        TurnSystem.instance.IncrementTurn += UpdateActionPoints;
    }

    private void OnDisable()
    {
        UnitActionSystem.instance.OnSelectedUnitChanged -= OnUnitChange;
        UnitActionSystem.instance.OnSelectedActionChanged -= ChangeSelectedAction;
        UnitActionSystem.instance.actionTaken -= UpdateActionPoints;
        TurnSystem.instance.IncrementTurn -= UpdateActionPoints;
    }

    private void Start()
    {
        UpdateActionPoints();
        CreateButtons();
        ChangeSelectedAction();
    }
    #endregion

    #region //UI Updating
    private void OnUnitChange(object sender, EventArgs e)
    {
        UpdateActionPoints();
        CreateButtons();
        ChangeSelectedAction();
    }

    private void UpdateActionPoints()
    {
        actionPointText.text = $"Action Points: {UnitActionSystem.instance.GetSelectedUnit().GetActionPoints()}";
    }

    private void CreateButtons()
    {
        buttons.Clear();
        foreach(Transform button in container)
            Destroy(button.gameObject);

        var unit = UnitActionSystem.instance.GetSelectedUnit();
        foreach(var action in unit.GetActions())
        {
            var button = Instantiate(buttonPrefab, container);
            button.SetAction(action);
            buttons.Add(button);
        }
    }

    public void ChangeSelectedAction()
    {
        foreach(var button in buttons)
        {
            if(button.GetAction() == UnitActionSystem.instance.GetSelectedAction()) button.SetActive();
            else button.SetInactive();
        }
    }
    #endregion
}
