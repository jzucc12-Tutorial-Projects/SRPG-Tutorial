using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Handles UI related to actively select units and actions
/// </summary>
public class UnitActionSystemUI : MonoBehaviour
{
    #region //Variables
    [SerializeField] private ActionButtonUI buttonPrefab = null;
    [SerializeField] private Transform container = null;
    [SerializeField] private TextMeshProUGUI actionPointText = null;
    private List<ActionButtonUI> buttons = new List<ActionButtonUI>();
    private UnitActionSystem unitActionSystem = null;
    private ObjectPool<ActionButtonUI> buttonPool = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        unitActionSystem = FindObjectOfType<UnitActionSystem>();

        foreach(Transform child in container)
            Destroy(child.gameObject);

        buttonPool = new ObjectPool<ActionButtonUI>(
            () => Instantiate(buttonPrefab, container), 
            (button) => button.gameObject.SetActive(true),
            (button) => button.gameObject.SetActive(false),
            (button) => Destroy(button.gameObject), true, 10);
    }

    private void OnEnable()
    {
        UnitActionSystem.UpdateUI += UpdateUI;
        UnitActionSystem.OnSelectedUnitChanged += SetupButtons;
    }

    private void OnDisable()
    {
        UnitActionSystem.UpdateUI -= UpdateUI;
        UnitActionSystem.OnSelectedUnitChanged -= SetupButtons;
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
        actionPointText.text = $"Action Points: {unit.GetAP()}";
    }

    private void SetupButtons(Unit unit)
    {
        foreach(var button in buttons)
            buttonPool.Release(button);

        buttons.Clear();
        if(unit == null) return;

        var actions = unit.GetActions();
        bool lastIsAlt = actions[actions.Length - 1] is IAltAction;
        for(int ii = 0; ii < actions.Length; ii++)
        {
            var button = buttonPool.Get();
            buttons.Add(button);
            button.transform.SetSiblingIndex(ii);

            //Figure out tooltip positioning
            int tooltipPos = 0;
            if(ii > 0)
            {
                bool secondLast = ii == actions.Length - 2;
                bool isLast = ii == actions.Length - 1;
                tooltipPos = (lastIsAlt && secondLast) || isLast ? 2 : 1;
            }
            button.SetAction(unitActionSystem, actions[ii], tooltipPos);
        }
    }

    public void UpdateActionButtons(BaseAction action)
    {
        foreach(var button in buttons)
            button.UpdateUI(action);
    }
    #endregion
}