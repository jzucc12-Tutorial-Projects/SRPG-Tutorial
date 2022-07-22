public static class GameGlobals
{
    private static GameMode currentMode = GameMode.team2AI;
    public static void SetMode(GameMode newMode)
    {
        currentMode = newMode;
    }
    public static bool IsThisMode(GameMode mode) => mode == currentMode;
    public static bool IsTeam1AI() => currentMode == GameMode.team1AI;
    public static bool IsTeam2AI() => currentMode == GameMode.team2AI;
    public static bool TwoPlayer() => currentMode == GameMode.twoPlayer;
    public static bool IsAI(bool isTeam1)
    {
        if(GameGlobals.TwoPlayer()) return false;
        else return !(isTeam1 ^ GameGlobals.IsTeam1AI());
    }
}

public enum GameMode
{
    team2AI = 0,
    team1AI = 1,
    twoPlayer = 2
}