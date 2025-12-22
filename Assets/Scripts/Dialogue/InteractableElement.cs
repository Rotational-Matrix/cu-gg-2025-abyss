using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public abstract class InteractableElement : MonoBehaviour
{
    // All interactableElements need to have some kind of OnTriggerExit and OnTriggerEnter

    public abstract void ExecuteInteract(); //may include to remove itself or disable its own trigger
    public abstract void AlertToUpdateInteract(bool value); //should tell if to disable or enable trigger

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.AddInteraction(this); //makes prompt appear (if not already present)
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.RemoveInteraction(this);
    }
    

}
