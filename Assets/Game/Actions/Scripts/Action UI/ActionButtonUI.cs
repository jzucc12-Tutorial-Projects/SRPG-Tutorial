using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    #region //Variables
    [SerializeField] private TextMeshProUGUI actionText = null;
    [SerializeField] private TextMeshProUGUI quantityText = null;
    [SerializeField] private Button button = null;
    [SerializeField] private Image border = null;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;
    private BaseAction action = null;
    #endregion


    private void OnEnable()
    {
        BaseAction.OnAnyActionEnded += UpdateUI;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyActionEnded -= UpdateUI;
    }

    #region //UI Updating
    public void SetAction(BaseAction action)
    {
        this.action = action;
        button.onClick.AddListener(() => {
            UnitActionSystem.instance.SetSelectedAction(action);
        });
        UpdateUI(action);
    }

    private void UpdateUI(BaseAction _)
    {
        actionText.text = action.GetActionName().ToUpper();
        quantityText.enabled = action.GetQuantity() != -1;
        quantityText.text = $"x{action.GetQuantity()}";
    }

    public BaseAction GetAction() => action;
    public void SetActive() => border.color = activeColor;
    public void SetInactive() => border.color = inactiveColor;
    #endregion
}
