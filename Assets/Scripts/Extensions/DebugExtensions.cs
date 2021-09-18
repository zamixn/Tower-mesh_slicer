using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugExtensions
{
    public static void DrawPlane(Vector3 planePos, Vector3 planeNormal, float duration, Color planeColor, Color normalColor)
    {
        Vector3 v3;

        if (planeNormal.normalized != Vector3.forward)
            v3 = Vector3.Cross(planeNormal, Vector3.forward).normalized * planeNormal.magnitude;
        else
            v3 = Vector3.Cross(planeNormal, Vector3.up).normalized * planeNormal.magnitude; ;

        var corner0 = planePos + v3;
        var corner2 = planePos - v3;
        var q = Quaternion.AngleAxis(90.0f, planeNormal);
        v3 = q * v3;
        var corner1 = planePos + v3;
        var corner3 = planePos - v3;

        Debug.DrawLine(corner0, corner2, planeColor, duration);
        Debug.DrawLine(corner1, corner3, planeColor, duration);
        Debug.DrawLine(corner0, corner1, planeColor, duration);
        Debug.DrawLine(corner1, corner2, planeColor, duration);
        Debug.DrawLine(corner2, corner3, planeColor, duration);
        Debug.DrawLine(corner3, corner0, planeColor, duration);
        Debug.DrawRay(planePos, planeNormal, normalColor, duration);
    }

    public static void DrawPoint(Vector3 pos, float duration, Color color, float radius = .1f)
    {
        Debug.LogWarning(pos);
        float maxAngle = Mathf.PI * 2;
        float deltaAngle = Mathf.PI / (2 * (radius * 20));
        for (float ax = 0; ax < maxAngle; ax += deltaAngle)
        {
            for (float ay = 0; ay < maxAngle; ay += deltaAngle)
            {
                Vector3 direction = new Vector3(Mathf.Cos(ax) * Mathf.Sin(ay), Mathf.Sin(ax), Mathf.Cos(ax) * Mathf.Cos(ay));
                direction = direction.normalized * radius;
                Debug.DrawRay(pos, direction, color, duration);
                Debug.DrawRay(pos, -direction, color, duration);
            }
        }
        
    }

    public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
        Debug.DrawRay(pos, direction, color);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
        Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
    }
}
