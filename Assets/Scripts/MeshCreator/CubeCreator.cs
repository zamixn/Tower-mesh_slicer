using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCreator : MeshCreator
{
    public override void Add(Vector3 scale, Vector3 origin, Vector3 rotation)
    {
        Quaternion qRot = new Quaternion();
        qRot.eulerAngles = rotation;
        Add(scale, origin, qRot);
    }

    public override void Add(Vector3 scale, Vector3 origin, Quaternion rotation)
    {
        Vector3 sideSize = Vector3.one;

        QuadCreator quadCreator = new QuadCreator(this);

        quadCreator.Add(sideSize, new Vector3(0, 0, 0), new Vector3(0, 0, 0));
        quadCreator.Add(sideSize, new Vector3(0, 1, 0), new Vector3(90, 0, 0));
        quadCreator.Add(sideSize, new Vector3(0, 0, 1), new Vector3(0, 90, 0));
        quadCreator.Add(sideSize, new Vector3(1, 0, 0), new Vector3(0, -90, 0));
        quadCreator.Add(sideSize, new Vector3(0, 0, 1), new Vector3(-90, 0, 0));
        quadCreator.Add(sideSize, new Vector3(1, 0, 1), new Vector3(0, 180, 0));

        for (int i = 0; i < Vertices.Count; i++)
            Vertices[i] = TransformVert(Vertices[i], scale, origin, rotation);
    }
}
