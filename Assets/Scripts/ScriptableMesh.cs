using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableMesh : MonoBehaviour, ISelectable
{
    public MeshFilter MeshFilter { get; private set; }
    public MeshRenderer MeshRenderer { get; private set; }
    public Mesh Mesh { get; private set; }
    public void Init()
    {
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void AssignMesh(Mesh mesh, Material material)
    {
        MeshFilter.mesh = mesh;
        MeshRenderer.material = material;
        Mesh = mesh;
    }

    public void Move(Vector3 delta)
    {
        transform.position += delta;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsValid()
    {
        return this != null && gameObject != null;
    }
}
