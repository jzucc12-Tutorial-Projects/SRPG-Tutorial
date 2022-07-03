using UnityEngine;

public class MaxAttribute : PropertyAttribute
{
    /// <summary>
    /// Allows you to set a maximum number
    /// </summary>
    public readonly float max;
    public MaxAttribute(float max)
    {
        this.max = max;
    }
}