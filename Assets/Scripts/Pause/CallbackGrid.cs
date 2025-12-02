using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class CallbackGrid : DefaultGrid, IGridSelectable //is this redundant?
{
    /* inherits everything from default grid operation
     *  - has [SerializableField] expected for CallbackElements
     *  - requires treating the 
     *  
     *  Something like this will be used for the following:
     *      - Static popups:
     *          - LoadSave
     *          - Controls
     *      - Dynamic popups:
     *          - 'Are_you_sure'
     *          - 'inputValue'
     */
    private StateManager.MenuInputType menuInputType = StateManager.MenuInputType.SelectableGrid;
    private bool canBeSoftExited = true;

    public void SetCallbackAt(int index, string displayText, Action<int> setCallback)
    {
        //as stated earlier, it should be obvious that selectableList NEEDS to be CallbackElements
        if(index >= 0 && index < selectableList.Count)
        {
            (selectableList[index] as CallbackElement).SetCallback(displayText, setCallback);
        }
    }

    public void SetInputAt(int index, int inputInt)
    {
        if (index >= 0 && index < selectableList.Count)
        {
            (selectableList[index] as CallbackElement).SetInput(inputInt);
        }
    }

    public void SetInputType(StateManager.MenuInputType menuInputType)
    {
        this.menuInputType = menuInputType;
    }

    public void SetSoftExitable(bool value)
    {
        canBeSoftExited = value;
    }

    public override StateManager.MenuInputType InputType()
    {
        return this.menuInputType;
    }

    public override bool SoftExitMenu()
    {
        if (canBeSoftExited)
            ExitMenu();
        return canBeSoftExited;
    }
}
