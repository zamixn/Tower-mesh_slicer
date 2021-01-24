using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void AddMultiple<T>(this List<T> list, params T[] elements)
    {
        foreach (var t in elements)
        {
            list.Add(t);
        }
    }

    public static bool ApproximateContains(this List<Vector3> list, Vector3 element)
    {
        foreach (var e in list)
        {
            if (e.Approximately(element))
                return true;
        }
        return false;
    }

    public static int IndexOfApproximate(this List<Vector3> list, Vector3 element)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Approximately(element))
                return i;
        }
        return -1;
    }

    public static int AddAndGetIndex<T>(this List<T> list, T element)
    {
        int index = list.Count;
        list.Add(element);
        return index;
    }

    public static bool Approximately(this Vector3 v, Vector3 other)
    {
        return Mathf.Approximately(v.x, other.x) && Mathf.Approximately(v.y, other.y) && Mathf.Approximately(v.z, other.z);
    }
}
