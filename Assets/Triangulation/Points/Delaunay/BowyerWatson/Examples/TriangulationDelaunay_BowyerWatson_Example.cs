using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using asim.unity.helpers;
using asim.unity.utils.geometry;
using System;
using asim.unity.geometry.triangulation;

//https://github.com/OskarSigvardsson/unity-delaunay/blob/master/README.org
//https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
//https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/#the-super-triangle code following

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
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Delaunay BowyerWatson Triangulation Triangle Count: " + triangles.Count;
    }
}