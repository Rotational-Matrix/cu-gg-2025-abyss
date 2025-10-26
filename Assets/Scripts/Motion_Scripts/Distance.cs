using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance
{
    public static Vector3 XToY(GameObject x, GameObject y)
    {
        return y.transform.position - x.transform.position;
    }
    public static float Dist(GameObject x, GameObject y)
    {
        Vector3 d = XToY(x, y);
        d.y = 0f;
        return d.magnitude;
    }
}
