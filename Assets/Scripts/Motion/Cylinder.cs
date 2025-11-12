using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cylinder : MonoBehaviour
{
    //currently handles dialogue, etc. with unityevents. if preferred, we could also have dialoguecanvasmanager continually check a public variable in this class
    public static event Action<Cylinder> onCylinderEntrance;
    private void OnTriggerEnter(Collider other)
    {
        if(onCylinderEntrance != null) onCylinderEntrance(this);
    }
}
