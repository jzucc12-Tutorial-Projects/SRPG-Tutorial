using UnityEngine;

/// <summary>
/// Materials for grid cells that will display when a given targeted action is selected
/// </summary>
[CreateAssetMenu(fileName = "TargetedVisualType", menuName = "SRPGTutorial/TargetedVisualType", order = 0)]
public class TargetedVisualType : GridVisualType
{
    [Tooltip("Used for target squares of a targeted action")] public Material targetedMaterial;
    [Tooltip("Used for target squares of a targeted action")] public Material targetedMouseMaterial;
}
