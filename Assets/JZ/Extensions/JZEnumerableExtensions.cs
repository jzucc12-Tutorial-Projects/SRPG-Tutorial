using System.Collections.Generic;
using UnityEngine;

public static class JZEnumerableExtensions
{
    /// <summary>
    /// Returns a random entry from a list or array
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomEntry<T>(this T[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
    public static T GetRandomEntry<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
