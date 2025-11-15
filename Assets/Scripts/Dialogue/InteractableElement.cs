using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableElement : MonoBehaviour
{
    // All interactableElements need to have some kind of OnTriggerExit and OnTriggerEnter
    

    public abstract void ExecuteInteract(); //may include to remove itself

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.AddInteraction(this); //maybe make prompt appear
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.RemoveInteraction(this);
    }

}
