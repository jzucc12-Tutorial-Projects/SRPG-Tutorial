using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Shows unit specific UI such as name and health
/// </summary>
public class UnitUI : MonoBehaviour
{
    #region //Unit components
    [SerializeField] private Unit unit = null;
    [SerializeField] private UnitHealth healthSystem = null;
    #endregion

    #region //UI components
    [SerializeField] private TextMeshProUGUI nameText = null;
    [SerializeField] private TextMeshProUGUI actionPointsText = null;
    [SerializeField] private Image healthBar = null;
    #endregion


    #region //Monobehaviour
    private void OnValidate()
    {
        if(!string.IsNullOrEmpty(unit.GetName()))
            nameText.text = unit.GetName();
    }

    private void OnEnable()
    {
        unit.OnActionPointChange += UpdateActionPointsText;
        healthSystem.OnHPChange += OnDamage;
    }

    private void OnDisable()
    {
        unit.OnActionPointChange -= UpdateActionPointsText;
        healthSystem.OnHPChange -= OnDamage;
    }

    private void Start()
    {
        UpdateActionPointsText();
        nameText.text = unit.GetName();
    }
    #endregion

    #region //Update UI
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetAP().ToString();
    }

    private void OnDamage(float percentageLeft)
    {
        healthBar.fillAmount = percentageLeft;
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        UpdateActionPointsText();
        healthBar.fillAmount = healthSystem.GetHealthPercentage();
    }
    #endregion
}
