using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    #region //Variables
    [SerializeField] private TextMeshProUGUI actionText = null;
    [SerializeField] private Button button = null;
    [SerializeField] private Image border = null;
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.white;
    private BaseAction action = null;
    #endregion


    #region //UI Updating
    public void SetAction(BaseAction action)
    {
        this.action = action;
        actionText.text = action.GetActionName().ToUpper();
        button.onClick.AddListener(() => {
            UnitActionSystem.instance.SetSelectedAction(action);
        });
    }

    public BaseAction GetAction() => action;
    public void SetActive() => border.color = activeColor;
    public void SetInactive() => border.color = inactiveColor;
    #endregion
}
