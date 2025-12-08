using JetBrains.Annotations; //is this something that swapping between versions added?
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using static UnityEngine.Rendering.DebugUI;

public class PauseMenuManager : MonoBehaviour, IStateManagerListener
{
    /// <summary>
    /// Part of [Cu]'s documentation for external usage of PauseMenuManager (don't worry this one isn't bad)
    /// 
    /// note that you can always ctrl-f these to find them in the code.
    /// 
    /// PauseMenuManager may be accessed via StateManager.PMManager as if it is basically static
    /// However, note that for Fields, PauseMenuManager actually is static. 
    /// 
    /// All things listed here are public, as per usual, no mention of private fns will be made
    /// 
    /// Fields:
    /// enum ConfigValues
    ///     - This was made before I knew how to use 'Delegate' funcitons
    ///     - These are the associated IDs of each of the options on the config menu
    /// 
    /// static Dictionary<ConfigItem, int> ConfigValues { get; private set; }
    ///     - This is where Config data is stored! (like user settings info)
    ///         - right now, nothing responds to it, but Brightness (as a stat) is here, and so is leach length!
    ///         - All items without (displayable) values have '-1' here.
    /// 
    /// Methods:
    /// void InitConfigMenu()
    ///     - activates and loads up the configMenu
    ///     
    /// void InitAVPopup(Action<int'> call, string valueName, string prevValue, int max, string maxText)
    ///     - activates and loads up 'Assign Value popup' (this one is very dynamic)
    ///     - enters a 'type number' environment
    ///     - parameters
    ///         - Action<int'> call: what will be done (should the player choose to assign a value)
    ///         - string valueName: e.g. "Brightness", the name the value is referred to by.
    ///         - string prevValue: the previous value in string form.
    ///         - int max: the max enterable value
    ///         - string maxText: the corresponding string of the maximum enterable value
    ///             - these are only different so as to explicitly deceive the player
    /// 
    /// void InitAYSPopup(string actText, Action<int'> call, int val, string cancelText)
    ///     - As per usual, activates and loads up an AYSPopup
    ///     - AYS is AreYouSure and is always in Y/N? form
    ///     - parameters
    ///         - string actText: the text on the propmt that does the action
    ///         - Action <int'> call: the action that will occur upon choosing to act
    ///         - int val: only useful if the 'call' requires an int (otherwise set to 0)
    ///         - string cancelText: the text on the propmt that cancels the action
    /// 
    /// void InitLSMenu()
    ///     - Activates and loads up the LoadSaveMenu
    ///         - Unlike the popups, this is a static menu which does the same thing when called
    ///         - The loading-from-save-state is implemented, but there is no reason to 'save' yet
    ///             - save slots that have no saves cannot be saved from
    /// 
    /// void InitCTRLMenu()
    ///     - activates and loads the controls menu
    ///         - currently, means a snarky page that says 'you are not in control' that can only be exited
    ///         - Note that ProtoInputHandler is abstracting key presses, so this might become genuinely functional
    ///         
    /// void SetActionPromptActive(bool value)
    ///     - Sets the action prompt on or off and also sets the glyph
    ///     - currently only does interact but could be easily changed to be more general
    ///     - the prompt is anticipated to only be on when neither in dialogue or menus
    ///         - this expectation is mostly due to interact only being available at these times
    /// 
    /// </summary>


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

    // interact prompt
    [SerializeField] private GameObject actPromptPanel;
    [SerializeField] private TMPro.TMP_Text actPromptText;

    // start menu (mostly the grid portion)
    [SerializeField] private CallbackGrid startMenu;

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
    public void InitLSMenu(bool isLoading)
    {
        //presume there are 3 load save areas (so four options to include quit)
        PrepareLS(isLoading);
        loadSaveMenu.InitiateGrid();
    }
    private void InitLSMenu(bool isLoading, Action<int> saveCall)
    {
        PrepareLS(isLoading, saveCall);
        loadSaveMenu.InitiateGrid();
    }
    public void InitCTRLMenu()
    {
        controlsMenu.InitiateGrid();
    }

    // start menu will be treated as regular menu
    public void InitStartMenu()
    {
        StateManager.ClearMenuStack(); //because the start menu will always be on bottom...
        PrepareStartMenu();
        StateManager.RCommander.SetBackdropActive(true); // this gets turned off only in the load save command
        startMenu.InitiateGrid();
    }


    public void SetActionPromptActive(bool value)
    {
        if (value)
        {
            string keyName = ProtoInputHandler.CurrentKeyboard[ProtoInputHandler.interactKey].displayName;
            actPromptText.text = "Press " + keyName.ToUpper() + " to interact";
            if (!(StateManager.IsInMenu() || StateManager.GetDialogueStatus()))
                actPromptPanel.SetActive(value);
            StateManager.AddStateChangeResponse(this); //gives the entire PMManager
        }
        else
        {
            actPromptPanel.SetActive(value);
            StateManager.RemoveStateChangeResponse(this);
        }

        
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
        StartMenuAwake();
        SetActionPromptActive(false);
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

    private void StartMenuAwake()
    {
        startMenu.SetSoftExitable(false); // the start menu can never be 'left' via esc
        // NEW GAME 
        // CONTINUE (autosave informed)
        // LOAD SAVE (not autosave informed)
        
        Action<int> newGameCall = (x) => { StateManager.NewGameSaveState(x); StateManager.LoadSaveState(x); };
        
        // note that the special InitLSMenu is considered 'not loading' becuase that only matters for
        // what kinds of files are deemed available.
        startMenu.SetCallbackAt(0, "NEW GAME", (x) => InitLSMenu(false, newGameCall));
        
        //startMenu.SetCallbackAt(1...) is something that must be defined in prepare startMenu 
        startMenu.SetCallbackAt(2, "LOAD SAVE", (x) => InitLSMenu(true));
    }

    private void PrepareLS(bool isLoading)
    {
        Action<int> bucketAct;
        
        if (isLoading)
            bucketAct = (x) => { StateManager.LoadSaveState(x); };
        else
            bucketAct = (x) => { StateManager.CreateSaveState(x); StateManager.ExitTopMenu(); };
        PrepareLS(isLoading, bucketAct);
    }
    private void PrepareLS(bool isLoading, Action<int> inputAction) //to be called on observation, not awake
    {
        //needs to look at list and observe all 3 save states to run
        for (int i = 0; i < 3; i++)
        {
            string bucketStr = "SAVE " + i;
            if (isLoading)
                bucketStr = "LOAD " + bucketStr;
            else
                bucketStr = "SET " + bucketStr;
            //bucketStr += StateManager.DCManager.CanLoadInkSaveState(i);
            Action<int> bucketAct;
            if(isLoading)
            {
                if (StateManager.DCManager.CanLoadInkSaveState(i))
                {
                    //note that loading the save state clears the menu stack
                    Action<int> nestedAct = (x) => { inputAction(x); }; 
                    bucketAct = (x) => { InitAYSPopup(bucketStr + "?", nestedAct, x, "DO NOT " + bucketStr); };
                }
                else
                {
                    bucketAct = StateManager.PhonyAction; //literally do nothing on being chosen
                }
            }
            else
            {
                Action<int> nestedAct = (x) => { inputAction(x); };
                bucketAct = (x) => { InitAYSPopup(bucketStr + "?", nestedAct, x, "DO NOT " + bucketStr); };
            }
            string fullStr = bucketStr + StateManager.DCManager.SaveStateDescriptor(i);
            loadSaveMenu.SetCallbackAt(i, fullStr, bucketAct);
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
    private void PrepareStartMenu()
    {
        /* NEW GAME (index 0) was fully set in Awake call
         * LOAD SAVE (index 2) was fully set in Awake call
         */
        //(180, 180, 180, 255); is grey for dialogue! I'm using it here
        string displayText;
        Action<int> call;
        if(StateManager.DCManager.CanLoadInkSaveState(-1)) //i.e. if continue would be legal
        {
            displayText = "CONTINUE";
            call = (x) => StateManager.LoadAutosave();
        }
        else // if continue would be illegal
        {
            displayText = "<color=#B4B4B4FF>CONTINUE</color>";
            call = StateManager.PhonyAction; // i.e do nothing
        }
        startMenu.SetCallbackAt(1, displayText, call);
    }


    

    // are not actually public. I never made broadcasters, so these will appear public
    //currently exclusively for the prompt becuase I couldn't get a better way
    public void OnStateChange(bool inMenu, bool inDialogue, int stateFlag)
    {
        actPromptPanel.SetActive(!(inMenu || inDialogue || (stateFlag != 0)));
    }

}
