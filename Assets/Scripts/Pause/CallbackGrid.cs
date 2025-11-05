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
}
