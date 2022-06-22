using UnityEngine;

public static class JZVectorExtensions
{
    //Courtesy of the GMTK Discord

    /// <summary>
    /// Allows you to replace the a value in a vector
    /// </summary>
    public static Vector3 Replace(this Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(
            x ?? v.x,
            y ?? v.y,
            z ?? v.z
        );
    }

    /// <summary>
    /// Flatten the Vector along the Y axis.
    /// </summary>
    public static Vector3 Flatten(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }

    /// <summary>
    /// Flatten the Vector along the X axis.
    /// </summary>
    public static Vector3 FlattenX(this Vector3 vector)
    {
        return new Vector3(0f, vector.y, vector.z);
    }

    /// <summary>
    /// Flatten the Vector along the Z axis.
    /// </summary>
    public static Vector3 FlattenZ(this Vector3 vector)
    {
        return new Vector3(vector.x, vector.y, 0f);
    }

    /// <summary>
    /// Flatten the Vector along the specified vector. Vector is assumed to be normalized
    /// </summary>
    /// <param name="axis">Vector to be flattened along. Must be normalized.</param>
    public static Vector3 Flatten(this Vector3 vector, Vector3 axis)
    {
        return vector - (axis * Vector3.Dot(vector, axis));
    }
}
