using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool IsInterface<T>(this GameObject obj, out T t) where T : class
    {
        t = obj.GetComponent<T>();
        return t != null;
    }

    public static string ToString(this Vector3 v, int decimalNumbers = 0)
    {
        string format = "0.";
        for (int i = 0; i < decimalNumbers; i++)
            format += "0";

        return $"({v.x.ToString(format)}, {v.y.ToString(format)}, {v.z.ToString(format)})";
    }

    public static float Sign(this float f)
    {
        return Mathf.Sign(f);
    }
}
