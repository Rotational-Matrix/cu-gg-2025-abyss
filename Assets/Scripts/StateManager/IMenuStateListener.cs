using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateManagerListener
{
    //currently, all listeners should expect to receive a state of the following:
    // bool MenuStateActive, bool Dialogue Active, bool external_override active.
    // honestly the external overide is supplied as something to allow for later changes

    /* IStateManagerListener is an interface to implement in order to listen for state changes
     * State changes occur when any of the following change:
     *  - whether the game is in any type of 'menu'
     *  - whether dialogue is occurring
     *  - special states are entered or exited
     * 
     * On State Change, the titular method is broadcast with the new state.
     * inMenu and inDialogue are clear. 
     * stateFlag is an indicator for less commonly used information.
     * 
     */

    public void OnStateChange(bool inMenu, bool inDialogue, int stateFlag);

    // using stateFlag is unfortunately a slapdash maneouvre to be able to account for specific situations
    // stateFlag = 0; entails usual operation
    // stateFlag can be used to distinguish inDialogue types
    // Currently, stateFlag > 0 means modified conditions (like dialogue w/ player movement)
    //            stateFlag < 0 means disruptions (like nothing allowed to be on)
    /* Let the following be stateFlag's defined meaning
     *  - stateFlag = 0  --->  no special case
     *  - stateFlag = 1  --->  dialogue permits player movement
     */
}
