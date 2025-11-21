using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackElement : SelectableElement, ISelectableElement
{
    /* MenuFlowElements
     *  - Just learned that c# has innate callbacks, which makes this sooo much easier
     *      - Func<TInput,TOutput> and Action<TInput> can be used for callback sake
     *  
     *  - These menuItems are for more volatile popups which probably change on each call
     *      - thus, each call needs to make sure it first sets behaviour (and input)
     */
    [SerializeField] private GameObject selectorImage;
    [SerializeField] private TMPro.TMP_Text itemTextObject;
    private Action<int> callback = null; //may very well ignore the int
    private int inputVal = 0;
    public override void SetSelected(bool setSelected)
    {
        selectorImage.SetActive(setSelected);
    }
    public override bool Choose()
    {
        //null callbacks mean no action on call
        //  -probably leads to quitting the current menu
        if(!object.Equals(callback,null))
            callback(inputVal);
        return true; //presumes always succeeds (or callback handles failure)
    }

    //since these callback options are basically for popups only,
    //SetVisible(false) basically wipes everything
    //
    public override void SetVisible(bool visible) 
    {
        if (visible)
        {
            //SetVisible(true) should *ONLY* be called after AssignBehaviour, SetInputInt
            this.gameObject.SetActive(visible); //which is necessarily true...
            this.selectorImage.SetActive(!visible); //still wipes selector image
        }
        else
            Clean();
    }

    //these two will prolly be called upon the popup call
    public void SetCallback(string displayText, Action<int> setCallback)
    {
        itemTextObject.text = displayText;
        callback = setCallback;
    }

    public void SetInput(int inputInt)
    {
        inputVal = inputInt;
    }

    private void OnDisable()
    {
        SetVisible(false);
    }

    //note that clean is way less severe than usual.
    //Since both stable and dynamic UI panels 'popups' will use this,
    //Clean() allows retention of all properties for static UI panel sake
    //Please use SetCallback and SetInput before first call and as needed.
    private void Clean()
    {
        //itemTextObject.text = "";
        selectorImage.SetActive(false);
        //inputForCallback = 0;
        //intCallback = null;
    }
}
