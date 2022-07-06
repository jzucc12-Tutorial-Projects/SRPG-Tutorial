using System;
using UnityEngine;

/// <summary>
/// Shows information pertaining to recently performed actions
/// </summary>
public class ActionLog : MonoBehaviour
{
    #region //Variables
    private ActionLogEntry[] entries = new ActionLogEntry[0];
    private static Action<string> OnLogAction;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        entries = GetComponentsInChildren<ActionLogEntry>();
    }

    private void OnEnable()
    {
        OnLogAction += AddText;
        ActionLogListener.LogEvents += AddText;
    }

    private void OnDisable()
    {
        OnLogAction -= AddText;
        ActionLogListener.LogEvents -= AddText;
    }

    private void Start()
    {
        foreach(var entry in entries)
            entry.ResetText();
    }
    #endregion

    #region //Logging entries
    public static void LogAction(string text)
    {
        OnLogAction?.Invoke(text);
    }

    public void AddText(string text)
    {
        for(int ii = entries.Length - 2; ii >= 0; ii--)
            entries[ii].CopyEntry(entries[ii+1]);

        entries[0].SetText(text);
    }
    #endregion
}