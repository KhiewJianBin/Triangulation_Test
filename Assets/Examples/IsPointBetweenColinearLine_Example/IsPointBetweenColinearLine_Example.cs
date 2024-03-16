using UnityEngine;
using UnityEngine.UI;
using asim.unity.helpers;

namespace asim.unity.utils.geometry
{
    public class IsPointBetweenColinearLine_Example : MonoBehaviour
    {
        [SerializeField] Text Text;

        [SerializeField] Camera cam;

        [SerializeField] GameObject P1;
        [SerializeField] GameObject P2;
        [SerializeField] GameObject Point;

        void OnGUI()
        {
            //Convert World Pos to GUI Pos
            Vector3 GUIP1pos = cam.WorldToScreenPoint(P1.transform.position);
            Vector3 GUIP2pos = cam.WorldToScreenPoint(P2.transform.position);
            GUIP1pos.y = UnityOnGUIHelper.Height - GUIP1pos.y;
            GUIP2pos.y = UnityOnGUIHelper.Height - GUIP2pos.y;

            UnityOnGUIHelper.DrawLine(GUIP1pos, GUIP2pos, Color.red, 2);

            bool IsPointBetweenLine = GeometryUtils.IsPointBetweenColinearLine(P1.transform.position, P2.transform.position, Point.transform.position);
            if (IsPointBetweenLine) Text.text = "Point Is Between Colinear Line";
            else Text.text = "Point Is Not Between Colinear Line";
        }
    }
}