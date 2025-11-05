using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PauseMenuManager : MonoBehaviour
{
    /* This will be the pause menu manager, similar to the dialogue canvas manager
     * The pause system chould operate as follows:
     *  - Pause menu will exist as a canvas structure
     *  - There may be multiple 'tabs' of the pause menu
     *      - in such case, there will be an indicator to which tab one is on
     *  - The configural tab (the most important) will be sturctured as follows:
     *      - grid layout of 'selectable features'
     *          - selectable features will be prefabs with the following child gameObjects:
     *              - a TMPro Textbox (or textmesh or whatever it is called)
     *              - a selector (that can be used similar to how choice boxes are.
     *              - a displayed value (if relevant)
     *              - reaction to being selected (like placing the user in a state to require input)
     *          - there will be a handler to control each of these as a collective (similar to choiceBoxHandler)
     *  - There should be commands to access the data this can alter, as well as calling it
     */

    public enum ConfigItem
    {
        Resume,
        Brightness,
        LeashLength,
        Controls,
        LoadSaveOption,
        ExitOption,
    }
    public static Dictionary<ConfigItem, int> ConfigValues { get; private set; }

    [SerializeField] private GameObject pauseMenuPanel; //presumes there will only be one
    [SerializeField] private TMPro.TMP_Text pauseMenuText; //prolly doesn't need to actually know about this

    // popup + menu collection (generally, dynamic menus are called popups here)
    [SerializeField] private CallbackGrid assignValuePopup; //for leash and for Brightness (dynamic!)
    [SerializeField] private CallbackGrid areYouSurePopup; // for exit-game and load-save esque calls (dynamic!)
    [SerializeField] private CallbackGrid loadSaveMenu;
    [SerializeField] private CallbackGrid controlsMenu;

    [SerializeField] private GameObject menuItemHandler; //presumed to be a collection of menu items w/proper handler

    /* Doesn't create the popup, just overwrites properties on the existing one
     */
    public void InitAVPopup(/*realistically should be passing args*/)
    {
        //assignValuePopup.NOTIMPLEMENTED();
    }
    public void InitAYSPopup(/*string headerText, */string actText, Action<int> call, int val, string cancelText)
    {
        //note e2Call and e2Val to be null and 0 respectively (hence why they are not needed
        //areYouSurePopup.NOTIMPLEMENTED(headerText, e1Text, e1Call, e1Val, e2Text);
        PrepareAYS(actText, call, val, cancelText);
        areYouSurePopup.InitiateGrid();
    }
    public void InitLSMenu()
    {
        //presume there are 3 load save areas (so four options to include quit)
        PrepareLS();
        loadSaveMenu.InitiateGrid();
    }
    public void InitCTRLMenu()
    {
        controlsMenu.InitiateGrid();
    }



    public void ToggleMenu() //only concerns the menu panel rn
    {
        SetPauseMenuState(!StateManager.GetPauseMenuStatus());
    }

    private void SetPauseMenuState(bool setActive)
    {
        pauseMenuPanel.SetActive(setActive); //currently only handles the panel
        StateManager.SetPauseMenuStatus(setActive);
    }

    private void Awake()
    {
        SetPauseMenuState(false);

        //feel like this should be stored in PauseMenuManager...
        ConfigValues = new Dictionary<ConfigItem, int>();
        ConfigValues.Add(ConfigItem.Brightness, -1);
        ConfigValues.Add(ConfigItem.Brightness, 50); //hardCoded initBrightness
        ConfigValues.Add(ConfigItem.LeashLength, -1); //hardCoded initLeashLength
        ConfigValues.Add(ConfigItem.Controls, -1);
        ConfigValues.Add(ConfigItem.LoadSaveOption, -1);
        ConfigValues.Add(ConfigItem.ExitOption, -1);

        CTRLAwake();
        LSAwake();
    }
    

    private void CTRLAwake()
    {
        //the lambda statement notably takes an 'int x' just to fit the cast.
        controlsMenu.SetCallbackAt(0, "ACCEPT AND EXIT", (x) => { StateManager.MenuStack.Peek().ExitMenu(); });
    }

    // EXPECTED 3 SAVESTATE SPOTS (and 1 quit)
    private void LSAwake()
    {
        //elements 0,1,2 are all dynamic for the most part
        loadSaveMenu.SetCallbackAt(3, "CANCEL", (x) => { StateManager.MenuStack.Peek().ExitMenu(); });
        for (int i = 0; i < 3; i++)
            loadSaveMenu.SetInputAt(i, i);
    }

    private void PrepareLS() //to be called on observation, not awake
    {
        //needs to look at list and observe all 3 save states to run
        for (int i = 0; i < 3; i++)
        {
            string bucketStr = "LOAD SAVE " + i;
            Action<int> bucketAct;
            if (StateManager.DCManager.IsInkSaveStateEmpty(i))
            {
                bucketStr += " [EMPTY]";
                bucketAct = null; //literally do nothing on being chosen
            }
            else
            {
                Action<int> nestedAct = (x) => { StateManager.DCManager.RunInkSaveState(x); };
                bucketAct = (x) => { InitAYSPopup(bucketStr + "?", nestedAct, x, "DO NOT " + bucketStr); };
            }
            loadSaveMenu.SetCallbackAt(i, bucketStr, bucketAct);
        }
    }

    private void PrepareAYS(string text, Action<int> call, int val, string cancelText)
    {
        //note that in all circumstances, there are 2 options
        areYouSurePopup.SetCallbackAt(0, cancelText, (x) => { StateManager.MenuStack.Peek().ExitMenu(); });
        areYouSurePopup.SetInputAt(1, val);
        Action<int> bucket_call = (x) => { StateManager.MenuStack.Peek().ExitMenu(); call(x); };
        areYouSurePopup.SetCallbackAt(1, text, bucket_call);
    }




}
