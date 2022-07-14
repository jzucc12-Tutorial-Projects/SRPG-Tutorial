using UnityEngine;

/// <summary>
/// Alters player accuracy in their favor because humans are bad with P&S
/// </summary>
[CreateAssetMenu(fileName = "FudgeAccuracySO", menuName = "SRPGTutorial/FudgeAccuracySO", order = 0)]
public class FudgeAccuracySO : ScriptableObject
{
    [Header("Bounds")]
    [Tooltip("Lowest fudging amount")] [SerializeField, MinMax(0, 100)] private int lowerLimit = 0;
    [Tooltip("Highest fudge amount. Will to default to accuracy past this")] [SerializeField, MinMax(0, 100)] private int upperLimit = 90;

    [Header("Constant portion")]
    [Tooltip("Fudging will be constant under this input limit")] [SerializeField, MinMax(0, 100)] private int constantThreshold = 10;
    [Tooltip("Fudge amount in the constant region")] [SerializeField, MinMax(0, 100)] private int constantAmount = 5;

    [Header("Decaying portion")]
    [Tooltip("Proportionally affects fudge value. No affect on curve flatness.")] [SerializeField, Min(0)] private float coefficient = 6f;
    [Tooltip("Proportionally affects fudge value. Inversely affects curve flatness.")] [SerializeField, Min(0)] private float numerator = 7f;


    public int GetFudgeAccuracy(int accuracy)
    {
        float fudgeFactor = lowerLimit;
        if(accuracy == 0) fudgeFactor += 0;
        else if(accuracy >= upperLimit) fudgeFactor += 0;
        else if(accuracy < constantThreshold) fudgeFactor += constantAmount;
        else fudgeFactor += coefficient * (1f + numerator/accuracy);
        float fudgedAccuracy = Mathf.Max(accuracy + fudgeFactor, accuracy);
        return Mathf.RoundToInt(Mathf.Min(100, fudgedAccuracy));
    }
}
