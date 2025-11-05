using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour, ISelectableElement
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

    [SerializeField] private PauseMenuManager.ConfigItem configItem; //This serves as the 'type of configmenu item
    
    public void SetSelected(bool setSelected)
    {
        selectorImage.SetActive(setSelected);
    }
    public bool Choose()
    {
        return React();
    }
    public void SetVisible(bool visible)
    {
        if(visible)
        {
            itemValueTextObject.text = PrepareItemValue(); //sets the item value text
            itemTextObject.text = PrepareItemName(); //sets the item name text
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
    /*private void InitMenuItem(PauseMenuManager.ConfigItem configItem)
    {
        this.configItem = configItem;
    }*/

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
        return itemText;
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
                // (enter number popup) FIXXXXXXXX
                break;
            case PauseMenuManager.ConfigItem.LeashLength:
                if (LeashConfigKnown())
                {
                    // (enter number popup) FIXXXXXXXX
                }
                else
                    success = false;
                break;
            case PauseMenuManager.ConfigItem.Controls:
                // (exit only popup) FIXXXXXXXX
                break;
            case PauseMenuManager.ConfigItem.LoadSaveOption:
                // (save state menu popup) FIXXXXXXXX
                break;
            case PauseMenuManager.ConfigItem.ExitOption:
                // should actually lead to an "are you sure you want to exit?" popup
                break;
            default:
                throw new System.ArgumentException("how...?");
                //break;
        }
        return success;
    }

    //note that most of these individual reactions will require accessing StateManager

    //ExitCall() is a little dangerous. MenuStack must ensure that only the top-most menu can be called
    //should prolly be altered to an "are you sure?" popup
    private void ExitCurrentMenu()
    {
        StateManager.MenuStack.Peek().ExitMenu();
    }
    
    // as stupid as it is, I can actually make all the methods in this class,
    // and then call them by switchcase based on StateManager.ConfigItem value

    

}
