using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using System.Collections.Generic;
using asim.unity.geometry.triangulation;
using asim.unity.utils.geometry;
using static asim.unity.utils.geometry.DoublyConnectedEdgeList;

[ExecuteInEditMode]
public class TriangulationDelaunay_FlipEdge_Example : MonoBehaviour
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

        //Step 2. Perform a triangulation algorithm
        vertices = vertices.OrderBy(v => v.x).ToList(); // Sort based on x value as required by algorithm used
        triangles = Triangulation.Incremental(vertices);

        //Step 3. Perform deluanay flipedge algorithm, to get better triangles
        //triangles = Triangulation.Delaunay_FlipEdge(vertices, triangles);

        //Step 4. create the mesh
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        Meshfilter.mesh = mesh;

        Text.text = "Delaunay FlipEdge Triangulation Triangle Count: " + triangles.Count / 3;

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

    public int next = 0;
    void OnDrawGizmos()
    {
        
    }
}