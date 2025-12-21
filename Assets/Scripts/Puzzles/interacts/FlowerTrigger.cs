using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FlowerTrigger : InteractableElement
{
    // ne
    /*
    // All interactableElements need to have some kind of OnTriggerExit and OnTriggerEnter

    public abstract void ExecuteInteract(); //may include to remove itself or disable its own trigger
    public abstract void AlertToUpdateInteract(bool value); //should tell if to disable or enable trigger

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.AddInteraction(this);
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        if (object.Equals(StateManager.Eve.pCollider, other))
            StateManager.RemoveInteraction(this);
    }
    
     */
    public override void AlertToUpdateInteract(bool value)
    {
        gameObject.GetComponent<Collider>().enabled = value;
        //OnTriggerEnter will get triggered if trigger turns on && object in trigger
        //OnTriggerExit will *NOT* get triggered if trigger turns off && object was in trigger
        if (!value)
        {
            StateManager.RemoveInteraction(this);
        }
    }
    
    public override void ExecuteInteract()
    {
        StateManager.RCommander.IncremFlowerCount();
        SetFlowerActive(false);
    }

    public void SetFlowerActive(bool value)
    {
        // for laziness, presumes the flower object to be attached to itself
        AlertToUpdateInteract(value);
        gameObject.SetActive(value);
    }


}
