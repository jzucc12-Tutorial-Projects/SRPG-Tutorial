using UnityEngine;

/// <summary>
/// Holds accuracy data pertaining to a given weapon
/// </summary>
[CreateAssetMenu(fileName = "AccuracySO", menuName = "SRPGTutorial/AccuracySO", order = 0)]
public class AccuracySO : ScriptableObject
{
    #region //Variables
    [Header("Accuracy and drop values")]
    [Tooltip("Base accuracy when in near range")] [SerializeField, MinMax(1, 100)] private int baseAccuracy = 100;
    [Tooltip("Accuracy drop multiplier with non-unit targets")] [SerializeField] private float nonUnitMult = 0.75f;
    [Tooltip("How the accuracy system modifies accuracy in the player's favor")]
    [SerializeField, ScriptableObjectDropdown(typeof(FudgeAccuracySO))] FudgeAccuracySO fudgeSO = null;

    [Header("Distance drop")]
    [Tooltip("Should accuracy drop with distance?")] [SerializeField] private bool dropWithDistance = true;
    [Tooltip("Drop per square out of near range.")] [ShowIf("dropWithDistance"), SerializeField, Min(0)] private int distanceDrop = 5;
    [Tooltip("Accuracy does not decrease with distance in this range")] [ShowIf("dropWithDistance"), SerializeField] private int nearRange = 3;

    [Header("Blocking drop")]
    [Tooltip("Should accuracy drop with obstacles?")] [SerializeField] private bool dropWithBlocking = true;
    [Tooltip("Drop per obstacle between shooter and target.")] [ShowIf("dropWithBlocking"), SerializeField, Min(0)] private int blockingDrop = 5;
    [Tooltip("Colliders that impede shot accuracy.")] [ShowIf("dropWithBlocking"), SerializeField] private LayerMask accuracyBlockingLayer = 0;

    [Header("Health drop")]
    [Tooltip("Should accuracy with this weapon drop with health?")] [SerializeField] private bool dropWithHealth = true;
    [Tooltip("Percentage of max hp missing that causes accuracy drop.")] [ShowIf("dropWithHealth"), SerializeField, MinMax(0, 1f)] private float healthTick = 5;
    [Tooltip("Drop per missing health tick")] [ShowIf("dropWithHealth"), SerializeField, Min(0)] private int healthDrop = 2;


    [Header("Crit")]
    [Tooltip("Percent chance of crit. Can't be modified")] [SerializeField, MinMax(0, 100)] private int critChance = 5;
    [Tooltip("Crit damage multiplier")] [SerializeField, Min(1)] private float critMult = 1.5f;
    #endregion


    #region //Methods
    /// <summary>
    /// <para>Returns the damage multiplier for an attack</para>
    /// <para>0 - Attack misses</para>
    /// <para>1 - Attack hits</para>
    /// <para>1+ - Attack crits</para>
    /// </summary>
    /// <param name="attacker"></param>
    /// <param name="attackerPosition"></param>
    /// <param name="target"></param>
    /// <param name="circularRange"></param>
    /// <returns></returns>
    public float DamageMult(Unit attacker, Vector3 attackerPosition, ITargetable target)
    {
        return DamageMult(CalculateAccuracy(attacker, attackerPosition, target));
    }

    /// <summary>
    /// <para>Returns the damage multiplier for an attack</para>
    /// <para>0 - Attack misses</para>
    /// <para>1 - Attack hits</para>
    /// <para>1+ - Attack crits</para>
    /// </summary>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public float DamageMult(int accuracy)
    {
        int roll = UnityEngine.Random.Range(0, 101);
        if (roll < CalculateCritChance(accuracy)) return critMult;
        else if (accuracy == 100) return 1;
        return roll < FudgeAccuracy(accuracy)? 1 : 0;
    }

    /// <summary>
    /// Offsets accuracy because people are bad with probability.
    /// Fudge equation generated in excel spreadsheet
    /// </summary>
    /// <param name="accuracy"></param>
    /// <returns></returns>
    public int FudgeAccuracy(int accuracy)
    {
        if(fudgeSO == null) return accuracy;
        else return fudgeSO.GetFudgeAccuracy(accuracy);
    }

    public int CalculateAccuracy(Unit attacker, Vector3 attackerPosition, ITargetable target)
    {
        //Set up
        GridCell attackerGridCell = attackerPosition.GetGridCell();
        int accuracy = baseAccuracy;
        bool isUnit = target is Unit;
        int drop = 0;

        //Distance drop
        if(dropWithDistance)
        {
            int distance = attackerGridCell.GetGridDistance(target.GetGridCell(), false);
            drop += distanceDrop * (Mathf.Max(0, distance - nearRange));
        }

        //Blocking obstacle drop
        if(dropWithBlocking)
        {
            var aimDir = attackerPosition - target.GetWorldPosition();
            var hits = Physics.OverlapCapsule(attackerPosition, target.GetWorldPosition(), 0.01f, accuracyBlockingLayer);
            drop += blockingDrop * Mathf.Max(0, hits.Length - 2); //Subtract one for the weapon AND the target
        }
        if(!isUnit) 
            drop = Mathf.RoundToInt(drop * nonUnitMult);

        //Unit health drop
        if(dropWithHealth)
        {
            float hpMissing = 1 - attacker.GetHealthPercentage();
            accuracy += healthDrop * Mathf.RoundToInt(hpMissing / healthTick);
        }

        return accuracy - drop + attacker.GetAccuracyMod();
    }

    public int CalculateCritChance(int accuracy)
    {
        return Mathf.Min(critChance, accuracy);
    }
    #endregion

    #region //Getters
    public int GetBaseAccuracy() => baseAccuracy;
    public int GetCritChance() => critChance;
    public float GetCritMult() => critMult;
    public bool IsNearRange(GridCell start, GridCell end)
    {
        return start.GetGridDistance(end, true) <= nearRange;
    }
    #endregion
}