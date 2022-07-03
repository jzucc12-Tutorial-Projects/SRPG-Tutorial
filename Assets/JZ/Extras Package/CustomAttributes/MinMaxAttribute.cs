using UnityEngine;

public class MinMaxAttribute : PropertyAttribute
{
    /// <summary>
    /// Allows you to set a maximum number
    /// </summary>
    public readonly float min;
    public readonly float max;
    public MinMaxAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}