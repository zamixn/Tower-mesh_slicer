using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableMesh : MonoBehaviour
{
    public MeshFilter MeshFilter { get; private set; }
    public MeshRenderer MeshRenderer { get; private set; }

    public void Init()
    {
        MeshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    public void AssignMesh(Mesh mesh, Material material)
    {
        MeshFilter.mesh = mesh;
        MeshRenderer.material = material;
    }
}
