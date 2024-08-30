using System.Collections.Generic;

using UnityEngine;

namespace asim.unity.geometry.triangulation
{
    public static partial class Triangulation
    {
        /// <summary>
        /// BowyerWatson Delaunay triangulations
        /// Uses Welzl for Super Triangle
        /// Refrence code https://www.gorillasun.de/blog/bowyer-watson-algorithm-for-delaunay-triangulation/#the-super-triangle
        /// </summary>
        public static List<int> Delaunay_BowyerWatson(List<Vector2> points)
        {
            //1a. Use welzl algorithm to get minimum circle
            var minCircle = SmallestCircle.Welzl(new(points));
            float step = Mathf.PI * 2 / 3;

            //1b. Create Super Triangle from minimum circle
            Vector2 center = minCircle.Center;
            float radius = minCircle.Radius * 2f;
            Vector2 tp0 = center + new Vector2(Mathf.Cos(step * 0), Mathf.Sin(step * 0)) * radius;
            Vector2 tp1 = center + new Vector2(Mathf.Cos(step * 1), Mathf.Sin(step * 1)) * radius;
            Vector2 tp2 = center + new Vector2(Mathf.Cos(step * 2), Mathf.Sin(step * 2)) * radius;
            var superTriangle = new Triangle(tp0, tp1, tp2);

            //2. Start with super triangle
            List<Triangle> triangles = new() { superTriangle };

            //3. Add points ony by one
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                triangles = addVertex(point, triangles);
            }

            //4. Remove triangles that share edges with super triangle
            triangles = triangles.FindAll(triangle =>
                !(triangle.v0 == superTriangle.v0 || triangle.v0 == superTriangle.v1 || triangle.v0 == superTriangle.v2 ||
                triangle.v1 == superTriangle.v0 || triangle.v1 == superTriangle.v1 || triangle.v1 == superTriangle.v2 ||
                triangle.v2 == superTriangle.v0 || triangle.v2 == superTriangle.v1 || triangle.v2 == superTriangle.v2)
            );

            //5. Reorder each triangle vertices to match draw order, return as triangle indices
            List<int> trianglesReordered = new List<int>(triangles.Count * 3);
            for (int i = 0; i < triangles.Count; i++)
            {
                var windingOrder = GeometryUtils.Orientation(triangles[i].v0, triangles[i].v1, triangles[i].v2);
                if (windingOrder < 0)
                {
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v0));
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v1));
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v2));
                }
                else //if (windingOrder > 0)
                {
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v2));
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v1));
                    trianglesReordered.Add(points.FindIndex(v => v == triangles[i].v0));
                }
            }

            return trianglesReordered;

            List<Triangle> addVertex(Vector2 vertex, List<Triangle> triangles)
            {
                var edges = new List<Edge>();

                var newTriangles = new List<Triangle>();
                for (int i = 0; i < triangles.Count; i++)
                {
                    var triangle = triangles[i];

                    if (GeometryUtils.InsideCircumcircle(vertex, triangle.v0, triangle.v1, triangle.v2))
                    {
                        edges.Add(new Edge(triangle.v0, triangle.v1));
                        edges.Add(new Edge(triangle.v1, triangle.v2));
                        edges.Add(new Edge(triangle.v2, triangle.v0));
                    }
                    else
                    {
                        newTriangles.Add(triangle);
                    }
                }
                triangles = newTriangles;

                // Get unique edges
                edges = uniqueEdges(edges);
                var e2 = new List<Edge>(edges);

                // Create new triangles from the unique edges and new vertex
                foreach (var edge in e2)
                {
                    triangles.Add(new Triangle(edge.v0, edge.v1, vertex));
                    edges.Remove(edge);
                }

                return triangles;
            }

            List<Edge> uniqueEdges(List<Edge> edges)
            {
                var uniqueEdges = new List<Edge>();

                for (var i = 0; i < edges.Count; ++i)
                {
                    var isUnique = true;

                    // See if edge is unique
                    for (var j = 0; j < edges.Count; ++j)
                    {
                        if (i != j && edges[i].Equals(edges[j]))
                        {
                            isUnique = false;
                            break;
                        }
                    }

                    if (isUnique) uniqueEdges.Add(edges[i]);
                }

                return uniqueEdges;
            }
        }
    }
}
