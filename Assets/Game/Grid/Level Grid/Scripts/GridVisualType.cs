using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Materials for grid cells that will display when a given action is selected
/// </summary>
[CreateAssetMenu(fileName = "GridVisualType", menuName = "SRPGTutorial/GridVisualType", order = 0)]
public class GridVisualType : ScriptableObject
{
    public List<string> actionNames;
    [Tooltip("Used for all actions")] public Material baseMaterial;
    [Tooltip("Used for mouse hovering")] public Material mouseMaterial;
}