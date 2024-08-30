using System.Collections.Generic;

using UnityEngine;

namespace asim.unity.geometry.triangulation
{
    public static class TriangulationUtils
    {
        public static bool VerifyDelaunay(List<Vector3> Vertices, List<int> Triangles)
        {
            try
            {
                for (int i = 0; i < Triangles.Count; i += 3)
                {
                    var c0 = Vertices[Triangles[i]];
                    var c1 = Vertices[Triangles[i + 1]];
                    var c2 = Vertices[Triangles[i + 2]];

                    for (int j = 0; j < Vertices.Count; j++)
                    {
                        var p = Vertices[j];
                        if (GeometryUtils.InsideCircumcircle(p, c0, c1, c2))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}