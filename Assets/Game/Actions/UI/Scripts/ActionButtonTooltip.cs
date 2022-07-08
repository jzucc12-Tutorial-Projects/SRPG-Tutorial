using TMPro;
using UnityEngine;

/// <summary>
/// Tooltip to appear after you hover over an action button
/// </summary>
public class ActionButtonTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPrefab = null;
    [SerializeField] private Color headerColor = Color.white;


    public void SetUp(ActionTooltip so)
    {
        SetText(so.effectText, "Effect:");
        SetText(so.altText, "Alt:");
        SetText(so.costText, "Cost:");
        SetText(so.rangeText, "Range:");
        SetText(so.damageText, "Damage:");
        SetText(so.accuracyText, "Accuracy:");
        SetText(so.coolDownText, "Cooldown:");
    }

    private void SetText(TextMeshProUGUI tmp, string text, string prefix)
    {
        tmp.text = $"{prefix} {text}";
        tmp.enabled = !string.IsNullOrEmpty(text);
    }

    private void SetText(string text, string prefix)
    {
        if(string.IsNullOrEmpty(text)) return;
        var tmp = Instantiate(textPrefab, transform);
        tmp.color = headerColor;
        tmp.text = $"<b>{prefix}</b> <color=#FFFFFF>{text}</color>";
    }
}
