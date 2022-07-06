using UnityEngine;

/// <summary>
/// Holds accuracy data pertaining to a given weapon
/// </summary>
[CreateAssetMenu(fileName = "AccuracySO", menuName = "SRPGTutorial/AccuracySO", order = 0)]
public class AccuracySO : ScriptableObject
{
    #region //Variables
    [Tooltip("Accuracy does not decrease with distance in this range")] [SerializeField] private int nearRange = 3;
    [Tooltip("Base accuracy when in near range")] [SerializeField, MinMax(1, 100)] private int baseAccuracy = 100;
    [Tooltip("Drop per square out of near range and per obstacle between shooter and target.")] [SerializeField, Min(0)] private int accuracyDrop = 5;
    [Tooltip("Colliders that impede shot accuracy.")] [SerializeField] private LayerMask accuracyObstacleLayer = 0;
    [Tooltip("Percent chance of crit. Can't be modified")] [SerializeField, MinMax(0, 100)] private int critChance = 5;
    [Tooltip("Percent chance of crit. Can't be modified")] [SerializeField, Min(1)] private float critMult = 1.5f;
    private int nonUnitDrop => accuracyDrop / 4; //Accuracy drop when aiming at non-unit targets
    #endregion


    #region //Methods
    /// <summary>
    /// <para>Returns the damage multiplier for an attack</para>
    /// <para>0 - Attack misses</para>
    /// <para>1 - Attack hits</para>
    /// <para>1+ - Attack crits</para>
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <param name="circularRange"></param>
    /// <returns></returns>
    public float DamageMult(Vector3 origin, ITargetable target, bool circularRange)
    {
        return DamageMult(CalculateAccuracy(origin, target, circularRange));
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
        if(roll < GetCritChance(accuracy)) return critMult;

        int fudgeFactor = 0;
        if(accuracy > 0) fudgeFactor = 12 - (accuracy / 10);
        int fudgedAccuracy = Mathf.Min(100, accuracy + fudgeFactor);
        return roll <= fudgedAccuracy ? 1 : 0;
    }

    public int CalculateAccuracy(Vector3 attackerPosition, ITargetable target, bool circularRange)
    {
        //Set up
        GridCell attackerGridCell = attackerPosition.GetGridCell();
        int accuracy = baseAccuracy;
        int drop = target is Unit ? accuracyDrop : nonUnitDrop;

        //Distance drop
        int distance = attackerGridCell.GetGridDistance(target.GetGridCell(), circularRange);
        accuracy -= drop * (Mathf.Max(0, distance - nearRange));

        //Blocking obstacle drop
        var aimDir = attackerPosition - target.GetWorldPosition();
        var hits = Physics.OverlapCapsule(attackerPosition, target.GetWorldPosition(), 0.01f, accuracyObstacleLayer);
        accuracy -= drop * Mathf.Max(0, hits.Length - 2); //Subtract one for the weapon AND the target
        return accuracy;
    }

    public int GetCritChance(int accuracy)
    {
        return Mathf.Min(critChance, accuracy);
    }
    #endregion
}