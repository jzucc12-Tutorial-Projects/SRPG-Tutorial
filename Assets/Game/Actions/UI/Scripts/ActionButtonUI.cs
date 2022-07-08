using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI for an action selection button
/// </summary>
public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private BaseAction action = null;
    private UnitActionSystem uaSystem = null;

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
        this.uaSystem = uaSystem;
        toolTip.SetUp(action.GetToolTip());
        UpdateUI(action);
        if(action is IAltAction) gameObject.SetActive(false);
    }

    public void UpdateUI(BaseAction activeAction)
    {
        actionText.text = action.GetActionName().ToUpper();
        quantityText.enabled = action.GetQuantity() != -1;
        quantityText.text = $"x{action.GetQuantity()}";
        apText.text = $"{action.GetAPCost()} AP";

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

        if(action == activeAction)
            gameObject.SetActive(true);
    }

    public BaseAction GetAction() => action;
    #endregion

    #region //Pointer events
    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Select button if left click
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            uaSystem.SetSelectedAction(action);
        }

        //Swap actions if right click and applicable
        else if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(action.HasAltAction())
            {
                uaSystem.SetSelectedAction(action.GetAltAction());
                gameObject.SetActive(false);
            }
            else if(action is IAltAction)
            {
                var altAction = action as IAltAction;
                uaSystem.SetSelectedAction(altAction.GetRootAction());
                gameObject.SetActive(false);
            }
        }
    }
    #endregion
}