using TMPro;
using UnityEngine;

/// <summary>
/// Text entry for the action log
/// </summary>
public class ActionLogEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText = null;

    public void ResetText()
    {
        logText.text = "";
    }

    public void SetText(string text)
    {
        logText.text = text;
    }

    public void CopyEntry(ActionLogEntry destination)
    {
        destination.logText.text = logText.text;
    }
}