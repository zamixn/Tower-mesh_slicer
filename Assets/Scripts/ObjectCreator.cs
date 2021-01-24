using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private Material Material;

    private void Start()
    {
        Vector3 size = new Vector3(5, 1, 5);
        Vector3 origin = new Vector3(-2, -1, -2);
        Vector3 rotation = new Vector3(0, 0, 0);

        CubeCreator cubeCreator = new CubeCreator();
        cubeCreator.Add(size, origin, rotation);
        Mesh mesh = cubeCreator.CreateMesh(true);

        CreateSciptableMesh("GeneratedObject", mesh, Material);

        // test out cut
        Vector3 planePos = new Vector3(0, 0, 0);
        Vector3 planeNormal = new Vector3(1, 1, 0).normalized;
        Mesh[] meshParts = MeshSlicer.Slice(mesh, planePos, planeNormal);
        for (int i = 0; i < meshParts.Length; i++)
        {
            Mesh m = meshParts[i];
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();

            CreateSciptableMesh($"meshPart{i + 1}(Generated)", m, Material);
        }
    }

    public ScriptableMesh CreateSciptableMesh(string name, Mesh mesh, Material material)
    {
        GameObject go = new GameObject(name);
        ScriptableMesh scriptableMesh = go.AddComponent<ScriptableMesh>();
        scriptableMesh.Init();
        scriptableMesh.AssignMesh(mesh, material);
        return scriptableMesh;
    }
}