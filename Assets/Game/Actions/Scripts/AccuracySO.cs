using UnityEngine;


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
    //Returns the damage multiplier
    public float ShotHits(Vector3 origin, ITargetable target, bool circularRange)
    {
        return ShotHits(CalculateAccuracy(origin, target, circularRange));
    }

    public float ShotHits(int accuracy)
    {
        int roll = UnityEngine.Random.Range(0, 101);
        if(roll < Mathf.Min(critChance, accuracy)) return critMult;

        int fudgeFactor = 0;
        if(accuracy > 0) fudgeFactor = 12 - (accuracy / 10);
        int fudgedAccuracy = Mathf.Min(100, accuracy + fudgeFactor);
        return roll <= fudgedAccuracy ? 1 : 0;
    }

    public int CalculateAccuracy(Vector3 attackerPosition, ITargetable target, bool circularRange)
    {
        //Set up
        GridPosition attackerGridPosition = LevelGrid.instance.GetGridPosition(attackerPosition);
        int accuracy = baseAccuracy;
        int drop = target is Unit ? accuracyDrop : nonUnitDrop;

        //Distance drop
        int distance = LevelGrid.instance.GetGridDistance(attackerGridPosition, target.GetGridPosition(), circularRange);
        accuracy -= drop * (Mathf.Max(0, distance - nearRange));

        //Blocking obstacle drop
        var attackPos = LevelGrid.instance.GetWorldPosition(attackerGridPosition);
        var aimDir = attackPos - target.GetWorldPosition();
        var hits = Physics.OverlapCapsule(attackerPosition, target.GetTargetPosition(), 0.01f, accuracyObstacleLayer);
        accuracy -= drop * Mathf.Max(0, hits.Length - 2); //Subtract one for the weapon AND the target
        return accuracy;
    }
    #endregion
}