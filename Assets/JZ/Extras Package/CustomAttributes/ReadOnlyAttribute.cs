using UnityEngine;

public class ReadOnlyAttribute : PropertyAttribute
{
    /// <summary>
    /// <para>Field can't be edited in the inspector</para>
    /// </summary>
    public ReadOnlyAttribute()
    {
    }
}