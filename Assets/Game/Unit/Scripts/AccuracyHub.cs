using System;
using System.Collections.Generic;

public static class AccuracyHub
{
    //The ints are accuracy, fudged accuracy, and crit chance
    public static event Action<Dictionary<ITargetable, (int, int, int)>> Targeting;
    public static event Action StopTargeting;


    public static void ShowAccuracyUI(Unit attacker, List<GridCell> targetCells, AccuracySO accuracySO, LevelGrid levelGrid)
    {
        Dictionary<ITargetable, (int, int, int)> targets = new Dictionary<ITargetable, (int, int, int)>();
        foreach(var gridCell in targetCells)
        {
            ITargetable target = levelGrid.GetTargetable(gridCell);
            if(targets.ContainsKey(target)) continue;
            int accuracy = accuracySO.CalculateAccuracy(attacker, attacker.GetWorldPosition(), target);
            int fudgeAccuracy = accuracySO.FudgeAccuracy(accuracy);
            int crit = accuracySO.CalculateCritChance(accuracy);
            targets.Add(target, (accuracy, fudgeAccuracy, crit));
        }
        Targeting?.Invoke(targets);
    }

    public static void HideAccuracyUI() => StopTargeting?.Invoke();
}