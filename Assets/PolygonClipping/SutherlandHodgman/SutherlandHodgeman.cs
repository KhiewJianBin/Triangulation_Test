using asim.unity.extensions;
using asim.unity.utils.geometry;
using System.Collections.Generic;
using UnityEngine;

public static partial class PolygonClipping
{
    /// <summary>
    /// Implementation of the Sutherland Hodgeman clipping algorithm.
    /// Based on Psudo code from : https://en.wikipedia.org/wiki/Sutherland%E2%80%93Hodgman_algorithm
    /// Works only with Convex Clipping polygon
    /// </summary>
    public static List<Vector2> SutherlandHodgeman(List<Vector2> subjectPolygon, List<Vector2> clipPolygon)
    {
        if (subjectPolygon.Count < 3 || clipPolygon.Count < 3) return default;

        //0.
        List<Vector2> outputList = subjectPolygon;
        int clipPolygonOrientation = GeometryUtils.Orientation(clipPolygon[0], clipPolygon[1], clipPolygon[2]);

        //1. Loop though each edge of clipPolygon
        for (int i = 0; i < clipPolygon.Count; i++)
        {
            var clipEdge = (clipPolygon[i], clipPolygon[(i + 1) % clipPolygon.Count]);

            var inputList = outputList;
            outputList = new();
            for (int j = 0; j < inputList.Count; j++)
            {
                var current_point = inputList[j];
                var prev_point = inputList[MathExtensions.IndexLoop(j - 1, inputList.Count)];

                //Calculate intersection with inifinite line (acutally just very large)
                var p1 = clipEdge.Item1 - (clipEdge.Item2 - clipEdge.Item1) * 100;
                var p2 = clipEdge.Item2 + (clipEdge.Item2 - clipEdge.Item1) * 100;
                var Intersecting_point = GeometryUtils.IsLinesIntercept(prev_point, current_point, p1, p2);

                if (GeometryUtils.Orientation(clipEdge.Item1, clipEdge.Item2, current_point) == clipPolygonOrientation)
                {
                    if (GeometryUtils.Orientation(clipEdge.Item1, clipEdge.Item2, prev_point) != clipPolygonOrientation)
                    {
                        //Only add if we have intercept
                        if (Intersecting_point.IsIntercept)
                            outputList.Add(Intersecting_point.Item3);
                    }
                    outputList.Add(current_point);
                }
                else if (GeometryUtils.Orientation(clipEdge.Item1, clipEdge.Item2, prev_point) == clipPolygonOrientation)
                {
                    //Only add if we have intercept
                    if (Intersecting_point.IsIntercept)
                        outputList.Add(Intersecting_point.Item3);
                }
            }
        }

        return outputList;
    }
}