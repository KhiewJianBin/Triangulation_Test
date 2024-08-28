using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections.Generic;
using asim.unity.geometry.triangulation;

/// <summary>
/// Uses Convex Hull Algo
/// </summary>

[ExecuteInEditMode]
public class TriangulationPoints_Incremental_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] GameObject Points;
    [SerializeField] MeshFilter Meshfilter;

    void Update()
    {
        List<Vector3> vertices = new();
        List<int> triangles = new();

        //Step 0. Collect Vertices
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            var vertex = Points.transform.GetChild(i).transform.position;
            vertices.Add(vertex);
        }

        //Step 1. Make sure no duplicate vertices
        vertices = vertices.Distinct().ToList();

        //Step 2. Sort based on x value, perform algorithm
        vertices = vertices.OrderBy(v => v.x).ToList();
        triangles = Triangulation.Incremental(vertices);

        //Step 3. create the triangulated mesh
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Incremental Triangulation Triangle Count: " + triangles.Count / 3;
    }
}