using JetBrains.Annotations; //is this something that swapping between versions added?
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

    [SerializeField] private DefaultGrid configMenu; //presumes there will only be one
    //[SerializeField] private TMPro.TMP_Text pauseMenuText; //prolly doesn't need to actually know about this

    // popup + menu collection (generally, dynamic menus are called popups here)
    [SerializeField] private CallbackGrid assignValuePopup; //for leash and for Brightness (dynamic!)
    [SerializeField] private CustomNumInput avInputHandler;

    [SerializeField] private CallbackGrid areYouSurePopup; // for exit-game and load-save esque calls (dynamic!)
    [SerializeField] private CallbackGrid loadSaveMenu;
    [SerializeField] private CallbackGrid controlsMenu;


    /* Doesn't create the popup, just overwrites properties on the existing one
     */
    
    public void InitConfigMenu()
    {
        configMenu.InitiateGrid();
    }
    public void InitAVPopup(Action<int> call, string valueName, string prevValue, int max, string maxText)
    {
        PrepareAV(call, valueName, prevValue, max, maxText);
        assignValuePopup.InitiateGrid();
    }
    public void InitAYSPopup(/*string headerText, */string actText, Action<int> call, int val, string cancelText)
    {
        //note cancelCall and cancelVal are not dynamic and hence they are not needed as params
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


    /*
    public void ToggleMenu() //only concerns the menu panel rn
    {
        SetPauseMenuState(!StateManager.GetPauseMenuStatus());
    }

    private void SetPauseMenuState(bool setActive)
    {
        pauseMenuPanel.SetActive(setActive); //currently only handles the panel
        StateManager.SetPauseMenuStatus(setActive);
    }*/

    private void Awake()
    {
        //feel like this should be stored in PauseMenuManager...
        ConfigValues = new Dictionary<ConfigItem, int>();
        ConfigValues.Add(ConfigItem.Resume, -1);
        ConfigValues.Add(ConfigItem.Brightness, 35); //hardCoded initBrightness
        ConfigValues.Add(ConfigItem.LeashLength, -1); //hardCoded initLeashLength
        ConfigValues.Add(ConfigItem.Controls, -1);
        ConfigValues.Add(ConfigItem.LoadSaveOption, -1);
        ConfigValues.Add(ConfigItem.ExitOption, -1);

        CTRLAwake();
        LSAwake();
        AVAwake();
        ConfigAwake();
    }
    

    private void CTRLAwake()
    {
        //the lambda statement notably takes an 'int x' just to fit the cast.
        controlsMenu.SetCallbackAt(0, "ACCEPT AND EXIT", (x) => { StateManager.ExitTopMenu(); });
    }

    // EXPECTED 3 SAVESTATE SPOTS (and 1 quit)
    private void LSAwake()
    {
        //elements 0,1,2 are all dynamic for the most part
        loadSaveMenu.SetCallbackAt(3, "CANCEL", (x) => { StateManager.ExitTopMenu(); });
        for (int i = 0; i < 3; i++)
            loadSaveMenu.SetInputAt(i, i);
    }

    private void AVAwake()
    {
        assignValuePopup.SetInputType(StateManager.MenuInputType.DirectKey);
    }

    private void ConfigAwake()
    {
        int i = 0;
        foreach(ConfigItem cItem in Enum.GetValues(typeof(ConfigItem)))
        {
            (configMenu[i++] as MenuItem).SetConfigID(cItem);
        }
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
        areYouSurePopup.SetCallbackAt(0, cancelText, (x) => { StateManager.ExitTopMenu(); });
        areYouSurePopup.SetInputAt(1, val);
        Action<int> bucket_call = (x) => { StateManager.ExitTopMenu(); call(x); };
        areYouSurePopup.SetCallbackAt(1, text, bucket_call);
    }

    private void PrepareAV(Action<int> assignValCall, string valueName, string prevValue, int max, string maxText)
    {
        avInputHandler.InitNumInput(assignValCall, valueName, prevValue, max, maxText);
        StateManager.SetDirectAction(avInputHandler.HandleKey);
    }



}
