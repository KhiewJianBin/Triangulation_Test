using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;

using asim.unity.helpers;
using asim.unity.geometry.triangulation;
using asim.unity.utils.geometry;

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
        List<Vector2> polygon = new List<Vector2>();
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            var vertex = Polygon.transform.GetChild(i).transform.position;
            polygon.Add(vertex);
        }
        List<List<Vector2>> polygonholes = new List<List<Vector2>>();
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

        SimplePolygon spolygon = new SimplePolygon(polygon,polygonholes);

        try
        {
            List<Vector3> vertices = spolygon.GetVertices();
            List<Vector2> triangles = Triangulation.SimpleEarClipping(spolygon);
            List<int> triangleindex = new List<int>();
            for (int i = 0; i < triangles.Count; i++)
            {
                triangleindex.Add(vertices.IndexOf(triangles[i]));
            }

            Mesh mesh = new Mesh();
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

    void OnGUI()
    {
        //drawing
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            Transform p1 = Polygon.transform.GetChild(i);
            Transform p2 = Polygon.transform.GetChild((i+1) % Polygon.transform.childCount);

            //Convert World Pos to GUI Pos
            Vector3 p1pos = cam.WorldToScreenPoint(p1.position);
            Vector3 p2pos = cam.WorldToScreenPoint(p2.position);
            p1pos.y = UnityOnGUIHelper.Height - p1pos.y;
            p2pos.y = UnityOnGUIHelper.Height - p2pos.y;

            UnityOnGUIHelper.DrawLine(p1pos, p2pos, Color.red, 2);
        }

        foreach (var holes in PolygonHoles)
        {
            for (int i = 0; i < holes.transform.childCount; i++)
            {
                Transform p1 = holes.transform.GetChild(i);
                Transform p2 = holes.transform.GetChild((i + 1) % holes.transform.childCount);

                //Convert World Pos to GUI Pos
                Vector3 p1pos = cam.WorldToScreenPoint(p1.position);
                Vector3 p2pos = cam.WorldToScreenPoint(p2.position);
                p1pos.y = UnityOnGUIHelper.Height - p1pos.y;
                p2pos.y = UnityOnGUIHelper.Height - p2pos.y;

                UnityOnGUIHelper.DrawLine(p1pos, p2pos, Color.red, 2);
            }
        }
    }
}