using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections.Generic;
using asim.unity.geometry.triangulation;
using asim.unity.utils.geometry;
using static asim.unity.utils.geometry.DoublyConnectedEdgeList;

[ExecuteInEditMode]
public class TriangulationDelaunay_BowyerWatson_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] GameObject Points;
    [SerializeField] MeshFilter Meshfilter;

    [Header("Debug")]
    public bool flip = true;
    public int next = 0;


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

        //Step 2. Perform a triangulation algorithm
        vertices = vertices.OrderBy(v => v.x).ToList(); // Sort based on x value as required by algorithm used
        triangles = Triangulation.Incremental(vertices);

        //Step 3. Perform deluanay flipedge algorithm, to get better triangles
        if(flip)
        {
            triangles = Triangulation.Delaunay_FlipEdge(vertices, triangles).GetIndicesFromHalfEdges();
        }

        //Step 4. create the mesh
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Delaunay FlipEdge Triangulation Triangle Count: " + triangles.Count / 3;
        Text.text += "\n";

        //Extra 1. Check Delaunay
        Text.text += "Is Delaunay? " + Triangulation.VerifyDelaunay(vertices, triangles);

        //Debug
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            next = Mathf.Clamp(next - 1, 0, int.MaxValue);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            next = Mathf.Clamp(next + 1, 0, int.MaxValue);
        }
    }

    void OnDrawGizmos()
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

        vertices = vertices.OrderBy(v => v.x).ToList(); // Sort based on x value as required by algorithm used
        triangles = Triangulation.Incremental(vertices);

        var dcel = DoublyConnectedEdgeList.CreateFromTriangleMesh(vertices, triangles);

        if (flip)
        {
            triangles = Triangulation.Delaunay_FlipEdge(vertices, triangles).GetIndicesFromHalfEdges();
            dcel = DoublyConnectedEdgeList.CreateFromTriangleMesh(vertices, triangles);
        }

        for (int i = 0; i < dcel.HalfEdges.Count; i++)
        {
            var he = dcel.HalfEdges[i];

            if (he.IncidentFace != null)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            var from = he.Origin.Pos;
            var to = he.Next.Origin.Pos;
            var dir = (from - to).normalized;
            var offset = Vector3.Cross(dir, Vector3.forward) * 0.2f;
            Gizmos.DrawLine(from - offset - dir * 0.8f, to - offset + dir * 0.8f);
        }

        var face = dcel.Faces[1];
        var num = next;
        foreach (var he in WalkFace(face))
        {
            num--;
            if (num < 0) break;

            Gizmos.color = Color.blue;

            var from = he.Origin.Pos;
            var to = he.Next.Origin.Pos;
            var dir = (from - to).normalized;
            var offset = Vector3.Cross(dir, Vector3.forward) * 0.2f;
            Gizmos.DrawLine(from - offset - dir * 0.8f, to - offset + dir * 0.8f);

            Gizmos.color = Color.cyan;

            var from2 = he.Twin.Origin.Pos;
            var to2 = he.Twin.Next.Origin.Pos;
            var dir2 = (from2 - to2).normalized;
            var offset2 = Vector3.Cross(dir2, Vector3.forward) * 0.2f;
            Gizmos.DrawLine(from2 - offset2 - dir2 * 0.8f, to2 - offset2 + dir2 * 0.8f);
        }
    }
}