using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI for an action selection button
/// </summary>
public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private BaseAction action = null;

    #region //UI Elements
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI actionText = null;
    [SerializeField] private TextMeshProUGUI quantityText = null;
    [SerializeField] private TextMeshProUGUI apText = null;
    [SerializeField] private Button button = null;
    [SerializeField] private Image border = null;
    [SerializeField] private Image disabledLayer = null;
    [SerializeField] private ActionButtonTooltip toolTip = null;
    #endregion

    #region //Colors
    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private Color disabledColor = Color.red;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        OnPointerExit(null);
    }
    #endregion

    #region //UI Updating
    public void SetAction(UnitActionSystem uaSystem, BaseAction action)
    {
        this.action = action;
        button.onClick.AddListener(() => {
            uaSystem.SetSelectedAction(action);
        });

        toolTip.SetUp(action.GetToolTip());
        UpdateUI(action);
    }

    public void UpdateUI(BaseAction activeAction)
    {
        actionText.text = action.GetActionName().ToUpper();
        quantityText.enabled = action.GetQuantity() != -1;
        quantityText.text = $"x{action.GetQuantity()}";
        apText.text = $"{action.GetPointCost()} AP";

        if(action.CanSelectAction())
        {
            disabledLayer.enabled = false;
            button.interactable = true;
            border.color = action != activeAction ? inactiveColor : activeColor;
        }
        else
        {
            disabledLayer.enabled = true;
            button.interactable = false;
            border.color = disabledColor;
        }
    }

    public BaseAction GetAction() => action;
    #endregion

    #region //Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }
    #endregion
}