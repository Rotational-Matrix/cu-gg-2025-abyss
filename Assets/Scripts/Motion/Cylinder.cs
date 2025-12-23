using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cylinder : MonoBehaviour
{
    [SerializeField] private bool teleportToCave;

    //currently handles dialogue, etc. with unityevents. if preferred, we could also have dialoguecanvasmanager continually check a public variable in this class
    //public static event Action<Cylinder> onCylinderEntrance;
    private void OnTriggerEnter(Collider other)
    {
        //if(onCylinderEntrance != null && object.Equals(other, StateManager.Eve.pCollider))
        //    onCylinderEntrance(this);
        StateManager.Eve.TryGetComponent<PlayerAnimationManager>(out PlayerAnimationManager outPAM);
        if(!object.Equals(null,outPAM))
        {
            outPAM.Teleport(this);
        }
    }
    public bool IsTeleportingToCave()
    {
        return teleportToCave;
    }
}
