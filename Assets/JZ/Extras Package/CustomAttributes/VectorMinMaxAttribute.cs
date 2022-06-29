using UnityEngine;

public class VectorMinMaxAttribute : PropertyAttribute
{
    /// <summary>
    /// <para>Allows you to set limits on a given vector field</para>
    /// </summary>
    public readonly float min;
    public readonly float max;
    public VectorMinMaxAttribute(float min, float max)
    {
        this.min = min;
        this.max = max;
    }
}