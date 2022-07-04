using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetingUI : MonoBehaviour
{
    #region //Variables
    ITargetable targetable = null;
    [SerializeField] private GameObject rootObject = null;
    [SerializeField] private GameObject uiContainer = null;
    [SerializeField] private TextMeshProUGUI accuracyText = null;
    [SerializeField] private TextMeshProUGUI critText = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        targetable = rootObject.GetComponent<ITargetable>();
    }

    private void OnEnable()
    {
        ShootAction.Targeting += ShowUI;
        ShootAction.StopTargeting += HideUI;
    }

    private void OnDisable()
    {
        ShootAction.Targeting -= ShowUI;
        ShootAction.StopTargeting -= HideUI;
    }
    #endregion

    #region //Updating UI
    private void ShowUI(List<(ITargetable, int, int)> targets)
    {
        foreach(var target in targets)
        {
            if(target.Item1 != targetable) continue;
            uiContainer.SetActive(true);
            accuracyText.text = $"{target.Item2}%";
            critText.text = $"{target.Item3}%";
            return;
        }
    }

    private void HideUI()
    {
        uiContainer.SetActive(false);
    }
    #endregion
}