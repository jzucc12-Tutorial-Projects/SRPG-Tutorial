using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Tooltip to appear after you hover over an action button
/// </summary>
public class ActionButtonTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textPrefab = null;
    [SerializeField] private Color headerColor = Color.white;
    private ObjectPool<TextMeshProUGUI> textPool = null;
    private List<TextMeshProUGUI> tmps = new List<TextMeshProUGUI>();


    private void Awake()
    {
        textPool = new ObjectPool<TextMeshProUGUI>(
            () => Instantiate(textPrefab, transform),
            (tmp) => tmp.gameObject.SetActive(true),
            (tmp) => tmp.gameObject.SetActive(false),
            (tmp) => Destroy(tmp.gameObject));

        gameObject.SetActive(false);
    }

    public void SetUp(ActionTooltip so)
    {
        foreach(var tmp in tmps)
            textPool.Release(tmp);

        tmps.Clear();
        SetText(so.effectText, "Effect:");
        SetText(so.altText, "Alt:");
        SetText(so.costText, "Cost:");
        SetText(so.cooldownText, "Cooldown:");
        SetText(so.rangeText, "Range:");
        SetText(so.damageText, "Damage:");
        SetText(so.accuracyText, "Accuracy:");
    }

    private void SetText(string text, string prefix)
    {
        if(string.IsNullOrEmpty(text)) return;
        var tmp = textPool.Get();
        tmp.transform.SetSiblingIndex(tmps.Count);
        tmps.Add(tmp);
        tmp.color = headerColor;
        tmp.text = $"<b>{prefix}</b> <color=#FFFFFF>{text}</color>";
    }
}
