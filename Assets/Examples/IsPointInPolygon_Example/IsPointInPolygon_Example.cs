using asim.unity.helpers;
using asim.unity.utils.geometry;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class IsPointInPolygon_Example : MonoBehaviour
{
    [SerializeField] Text Text;
    [SerializeField] Text Text2;

    [SerializeField] Camera cam;

    [SerializeField] GameObject Point;
    [SerializeField] GameObject Polygon;
    [SerializeField] GameObject PointOutside;

    void OnGUI()
    {
        Vector2 point = Point.transform.position;

        List<Vector2> polygon = new();
        for (int i = 0; i < Polygon.transform.childCount; i++)
        {
            var vertex = Polygon.transform.GetChild(i).transform.position;
            polygon.Add(vertex);
        }

        //Draw polygon
        for (int i = 0; i < polygon.Count; i++)
        {
            var p1 = polygon[i];
            var p2 = polygon[(i + 1) % polygon.Count];

            //Convert World Pos to GUI Pos
            Vector3 p1pos = cam.WorldToScreenPoint(p1);
            Vector3 p2pos = cam.WorldToScreenPoint(p2);
            p1pos.y = UnityOnGUIHelper.Height - p1pos.y;
            p2pos.y = UnityOnGUIHelper.Height - p2pos.y;

            UnityOnGUIHelper.DrawLine(p1pos, p2pos, Color.red, 2);
        }

        //Draw Intersecting Line
        Vector2 pointOutside = PointOutside.transform.position;
        Vector3 pointpos = cam.WorldToScreenPoint(point);
        Vector3 pointOutsidepos = cam.WorldToScreenPoint(pointOutside);
        pointpos.y = UnityOnGUIHelper.Height - pointpos.y;
        pointOutsidepos.y = UnityOnGUIHelper.Height - pointOutsidepos.y;
        UnityOnGUIHelper.DrawLine(pointpos, pointOutsidepos, Color.blue, 2);

        if (GeometryUtils.IsPointInPolygon(polygon, point, pointOutside))
        {
            Text.text = "(Odd-Even Rule) Point <Color=green>Is In</Color> Polygon";
        }
        else
        {
            Text.text = "(Odd-Even Rule) Point Is <Color=red>Not In</Color> Polygon";
        }

        if (GeometryUtils.IsPointInPolygon2(polygon, point, pointOutside))
        {
            Text2.text = "(Non-Zero Winding Rule) Point <Color=green>Is In</Color> Polygon";
        }
        else
        {
            Text2.text = "(Non-Zero WindingRule) Point Is <Color=red>Not In</Color> Polygon";
        }
    }
}
