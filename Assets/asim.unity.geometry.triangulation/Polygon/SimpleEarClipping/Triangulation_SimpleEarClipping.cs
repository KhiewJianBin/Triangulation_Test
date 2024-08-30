using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using asim.unity.extensions;

namespace asim.unity.geometry.triangulation
{
    public static partial class Triangulation
    {
        /// <summary>
        /// From a "SimplePolygon", perform Triangulation using the EarClipping algorithm,
        /// to obtain a List of triangles define as indices from the input point list.
        /// 
        /// Based on Gary Meisters's the Two-Ears Theorem - every simple polygon has at least two non-over lapping ears
        /// From a SimplyPolygon, find a ear vertex, remove it and repeat
        /// Summerized by :https://www.geometrictools.com/Documentation/TriangulationByEarClipping.pdf
        /// Support Holes, but not holes in holes
        /// Time complexity : O(n*n)
        public static List<Vector2> SimpleEarClipping(SimplePolygon simpleP)
        {
            var OuterVerts = simpleP.OuterVertices;
            var HoleVerts = simpleP.HolesVertices;

            //Step 0. Check if got holes and do preprocessing
            if(HoleVerts != null & HoleVerts.Count > 0)
            {
                OuterVerts = CutAndJoin(OuterVerts, HoleVerts);
            }

            List<Vector2> triangles = new List<Vector2>();
            while (OuterVerts.Count >= 3)
            {
                //Step 2. Find a ear. for each ear found,
                (Vector2 p1, Vector2 p2, Vector2 p3) = FindEar(OuterVerts);

                //Step 2b. add to the list of triangles in order
                triangles.Add(p1);
                triangles.Add(p2);
                triangles.Add(p3);

                //Step 2c. remove the vertices and 
                OuterVerts.Remove(p2);

                //Step 2d. repeat until no more ear could be found (vertex count < 3)
            }

            return triangles;

            //Finding Mutually Visible Vertices and creating a new polygon index chain from it
            List<Vector2> CutAndJoin(List<Vector2> OuterVerts, List<List<Vector2>> HoleVerts)
            {
                while (HoleVerts.Count != 0)
                {
                    //Search for the hole set of vertices that has the verticies of max X value
                    var maxofmax = float.NegativeInfinity;
                    var maxofmaxindex = -1;
                    for (int i = 0; i < HoleVerts.Count; i++)
                    {
                        var max = HoleVerts[i].Max(v => v.x);
                        if (max > maxofmax)
                        {
                            maxofmax = max;
                            maxofmaxindex = i;
                        }
                    }

                    List<Vector2> Holes = HoleVerts[maxofmaxindex];
                   
                    if (Holes.Count < 3) throw new Exception("Polygon holes is not a hole (Vertex count < 3)");

                    //Step 1. Find the index that has the Max X pos
                    int InnerIndexMaxX_M = 0;
                    for (int i = 1; i < Holes.Count; i++)
                    {
                        if (Holes[i].x > Holes[InnerIndexMaxX_M].x)
                        {
                            InnerIndexMaxX_M = i;
                        }
                    }
                    //Step 2. Cast a ray to the right and find interception edge, get vertex index that has the max X
                    int OuterIndexMaxX_P = 0;
                    Vector2 rayEnd = Holes[InnerIndexMaxX_M] + new Vector2(100, 0);
                    Vector2 intercept_I = Vector2.zero;
                    float mindist = float.PositiveInfinity;
                    for (int i = 0; i < OuterVerts.Count; i++)
                    {
                        
                        Vector2 edgep1 = OuterVerts[i % OuterVerts.Count];
                        Vector2 edgep2 = OuterVerts[MathExtensions.IndexLoop(i + 1, Holes.Count)];

                        (bool IsParallel, bool IsLinesIntercept, Vector2 interceptPoint, _) =
                            GeometryUtils.IsLinesIntercept(Holes[InnerIndexMaxX_M], rayEnd, edgep1, edgep2);

                        if (!IsParallel && IsLinesIntercept)
                        {
                            float dist = Vector2.Distance(Holes[InnerIndexMaxX_M], interceptPoint);
                            if (dist < mindist)
                            {
                                intercept_I = interceptPoint;
                                OuterIndexMaxX_P = edgep1.x > edgep2.x ? i : i + 1;
                            }
                        }
                    }
                    //Step 3. Check if any points are inside the triangle, if any, find R that is closest to M
                    int R = -1;
                    float cloesestToMDist = float.PositiveInfinity;
                    for (int i = 0; i < OuterVerts.Count; i++)
                    {
                        if (i == OuterIndexMaxX_P) continue;

                        if (GeometryUtils.IsPointInTriangle(Holes[InnerIndexMaxX_M], intercept_I, OuterVerts[OuterIndexMaxX_P], OuterVerts[i]) == 1)
                        {
                            var dist = Vector2.Distance(Holes[InnerIndexMaxX_M], OuterVerts[i]);
                            if (dist < cloesestToMDist)
                            {
                                cloesestToMDist = dist;
                                R = i;
                            }
                        }
                    }

                    //Step 4a. if there are no points inside, then M & P are mutally visible
                    //Step 4b. otherwise M & R and mutally visible
                    if (R != -1)
                    {
                        OuterIndexMaxX_P = R;
                    }

                    var PValue = OuterVerts[OuterIndexMaxX_P];
                    for (int i = 0; i < Holes.Count + 1; i++)
                    {
                        OuterVerts.Insert(OuterIndexMaxX_P, Holes[MathExtensions.IndexLoop(InnerIndexMaxX_M - i, Holes.Count)]);
                    }
                    OuterVerts.Insert(OuterIndexMaxX_P, PValue);

                    HoleVerts.RemoveAt(maxofmaxindex);
                }

                return OuterVerts;
            }

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
                    int prevIndex = MathExtensions.IndexLoop(i - 1, SPolygon.Count);
                    int currentIndex = i;
                    int nextIndex = MathExtensions.IndexLoop(i + 1, SPolygon.Count);
                    
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
                            throw new Exception("Input is not a Simple Polygon");
                        }
                    }
                }
            }
        }
    }
}