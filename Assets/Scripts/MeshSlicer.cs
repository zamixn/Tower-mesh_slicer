using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MeshSlicer
{
    public static Mesh[] Slice(Mesh original, Vector3 planePos, Vector3 planeNormal)
    {
        MeshSlicer slicer = new MeshSlicer(original, planePos, planeNormal);
        return slicer.DoSlice();
    }

    private List<Vector3> meshVertices;
    private List<Vector2> meshUv;
    private List<int> meshTriangles;

    private Plane cuttingPlane;

    private List<Vector3> vertices1;
    private List<Vector2> uvs1;
    private List<int> triangles1;

    private List<Vector3> vertices2;
    private List<Vector2> uvs2;
    private List<int> triangles2;

    private List<Vector3> IntersectionPoints;

    private const bool Side1 = true;
    private const bool Side2 = !Side1;

    public MeshSlicer(Mesh original, Vector3 planePos, Vector3 planeNormal)
    {

        meshVertices = original.vertices.ToList();
        meshUv = original.uv.ToList();
        meshTriangles = original.triangles.ToList();

        cuttingPlane = new Plane(planeNormal, planePos);

        vertices1 = new List<Vector3>();
        uvs1 = new List<Vector2>();
        triangles1 = new List<int>();

        vertices2 = new List<Vector3>();
        uvs2 = new List<Vector2>();
        triangles2 = new List<int>();

        IntersectionPoints = new List<Vector3>();
    }


    public Mesh[] DoSlice()
    {
        DebugExtensions.DrawPlane(cuttingPlane.distance * cuttingPlane.normal, cuttingPlane.normal, 10, Color.blue, Color.cyan);

        for (int i = 0; i < meshTriangles.Count; i += 3)
        {
            var t1 = meshTriangles[i];
            var t2 = meshTriangles[i + 1];
            var t3 = meshTriangles[i + 2];

            var v1 = meshVertices[t1];
            var v2 = meshVertices[t2];
            var v3 = meshVertices[t3];

            var uv1 = meshUv[t1];
            var uv2 = meshUv[t2];
            var uv3 = meshUv[t3];

            bool v1Side = cuttingPlane.GetDistanceToPoint(v1) > 0;
            bool v2Side = cuttingPlane.GetDistanceToPoint(v2) > 0;
            bool v3Side = cuttingPlane.GetDistanceToPoint(v3) > 0;

            // if all vertices are on the same side of the cutting plane, add them to the corresponding data
            if (v1Side == v2Side && v1Side == v3Side)
            {
                AddTriangleToSide(v1Side, v1, v2, v3, uv1, uv2, uv3);
            }
            else 
            {
                Vector3 intersection1;
                Vector3 intersection2;

                // if v1 and v2 are on the same side
                if (v1Side == v2Side)
                {
                    GetIntersectionVertexOnPlane(v2, v3, uv2, uv3, cuttingPlane, out intersection1, out Vector2 intersection1Uv);
                    GetIntersectionVertexOnPlane(v3, v1, uv3, uv1, cuttingPlane, out intersection2, out Vector2 intersection2Uv);

                    // add triangles to first side
                    AddTriangleToSide(v1Side, v1, v2, intersection2, uv1, uv2, intersection2Uv);
                    AddTriangleToSide(v1Side, v2, intersection1, intersection2, uv2, intersection1Uv, intersection2Uv);

                    // add triangles to second side
                    AddTriangleToSide(v3Side, intersection2, intersection1, v3, intersection2Uv, intersection1Uv, uv3);
                }
                // if v1 and v3 are on the same side
                else if (v1Side == v3Side)
                {
                    GetIntersectionVertexOnPlane(v1, v2, uv1, uv2, cuttingPlane, out intersection1, out Vector2 intersection1Uv);
                    GetIntersectionVertexOnPlane(v2, v3, uv2, uv3, cuttingPlane, out intersection2, out Vector2 intersection2Uv);

                    // add triangles to first side
                    AddTriangleToSide(v1Side, v1, intersection1, v3, uv1, intersection1Uv, uv3);
                    AddTriangleToSide(v1Side, v3, intersection1, intersection2, uv3, intersection1Uv, intersection2Uv);

                    // add triangles to second side
                    AddTriangleToSide(v2Side, intersection1, v2, intersection2, intersection1Uv, uv2, intersection2Uv);
                }
                // if v1 is the only one on its side
                else //if (v2Side == v3Side)
                {
                    GetIntersectionVertexOnPlane(v1, v2, uv1, uv2, cuttingPlane, out intersection1, out Vector2 intersection1Uv);
                    GetIntersectionVertexOnPlane(v1, v3, uv1, uv3, cuttingPlane, out intersection2, out Vector2 intersection2Uv);

                    // add triangles to first side
                    AddTriangleToSide(v1Side, v1, intersection1, intersection2, uv1, intersection1Uv, intersection2Uv);

                    // add triangles to second side                   
                    AddTriangleToSide(v2Side, intersection1, v2, v3, intersection1Uv, uv2, uv3);
                    AddTriangleToSide(v2Side, intersection1, v3, intersection2, intersection1Uv, uv3, intersection2Uv);
                }

                IntersectionPoints.AddMultiple(intersection1, intersection2);
            }
        }

        // add faces along the plane to make solid meshes
        FillCutFaces();

        Mesh m1 = new Mesh();
        m1.vertices = vertices1.ToArray();
        m1.uv = uvs1.ToArray();
        m1.triangles = triangles1.ToArray();

        Mesh m2 = new Mesh();
        m2.vertices = vertices2.ToArray();
        m2.uv = uvs2.ToArray();
        m2.triangles = triangles2.ToArray();
        return new Mesh[] { m1, m2 };
    }

    private void FillCutFaces()
    {
        // if there was no cut made, return
        if (IntersectionPoints.Count == 0)
            return;

        Vector3 faceMidPoint = Vector3.zero;
        foreach (var point in IntersectionPoints)
            faceMidPoint += point;
        faceMidPoint /= IntersectionPoints.Count;

        for (int i = 0; i < IntersectionPoints.Count; i += 2)
        {
            Vector3 v1 = IntersectionPoints[i];
            Vector3 v2 = IntersectionPoints[i + 1];

            Vector3 normal = ComputeNormal(faceMidPoint, v2, v1).normalized;
            float direction = Vector3.Dot(normal, cuttingPlane.normal);

            if (direction > 0)
            {
                AddTriangleToSide(Side1, faceMidPoint, v1, v2, Vector2.zero, Vector2.zero, Vector2.zero);
                AddTriangleToSide(Side2, faceMidPoint, v2, v1, Vector2.zero, Vector2.zero, Vector2.zero);
            }
            else
            {
                AddTriangleToSide(Side1, faceMidPoint, v2, v1, Vector2.zero, Vector2.zero, Vector2.zero);
                AddTriangleToSide(Side2, faceMidPoint, v1, v2, Vector2.zero, Vector2.zero, Vector2.zero);
            }
        }
    }

    private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 side1 = vertex2 - vertex1;
        Vector3 side2 = vertex3 - vertex1;

        Vector3 normal = Vector3.Cross(side1, side2);

        return normal;
    }

    private void AddTriangleToSide(bool side, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        if (side)
            AddTriangle(ref vertices1, ref uvs1, ref triangles1, v1, v2, v3, uv1, uv2, uv3);
        else
            AddTriangle(ref vertices2, ref uvs2, ref triangles2, v1, v2, v3, uv1, uv2, uv3);
    }

    /// <summary>
    /// Gets the vertex intersecting a plane and line between two verteces
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="uv1"></param>
    /// <param name="uv2"></param>
    /// <param name="plane"></param>
    /// <param name="v"></param>
    /// <param name="uv"></param>
    private void GetIntersectionVertexOnPlane(Vector3 v1, Vector3 v2, Vector2 uv1, Vector2 uv2, Plane plane, out Vector3 v, out Vector2 uv)
    {
        // find intersection
        Ray ray = new Ray(v1, (v2 - v1));
        plane.Raycast(ray, out float distance);
        v = ray.GetPoint(distance);

        // interpolate uv
        uv = Vector2.Lerp(uv1, uv2, distance);
    }

    private void AddTriangle(ref List<Vector3> vertices, ref List<Vector2> uv, ref List<int> triangles,
        Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        int t1 = vertices.AddAndGetIndex(v1);
        int t2 = vertices.AddAndGetIndex(v2);
        int t3 = vertices.AddAndGetIndex(v3);

        uv.Add(uv1);
        uv.Add(uv2);
        uv.Add(uv3);

        triangles.Add(t1);
        triangles.Add(t2);
        triangles.Add(t3);
    }

    /// <summary>
    /// merges vertices that are the same, produces weird uvs
    /// </summary>
    private void AddTriangleSquished(ref List<Vector3> vertices, ref List<Vector2> uv, ref List<int> triangles,
        Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {
        int t1 = vertices.IndexOfApproximate(v1);
        int t2 = vertices.IndexOfApproximate(v2);
        int t3 = vertices.IndexOfApproximate(v3);

        if (t1 == -1)
        {
            t1 = vertices.AddAndGetIndex(v1);
            uv.Add(uv1);
        }
        if (t2 == -1)
        {
            t2 = vertices.AddAndGetIndex(v2);
            uv.Add(uv2);
        }
        if (t3 == -1)
        {
            t3 = vertices.AddAndGetIndex(v3);
            uv.Add(uv3);
        }


        triangles.Add(t1);
        triangles.Add(t2);
        triangles.Add(t3);
    }
}
