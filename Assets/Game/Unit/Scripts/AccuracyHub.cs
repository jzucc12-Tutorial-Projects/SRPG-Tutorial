using System;
using System.Collections.Generic;

public static class AccuracyHub
{
    public static event Action<Dictionary<ITargetable, (int, int)>> Targeting;
    public static event Action StopTargeting;


    public static void ShowAccuracyUI(Unit attacker, List<GridCell> targetCells, AccuracySO accuracySO)
    {
        Dictionary<ITargetable, (int, int)> targets = new Dictionary<ITargetable, (int, int)>();
        foreach(var gridCell in targetCells)
        {
            ITargetable target = gridCell.GetTargetable();
            int accuracy = accuracySO.CalculateAccuracy(attacker, attacker.GetWorldPosition(), target);
            int crit = accuracySO.CalculateCritChance(accuracy);
            targets.Add(target, (accuracy, crit));
        }
        Targeting?.Invoke(targets);
    }

    public static void HideAccuracyUI() => StopTargeting?.Invoke();
}