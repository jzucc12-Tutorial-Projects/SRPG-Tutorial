using System;

/// <summary>
/// Event aggregator for action log entries
/// </summary>
public static class ActionLogListener
{
    public static event Action<string> LogEvents;

    public static void Publish(string text)
    {
        LogEvents?.Invoke(text);
    }
}