using System.Collections.Generic;
using UnityEngine;

namespace asim.unity.geometry.triangulation
{
    public static partial class Triangulation
    {
        /// <summary>
        /// Given an already triangulated mesh, with vertices and indices, Perform the Delaunay Flip Edge algorithm
        /// Uses the DoublyConnectedEdgeList / HalfEdges data structure
        /// </summary>
        public static DoublyConnectedEdgeList Delaunay_FlipEdge(List<Vector3> vertices, List<int> indices)
        {
            //1. Prep for Flipedge - Convert this to the HalfEdge DataStruct
            DoublyConnectedEdgeList dcel = DoublyConnectedEdgeList.CreateFromTriangleMesh(vertices, indices);

            //2. Loop though all edges
            var edgestoflip = dcel.HalfEdges;
            for (int i = 0; i < edgestoflip.Count; i++)
            {
                var edge = edgestoflip[i];
            
                //3. Check if it does not satisfy delaunay condition, sum of angle <= 180, flip the edge
                //4. Note, using the current foreach loop will also check the twin again, but its acutally not required
                if (edge.Twin.IncidentFace != null)
                {
                    var Tri1_P1 = edge.Next.Origin.Pos;
                    var Tri1_P2 = edge.Next.Next.Origin.Pos;
                    var Tri1_P3 = edge.Next.Next.Next.Origin.Pos;

                    var Tri2_P1 = edge.Twin.Next.Origin.Pos;
                    var Tri2_P2 = edge.Twin.Next.Next.Origin.Pos;
                    var Tri2_P3 = edge.Twin.Next.Next.Next.Origin.Pos;

                    var Tri1Angle = Vector3.Angle(Tri1_P1 - Tri1_P2, Tri1_P3 - Tri1_P2);
                    var Tri2Angle = Vector3.Angle(Tri2_P1 - Tri2_P2, Tri2_P3 - Tri2_P2);

                    bool isdelaunay = Tri1Angle + Tri2Angle <= 180;

                    if (!isdelaunay)
                    {
                        //5. Reset loop and do it again from the top (because state has changed due to flip
                        if (DoublyConnectedEdgeList.FlipEdge(edge))
                        {
                            i = 0;
                        }
                    }
                }
            }

            return dcel;
        }
    }
}
