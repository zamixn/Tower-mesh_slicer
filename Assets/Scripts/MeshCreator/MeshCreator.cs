using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshCreator
{
    protected Mesh Mesh;
    protected List<Vector3> Vertices;
    protected List<Vector2> Uv;
    protected List<int> Triangles;

    public abstract void Add(Vector3 scale, Vector3 origin, Vector3 rotation);
    public abstract void Add(Vector3 scale, Vector3 origin, Quaternion rotation);


    public MeshCreator()
    {
        Mesh = new Mesh();
        Vertices = new List<Vector3>();
        Uv = new List<Vector2>();
        Triangles = new List<int>();
    }

    public MeshCreator(MeshCreator mesh)
    {
        Mesh = mesh.Mesh;
        Vertices = mesh.Vertices;
        Uv = mesh.Uv;
        Triangles = mesh.Triangles;
    }

    public Mesh CreateMesh(bool recalculateMetadata = false)
    {
        Mesh.vertices = Vertices.ToArray();
        Mesh.uv = Uv.ToArray();
        Mesh.triangles = Triangles.ToArray();
        if (recalculateMetadata)
        {
            Mesh.RecalculateNormals();
            Mesh.RecalculateBounds();
            Mesh.RecalculateTangents();
        }
        return Mesh;
    }

    public static Vector3 TransformVert(Vector3 v, Vector3 scale, Vector3 origin, Quaternion rotation)
    {
        Vector3 vert = new Vector3(v.x * scale.x, v.y * scale.y, v.z * scale.z);
        vert = rotation * vert;
        return vert + origin;
    }

    public static Vector3 TransformVert(Vector3 v, Vector3 scale, Vector3 origin, Vector3 rotation)
    {
        Quaternion q = new Quaternion();
        q.eulerAngles = rotation;
        return TransformVert(v, scale, origin, q);
    }
}

