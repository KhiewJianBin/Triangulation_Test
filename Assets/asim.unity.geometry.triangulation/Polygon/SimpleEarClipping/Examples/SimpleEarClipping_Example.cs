using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using asim.unity.geometry;
using asim.unity.geometry.triangulation;

[ExecuteInEditMode]
public class SimpleEarClipping_Example : MonoBehaviour
{
    [SerializeField] Text Text;

    [SerializeField] Camera cam;

    [SerializeField] GameObject Polygon;
    [SerializeField] List<GameObject> PolygonHoles;

    [SerializeField] MeshFilter Meshfilter;

    void Update()
    {
        List<Vector2> polygon = new();
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            var vertex = Polygon.transform.GetChild(i).transform.position;
            polygon.Add(vertex);
        }

        List<List<Vector2>> polygonholes = new();
        foreach (var holes in PolygonHoles)
        {
            List<Vector2> holesverts = new List<Vector2>();
            for (int i = 0; i < holes.transform.childCount; i++)
            {
                var vertex = holes.transform.GetChild(i).transform.position;
                holesverts.Add(vertex);
            }
            if (holesverts.Count < 3) continue;
            polygonholes.Add(holesverts);
        }

        SimplePolygon spolygon = new(polygon,polygonholes);

        try
        {
            List<Vector3> vertices = spolygon.GetVertices();
            List<Vector2> triangles = Triangulation.SimpleEarClipping(spolygon);
            List<int> triangleindex = new();
            for (int i = 0; i < triangles.Count; i++)
            {
                triangleindex.Add(vertices.IndexOf(triangles[i]));
            }

            Mesh mesh = new();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangleindex, 0);
            Meshfilter.mesh = mesh;

            Text.text = "EarCliping Triangle Count: " + triangles.Count / 3;
        }
        catch
        {
            Text.text = "Input Not a Simple Polygon";
        }
    }
}