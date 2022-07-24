using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// UI displayed when a targetable is subject to an attack that relies on accuracy
/// </summary>
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
        AccuracyHub.Targeting += ShowUI;
        AccuracyHub.StopTargeting += HideUI;
    }

    private void OnDisable()
    {
        AccuracyHub.Targeting -= ShowUI;
        AccuracyHub.StopTargeting -= HideUI;
    }
    #endregion

    #region //Updating UI
    private void ShowUI(Dictionary<ITargetable, (int, int, int)> targets)
    {
        if(targets.TryGetValue(targetable, out (int, int, int) values))
        {
            uiContainer.SetActive(true);
            accuracyText.text = $"{values.Item1}%"; //Used in actual builds
            // accuracyText.text = $"{values.Item1}/{values.Item2}%"; //Used to see fudged accuracy when in dev
            critText.text = $"{values.Item3}%";
        }
    }

    private void HideUI()
    {
        uiContainer.SetActive(false);
    }
    #endregion
}