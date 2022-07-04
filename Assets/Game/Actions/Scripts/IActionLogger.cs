using System;

public static class ActionLogListener
{
    public static event Action<string> LogEvents;

    public static void Publish(string text)
    {
        LogEvents?.Invoke(text);
    }
}