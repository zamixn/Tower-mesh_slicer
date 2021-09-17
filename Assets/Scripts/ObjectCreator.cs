using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectCreator
{
    public static ScriptableMesh CreateSciptableMesh(string name, Mesh mesh, Material material)
    {
        GameObject go = new GameObject(name);
        ScriptableMesh scriptableMesh = go.AddComponent<ScriptableMesh>();
        scriptableMesh.Init();
        scriptableMesh.AssignMesh(mesh, material);
        return scriptableMesh;
    }
}