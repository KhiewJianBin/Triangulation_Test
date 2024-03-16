using UnityEngine;
using UnityEngine.UI;

using System;
using System.Linq;
using System.Collections.Generic;

using asim.unity.helpers;
using asim.unity.extensions;
using asim.unity.utils.geometry;

public class EarClipping_Example : MonoBehaviour
{
    [SerializeField] Text Text;

    [SerializeField] Camera cam;

    [SerializeField] GameObject Polygon;

    [SerializeField] MeshFilter Meshfilter;

    /// <summary>
    /// The main algorithm of EarCliping Trianglulation
    /// Find a ear of a *Simple Polygon* (no intersecting edges)
    /// returns the 3 vectices of the triangle
    /// </summary>
    (Vector2, Vector2, Vector2) FindEar(List<Vector2> SPolygon)
    {
        int i = 0;

        //infinite loop can be used, because a simple polygon is 100% to have at least 1 ear
        while (true)
        {
            int prevIndex = (int)MathExtensions.fmod(i - 1, SPolygon.Count);
            int currentIndex = i;
            int nextIndex = (int)MathExtensions.fmod(i + 1, SPolygon.Count);

            Vector2 prevVertex = SPolygon[prevIndex];
            Vector2 currentVertex = SPolygon[currentIndex];
            Vector2 nextVertex = SPolygon[nextIndex];

            //Step 1. Check if Pi is convex using oreintation
            int orientation = GeometryUtils.Orientation(prevVertex, currentVertex, nextVertex);
            if (orientation == -1 || orientation == 0)
            {
                //Step 2. Check if the triangle formed, has no other polygon vertices inside
                bool found = false;
                for (int j = 0; j < SPolygon.Count; j++)
                {
                    //2a. if found, inside or on the triangle, check the next vertex
                    if (GeometryUtils.IsPointInTriangle(prevVertex, currentVertex, nextVertex, SPolygon[j]) == 1)
                    {
                        found = true;
                        break;
                    }
                }
                if (found)
                {
                    i = nextIndex;
                    if (i == 0)
                    {
                        Text.text = "Input is not a Simple Polygon";
                        throw new Exception("Input is not a Simple Polygon");
                    }

                    continue;
                }

                return (prevVertex, currentVertex, nextVertex);
            }
            //Step 3. otherwise check the next vertex
            else
            {
                i = nextIndex;
                if (i == 0)
                {
                    Text.text = "Input is not a Simple Polygon";
                    throw new Exception("Input is not a Simple Polygon");
                }
            }
        }
    }

    void Update()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        //Step 0. Collect Vertices of polygon
        List<Vector2> polygon = new List<Vector2>();
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            var vertex = Polygon.transform.GetChild(i).transform.position;
            polygon.Add(vertex);

            vertices.Add(vertex);
        }

        //Step 1. Make sure no duplicate vertices
        List<Vector2> SPolygon = polygon.Distinct().ToList();
        
        while (SPolygon.Count >= 3)
        {
            //Step 2. Find a ear. for each ear found,
            (Vector2 p1, Vector2 p2, Vector2 p3) = FindEar(SPolygon);

            //Step 2b. add to the list of triangles in order
            triangles.Add(vertices.IndexOf(p1));
            triangles.Add(vertices.IndexOf(p2));
            triangles.Add(vertices.IndexOf(p3));

            //Step 2c. remove the vertices and 
            SPolygon.Remove(p2);

            //Step 2d. repeat until no more ear could be found (vertex count < 3)
        }

        //Step 3. create the triangulated mesh
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "EarCliping Triangle Count: " + triangles.Count / 3;
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
    }
}