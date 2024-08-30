using System.Collections.Generic;

using UnityEngine;

using asim.unity.extensions;

namespace asim.unity.geometry.triangulation
{
    public static partial class Triangulation
    {
        /// <summary>
        /// From a list of input points, perform the Triangulation : Incremental algorithm
        /// to obtain a List of triangles define as indices from the input point list.
        /// 
        /// <Incremental>
        /// Algo is based of another algo - Convex Hull : Incremental,
        /// Modify it to add list of triangles while running the algo
        /// 
        /// <Input>
        /// A Sorted List of points based on X Pos
        /// 
        /// </summary>
        public static List<int> Incremental(List<Vector3> points)
        {
            if (points.Count < 3) return new List<int>();

            Vector3 InitP1 = points[0];
            Vector3 InitP2 = points[1];
            Vector3 InitP3 = points[2];
            List<(Vector3 pos, int originalIndex)> convexHull = new();
            convexHull.Add((InitP1, 0));
            convexHull.Add((InitP2, 1));
            convexHull.Add((InitP3, 2));

            List<int> triangles;

            int ConvexDir = GeometryUtils.Orientation(InitP1, InitP2, InitP3);
            if (ConvexDir == 1)
            {
                triangles = new() { 2, 1, 0 };
            }
            else
            {
                triangles = new() { 0, 1, 2 };
            }

            for (int i = 3; i < points.Count; i++)
            {
                var newPoint = points[i];

                int? pli = null;
                int? pri = null;

                for (int j = 0; j < convexHull.Count; j++)
                {
                    int pcurri = j;
                    int pnext = (j + 1) % convexHull.Count;
                    var edge = (convexHull[pcurri], convexHull[pnext]);
                    Vector2 p1 = edge.Item1.pos;
                    Vector2 p2 = edge.Item2.pos;
                    Vector2 p3 = newPoint;
                    int orientation = GeometryUtils.Orientation2(edge.Item1.pos, edge.Item2.pos, newPoint);

                    if (orientation != ConvexDir)
                    {
                        pri = pnext;

                        if (pli.HasValue == false)
                        {
                            pli = pcurri;
                        }

                        if (orientation == 1)
                        {
                            triangles.Add(i);
                            triangles.Add(edge.Item2.originalIndex);
                            triangles.Add(edge.Item1.originalIndex);
                        }
                        else
                        {
                            triangles.Add(edge.Item1.originalIndex);
                            triangles.Add(edge.Item2.originalIndex);
                            triangles.Add(i);
                        }
                    }
                    else
                    {
                        if (pri.HasValue)
                        {
                            break;
                        }
                    }
                }

                if (pri.HasValue == false) continue;

                convexHull.AddReplaceBetween(pli.Value, pri.Value, (points[i], i));
            }

            return triangles;
        }
    }
}