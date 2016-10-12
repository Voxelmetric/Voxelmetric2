using UnityEngine;
using System.Collections.Generic;

public class MeshData {

    public List<Vector3> verts = new List<Vector3>();
    public List<int> tris = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();

    public List<Vector3> colVerts = new List<Vector3>();
    public List<int> colTris = new List<int>();

    /// <summary>
    /// Should clear all details out of the mesh data object
    /// </summary>
    public void Clear()
    {
        verts = new List<Vector3>();
        colVerts = new List<Vector3>();
        tris = new List<int>();
        colTris = new List<int>();
        uvs = new List<Vector2>();
    }
}
