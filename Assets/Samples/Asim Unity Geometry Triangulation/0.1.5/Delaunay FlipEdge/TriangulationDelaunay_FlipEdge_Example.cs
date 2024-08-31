using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using asim.unity.geometry;
using asim.unity.geometry.triangulation;

[ExecuteInEditMode]
public class TriangulationDelaunay_FlipEdge_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] GameObject Points;
    [SerializeField] MeshFilter Meshfilter;

    public bool flip = true;

    void Update()
    {
        List<Vector3> vertices = new();

        //Step 0. Collect Vertices
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            var vertex = Points.transform.GetChild(i).transform.position;
            vertices.Add(vertex);
        }

        //Step 1. Make sure no duplicate vertices
        vertices = vertices.Distinct().ToList();

        //Step 2. Perform a triangulation algorithm
        vertices = vertices.OrderBy(v => v.x).ToList();
        var triangles = Triangulation.Incremental(vertices);

        //Step 3. Perform deluanay flipedge algorithm, to get better triangles
        if (flip)
        {
            triangles = Triangulation.Delaunay_FlipEdge(vertices, triangles).GetIndicesFromHalfEdges();
        }

        //Step 4. create the mesh
        Mesh mesh = new();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Delaunay FlipEdge Triangulation Triangle Count: " + triangles.Count / 3;
        Text.text += "\n";

        //Extra 1. Check Delaunay
        Text.text += "Is Delaunay? " + TriangulationUtils.VerifyDelaunay(vertices, triangles);
    }


    //Debug
    void OnDrawGizmos()
    {
        List<Vector3> vertices = new();

        //Step 0. Collect Vertices
        for (int i = 0; i < Points.transform.childCount; i++)
        {
            var vertex = Points.transform.GetChild(i).transform.position;
            vertices.Add(vertex);
        }

        //Step 1. Make sure no duplicate vertices
        vertices = vertices.Distinct().ToList();

        vertices = vertices.OrderBy(v => v.x).ToList(); // Sort based on x value as required by algorithm used
        var triangles = Triangulation.Incremental(vertices);

        var dcel = DoublyConnectedEdgeList.CreateFromTriangleMesh(vertices, triangles);

        if (flip)
        {
            triangles = Triangulation.Delaunay_FlipEdge(vertices, triangles).GetIndicesFromHalfEdges();
            dcel = DoublyConnectedEdgeList.CreateFromTriangleMesh(vertices, triangles);
        }

        GeometryHelpers.GizmoDrawHalfEdges(dcel.HalfEdges, Color.green, Color.red);

        var face = dcel.Faces[1];
        GeometryHelpers.GizmoDrawFaceHalfEdges(face, Color.blue, Color.red, 0.2f);
    }
}