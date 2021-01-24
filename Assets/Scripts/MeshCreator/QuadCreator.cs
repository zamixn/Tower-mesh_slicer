using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadCreator : MeshCreator
{
    private readonly Vector3[] QuadVertices = new Vector3[]
    {
        new Vector3(0, 1), new Vector3(1, 1),
        new Vector3(0, 0), new Vector3(1, 0)
    };
    private readonly Vector2[] QuadUV = new Vector2[]
    {
        new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0)
    };
    private readonly int[] QuadTriangles = new int[]
    {
        0, 1, 2,
        2, 1, 3
    };


    public QuadCreator(MeshCreator meshCreator) : base(meshCreator)
    {
    }

    public override void Add(Vector3 scale, Vector3 origin, Vector3 rotation)
    {
        Quaternion qRot = new Quaternion();
        qRot.eulerAngles = rotation;
        Add(scale, origin, qRot);
    }

    public override void Add(Vector3 scale, Vector3 origin, Quaternion rotation)
    {
        int verticesLength = Vertices.Count;

        for (int i = 0; i < QuadVertices.Length; i++)
            Vertices.Add(TransformVert(QuadVertices[i], scale, origin, rotation));

        for (int i = 0; i < QuadUV.Length; i++)
            Uv.Add(QuadUV[i]);
        for (int i = 0; i < QuadTriangles.Length; i++)
            Triangles.Add(verticesLength + QuadTriangles[i]);
    }
}
