using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutterTestRunner : MonoBehaviour
{
    [Header("Mesh Generation")]
    [SerializeField] private Material Material;
    [SerializeField] private Vector3 Size = new Vector3(5, 1, 5);
    [SerializeField] private Vector3 Origin = new Vector3(-2, -1, -2);
    [SerializeField] private Vector3 Rotation = new Vector3(0, 0, 0);
    [SerializeField] private bool DestroyOriginalMesh = true;

    [Header("Mesh Cutting")]
    [SerializeField] private Vector3 CuttingPlanePosition = new Vector3(0, 0, 0);
    [SerializeField] private Vector3 CuttingPlaneNormal = new Vector3(1, 1, 0);

    private List<ScriptableMesh> GeneratedMeshes = new List<ScriptableMesh>();

    public void CreateCube()
    {
        CleanUp();
        CubeCreator cubeCreator = new CubeCreator();
        cubeCreator.Add(Size, Origin, Rotation);
        Mesh mesh = cubeCreator.CreateMesh(true);

        var originalMesh = ObjectCreator.CreateSciptableMesh("Cube (Generated)", mesh, Material);
        originalMesh.gameObject.AddComponent<BoxCollider>();
        GeneratedMeshes.Add(originalMesh);
    }

    public void CutMesh()
    {
        if (GeneratedMeshes.Count <= 0)
            return;

        var meshToCut = GeneratedMeshes[0];

        // test out cut
        Mesh[] meshParts = MeshSlicer.Slice(meshToCut.Mesh, CuttingPlanePosition, CuttingPlaneNormal.normalized);
        for (int i = 0; i < meshParts.Length; i++)
        {
            Mesh m = meshParts[i];
            m.RecalculateNormals();
            m.RecalculateBounds();
            m.RecalculateTangents();

            var generatedMesh = ObjectCreator.CreateSciptableMesh($"meshPart{i + 1} (Generated)", m, Material);
            generatedMesh.gameObject.AddComponent<BoxCollider>();
            GeneratedMeshes.Add(generatedMesh);
        }

        if (DestroyOriginalMesh)
            Destroy(meshToCut.gameObject);
    }

    public void CleanUp()
    {
        foreach (var mesh in GeneratedMeshes)
        {
            Destroy(mesh.gameObject);
        }
        GeneratedMeshes.Clear();
    }
}
