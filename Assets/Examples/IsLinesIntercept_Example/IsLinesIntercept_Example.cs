using UnityEngine;
using UnityEngine.UI;
using asim.unity.helpers;

namespace asim.unity.utils.geometry
{
    public class IsLinesIntercept_Example : MonoBehaviour
    {
        [SerializeField] Text Text;

        [SerializeField] Camera cam;

        [SerializeField] GameObject L1P1;
        [SerializeField] GameObject L1P2;
        [SerializeField] GameObject L2P1;
        [SerializeField] GameObject L2P2;

        void OnGUI()
        {
            //Convert World Pos to GUI Pos
            Vector3 GUIL1P1pos = cam.WorldToScreenPoint(L1P1.transform.position);
            Vector3 GUIL1P2pos = cam.WorldToScreenPoint(L1P2.transform.position);
            Vector3 GUIL2P1pos = cam.WorldToScreenPoint(L2P1.transform.position);
            Vector3 GUIL2P2pos = cam.WorldToScreenPoint(L2P2.transform.position);
            GUIL1P1pos.y = UnityOnGUIHelper.Height - GUIL1P1pos.y;
            GUIL1P2pos.y = UnityOnGUIHelper.Height - GUIL1P2pos.y;
            GUIL2P1pos.y = UnityOnGUIHelper.Height - GUIL2P1pos.y;
            GUIL2P2pos.y = UnityOnGUIHelper.Height - GUIL2P2pos.y;

            UnityOnGUIHelper.DrawLine(GUIL1P1pos, GUIL1P2pos, Color.red, 2);
            UnityOnGUIHelper.DrawLine(GUIL2P1pos, GUIL2P2pos, Color.red, 2);

            (bool IsParallel,bool IsLinesIntercept, Vector2 interceptPoint,Vector2 p2) = GeometryUtils.IsLinesIntercept(
                L1P1.transform.position, L1P2.transform.position,
                L2P1.transform.position, L2P2.transform.position);

            if (!IsParallel)
            {
                if (IsLinesIntercept)
                {
                    Vector3 GUIinterceptpos = cam.WorldToScreenPoint(interceptPoint);
                    GUIinterceptpos.y = UnityOnGUIHelper.Height - GUIinterceptpos.y;
                    UnityOnGUIHelper.DrawDot(GUIinterceptpos, 5, 0, Color.green, Color.green);

                    Text.text = "Lines Do Intercept";
                }
                else Text.text = "Lines Do Not Intercept";
            }
            else
            {
                if (IsLinesIntercept)
                {
                    Vector3 GUIInterceptSegmentP1pos = cam.WorldToScreenPoint(interceptPoint);
                    Vector3 GUIInterceptSegmentP2pos = cam.WorldToScreenPoint(p2);
                    GUIInterceptSegmentP1pos.y = UnityOnGUIHelper.Height - GUIInterceptSegmentP1pos.y;
                    GUIInterceptSegmentP2pos.y = UnityOnGUIHelper.Height - GUIInterceptSegmentP2pos.y;

                    UnityOnGUIHelper.DrawLine(GUIInterceptSegmentP1pos, GUIInterceptSegmentP2pos, Color.green, 5);

                    Text.text = "Lines Segment Intercept";
                }
                else Text.text = "Lines Do Not Intercept";
            }
        }
    }
}