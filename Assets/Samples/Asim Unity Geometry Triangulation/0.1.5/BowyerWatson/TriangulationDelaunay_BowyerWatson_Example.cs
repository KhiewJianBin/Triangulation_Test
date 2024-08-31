using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using asim.unity.geometry.triangulation;

[ExecuteInEditMode]
public class TriangulationDelaunay_BowyerWatson_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] GameObject Points;
    [SerializeField] MeshFilter Meshfilter;

    void Update()
    {
        List<Vector3> vertices = new();
        List<Vector2> points = new();

        //Step 0. Collect Vertices
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            var vertex = Points.transform.GetChild(i).transform.position;
            vertices.Add(vertex);
            points.Add(vertex);
        }

        //Step 1. Perform BowyerWatson
        var triangles = Triangulation.Delaunay_BowyerWatson(points);

        //Step 2.create the mesh
        Mesh mesh = new();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Delaunay BowyerWatson Triangulation Triangle Count: " + triangles.Count;
    }
}