using UnityEngine;

[CreateAssetMenu(fileName = "TargetedVisualType", menuName = "SRPGTutorial/TargetedVisualType", order = 0)]
public class TargetedVisualType : GridVisualType
{
    [Tooltip("Used for target squares of a targeted action")] public Material targetedMaterial;
}
