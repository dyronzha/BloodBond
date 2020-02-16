using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck
{
    public static bool DetectGround(Vector3 goalPoint)
    {
        Vector3 fromPoint = goalPoint + new Vector3(0, 5.0f, 0);
        RaycastHit hit;
        if (Physics.Raycast(fromPoint, new Vector3(0, -1.0f, 0), out hit, 10.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public static bool DetectGround(Vector3 goalPoint, ref Vector3 groundPos) {
        Vector3 fromPoint = goalPoint + new Vector3(0, 5.0f, 0);
        RaycastHit hit;
        if (Physics.Raycast(fromPoint, new Vector3(0, -1.0f, 0), out hit, 10.0f, 1 << LayerMask.NameToLayer("Ground")))
        {
            groundPos = hit.point;
            return true;
        }
        else {
            return false;
        }
    }

    //public bool TeleportDetectGround(Vector3 pos, ref Vector3 goalPoint)
    //{
    //    Vector3 fromPoint = goalPoint + new Vector3(0, 5.0f, 0);
    //    for (int i = 0; i < 5; i++)
    //    {

    //    }
    //}

    public static bool TeleportDetectGround(Vector3 pos, Vector3 goalPoint, ref Vector3 groundPos)
    {
        Vector3 fromPoint = goalPoint + new Vector3(0, 5.0f, 0);
        RaycastHit hit;
        if (Physics.Raycast(fromPoint, new Vector3(0, -1.0f, 0), out hit, 10.0f, 1 << LayerMask.NameToLayer("Geound")))
        {
            groundPos = hit.point;
            return true;
        }
        else
        {
            Vector3 endPoint = fromPoint + new Vector3(0, -10.0f, 0);
            RaycastHit hit2;
            if (Physics.Linecast(endPoint, pos, out hit2, 1 << LayerMask.NameToLayer("Ground"))) {
                Vector3 dir = new Vector3(hit.point.x - pos.x, 0, hit.point.z - pos.z).normalized;
                goalPoint = new Vector3(hit.point.x, pos.y ,hit.point.z);
                return true;
            }
            return false;
        }
    }
}
