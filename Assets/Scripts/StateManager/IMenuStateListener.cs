using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateManagerListener
{
    //currently, all listeners should expect to receive a state of the following:
    // bool MenuStateActive, bool Dialogue Active, bool external_override active.
    // honestly the external overide is supplied as something to allow for later changes

    public void OnStateChange(bool inMenu, bool inDialogue, bool inExtern);
}
