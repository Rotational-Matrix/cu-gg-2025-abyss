using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : SelectableElement, ISelectableElement
{
    /* should be composed of 4 parts:
     *  - selector image
     *  - displayed text
     *  - (if applicable) displayed value
     *  - reaction
     */

    //presumably all children of the current GameObject this component is attached to.
    [SerializeField] private GameObject selectorImage;
    [SerializeField] private TMPro.TMP_Text itemTextObject;
    [SerializeField] private TMPro.TMP_Text itemValueTextObject;
    //reaction will very much need to be implemented

    private PauseMenuManager.ConfigItem configItem; //This serves as the 'type of configmenu item
    
    public override void SetSelected(bool setSelected)
    {
        selectorImage.SetActive(setSelected);
    }
    public override bool Choose()
    {
        return React();
    }
    public override void SetVisible(bool visible)
    {
        if(visible)
        {
            itemValueTextObject.text = PrepareItemValue(); //sets the item value text
            itemTextObject.text = PrepareItemName(); //sets the item name text
            this.selectorImage.SetActive(false); //wipes selector
            this.gameObject.SetActive(visible); //which is necessarily true...
            
        }
        else
            Clean();
    }

    /* InitMenuItem() is really where the laziness comes in.
     *  - Each menuItem is to be given its own functions to call and react with.
     *  - I don't want to create a unique class for every menu item, so
     *      each menuItems give their own ID to determine their functions
     */
    public void SetConfigID(PauseMenuManager.ConfigItem configItem)
    {
        this.configItem = configItem;
    }

    private string PrepareItemName()
    {
        string itemText;
        switch (configItem)
        {
            case PauseMenuManager.ConfigItem.Resume:
                itemText = "Resume";
                break;
            case PauseMenuManager.ConfigItem.Brightness:
                itemText = "Brightness";
                break;
            case PauseMenuManager.ConfigItem.LeashLength:
                itemText = ConfigLeashName();
                break;
            case PauseMenuManager.ConfigItem.Controls:
                itemText = "Controls";
                break;
            case PauseMenuManager.ConfigItem.LoadSaveOption:
                itemText = "Load Save File";
                break;
            case PauseMenuManager.ConfigItem.ExitOption:
                itemText = "Quit Game";
                break;
            default:
                itemText = "ERROR";
                break;
        }
        return itemText.ToUpper(); //I touppered this...
    }
    private string PrepareItemValue()
    {
        int itemVal = PauseMenuManager.ConfigValues[configItem];
        if (itemVal == -1)
            return "";
        else
            return itemVal.ToString();
    }
    private void Clean()
    {
        selectorImage.SetActive(false);
        this.gameObject.SetActive(false);
    }

    private string ConfigLeashName()
    {
        if (LeashConfigKnown()) 
            return "String Length";
        else
            return "???";
    }

    private bool LeashConfigKnown()
    {
        return StateManager.DCManager.GetInkVar<bool>("strlenConfigKnown"); //CALLS AN INKFILE VAR
    }

    private void OnDisable()
    {
        SetVisible(false); //normally I'd call 'Clean()' here, but this calls Clean anyway
    }


    /* note that this method isn't creating a component because, if we don't explicitly need to,
     * then we prolly shouldn't, as creating components at runtime is a little laborius
     */

    private bool React()
    {
        /* need to make a writable textbox
         * need to be able to make popups
         */
        //note to self to make the controls tab be empty and say something like:
        //  - "Sorry! You are not in control."
        bool success = true;
        switch (configItem)
        {
            case PauseMenuManager.ConfigItem.Resume:
                ExitCurrentMenu();
                break;
            case PauseMenuManager.ConfigItem.Brightness:
                AVPopupCall();
                break;
            case PauseMenuManager.ConfigItem.LeashLength:
                if (LeashConfigKnown())
                    AVPopupCall();
                else
                    success = false;
                break;
            case PauseMenuManager.ConfigItem.Controls:
                StateManager.PMManager.InitCTRLMenu();
                break;
            case PauseMenuManager.ConfigItem.LoadSaveOption:
                StateManager.PMManager.InitLSMenu();
                break;
            case PauseMenuManager.ConfigItem.ExitOption:
                // NOTE: SHOULD MAKE YOU EXIT THE GAME...ONLY USES AYS TO EXIT (FIXXXXX)
                StateManager.PMManager.InitAYSPopup("QUIT THE GAME (just leaves the pause rn)",
                    (x) => { ExitCurrentMenu(); }, 0, "DO NOT QUIT THE GAME");
                break;
            default:
                throw new System.ArgumentException("how...?");
                //break;
        }
        return success;
    }

    private void AVPopupCall()
    {
        int max = 99;
        string maxText = max.ToString();
        Action<int> bucketCall = (x) => { PauseMenuManager.ConfigValues[configItem] = x; this.RefreshItemVal(); };
        string prevValStr = PauseMenuManager.ConfigValues[configItem].ToString(); //danger
        if (configItem == PauseMenuManager.ConfigItem.Brightness)
        {
            max = 36; //HARDCODED (but lie to user) (put starting value + 1 maybe??)
            if (StateManager.DCManager.GetInkVar<bool>("triedToIncreaseBrightness")) //Inkfile VAR name
            {
                maxText = "<color=\"red\">" + max.ToString() + "</color>";
            }
            else
            {
                bucketCall = (x) =>
                {
                    PauseMenuManager.ConfigValues[configItem] = x;
                    this.RefreshItemVal();
                    if (x >= max) //it will get set to max anyway if it is higher...
                    {
                        StateManager.PMManager.InitAYSPopup("BRIGHTNESS CANNOT BE INCREASED",
                            StateManager.PhonyAction, 0, "ERROR: INVALID ACCESS");
                        StateManager.DCManager.SetInkVar<bool>("triedToIncreaseBrightness", true);
                    }
                };
            }
        }
        // current mechanism dangerously allows out of range str indicators FIXXXXXXXX
        StateManager.PMManager.InitAVPopup(bucketCall, PrepareItemName(), prevValStr, max, maxText);
    }

    private void RefreshItemVal()
    {
        itemValueTextObject.text = PrepareItemValue();
    }




    private void ExitCurrentMenu()
    {
        StateManager.ExitTopMenu();
    }

    // as stupid as it is, I can actually make all the methods in this class,
    // and then call them by switchcase based on StateManager.ConfigItem value


}
