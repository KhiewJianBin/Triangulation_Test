using UnityEngine;
using UnityEngine.UI;

using System.Collections.Generic;
using asim.unity.utils.geometry;

[ExecuteInEditMode]
public class SutherlandHodgeman_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] GameObject Polygon;
    [SerializeField] GameObject ClipPolygon;

    List<Vector2> polygon = new();
    List<Vector2> clippolygon = new();
    List<Vector2> drawpoints = new();

    void Update()
    {
        polygon.Clear();
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            var point = Polygon.transform.GetChild(i).transform.position;
            polygon.Add(point);
        }

        clippolygon.Clear();
        for (int i = 0; i < ClipPolygon.transform.childCount; i++)
        {
            var point = ClipPolygon.transform.GetChild(i).transform.position;
            clippolygon.Add(point);
        }


        drawpoints.Clear();

        if (!GeometryUtils.IsPolygonConvex(clippolygon)) return;

        drawpoints = PolygonClipping.SutherlandHodgeman(polygon, clippolygon);
    }

    void OnDrawGizmos()
    {
        Vector2 start = default;
        Vector2 end = default;

        //Draw points looping to start
        Gizmos.color = Color.green;
        if (polygon != null && polygon.Count > 0)
        {
            for (int i = 0; i < polygon.Count - 1; i++)
            {
                start = polygon[i];
                end = polygon[(i + 1)];
                Gizmos.DrawLine(start, end);
            }
            start = polygon[0];
            Gizmos.DrawLine(start, end);
        }

        //Draw points looping to start
        Gizmos.color = Color.red;
        if (clippolygon != null && clippolygon.Count > 0)
        {
            for (int i = 0; i < clippolygon.Count - 1; i++)
            {
                start = clippolygon[i];
                end = clippolygon[(i + 1)];
                Gizmos.DrawLine(start, end);
            }
            start = clippolygon[0];
            Gizmos.DrawLine(start, end);
        }


        //Draw points looping to start
        Gizmos.color = Color.yellow;
        if (drawpoints != null && drawpoints.Count > 0)
        {
            for (int i = 0; i < drawpoints.Count - 1; i++)
            {
                start = drawpoints[i];
                end = drawpoints[(i + 1)];
                Gizmos.DrawSphere(end, 1);
                Gizmos.DrawLine(start, end);
            }
            start = drawpoints[0];
            Gizmos.DrawSphere(start, 1);
            Gizmos.DrawLine(start, end);
        }
    }
}
