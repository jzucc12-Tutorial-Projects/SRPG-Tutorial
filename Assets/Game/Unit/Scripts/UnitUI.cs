using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    #region //Unit components
    [SerializeField] private Unit unit = null;
    [SerializeField] private HealthSystem healthSystem = null;
    #endregion

    #region //UI components
    [SerializeField] private TextMeshProUGUI actionPointsText = null;
    [SerializeField] private Image healthBar = null;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        unit.OnActionPointChange += UpdateActionPointsText;
        healthSystem.OnDamage += OnDamage;
    }

    private void OnDisable()
    {
        unit.OnActionPointChange -= UpdateActionPointsText;
        healthSystem.OnDamage -= OnDamage;
    }

    private void Start()
    {
        UpdateActionPointsText();
    }
    #endregion

    #region //Update UI
    private void UpdateActionPointsText()
    {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void OnDamage(float percentageLeft)
    {
        healthBar.fillAmount = percentageLeft;
    }
    #endregion
}
