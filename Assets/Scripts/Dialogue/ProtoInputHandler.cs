using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ProtoInputHandler : MonoBehaviour
{
    /// <summary>
    /// [Cu]'s documentation
    /// 
    /// Most of the documentation is actually provided below as it should be
    /// 
    /// What is accesible from ProtoInputHandler is abstract keys (as well as the keyboard)
    /// 
    /// How to add a funtions that respond to user input (esp. keypresses)
    ///     - Most Ideal: make function in MenuInputManager and place it in its appropriate spot
    ///         - e.g. 'eve-interact-key' should only work, say, outside of dialogue and menus,
    ///         so its appropriate behvaiour is performed by a funtion in ProtoInputHandler and placed
    ///         properly into the private 'DistributeInput()' method at the bottom where actions
    ///         that only occur outside of menus and dialogue occur.
    ///     - Also Valid: locally handle the update and keypress
    ///         - Obviously, it can be clunky, time-consuming, and/or foolish to handle certain calls in this file
    ///         - Conditions for locally updating and handling the keypresses
    ///             - Please use abstractable keys. Not already here? Add it!
    ///             - Make sure to account for call context (since, otherwise ProtoInputHandler would account for it)
    ///                 - e.g. the beheviour must not occur whenever it shouldn't occur
    ///                 which means that the behaviour must have a way of ascertaining its context
    ///                 
    ///     
    /// 
    /// </summary>

    private DialogueCanvasManager dcManager;
    private PauseMenuManager pmManager;


    // PLEASE: observe unity inputManager 1.15 (or any version, really)
    // I have been using the legacy KeyCode all this time w/o realising it, keyboard and key is so much better!

    //------------------------ keyboard -----------------------------
    public static UnityEngine.InputSystem.Keyboard CurrentKeyboard;

    [SerializeField] public bool DebugKeysActive; //if true, allows dangerous debug actions

    //--------------------- Dialogue Keys ---------------------------

    /* dialogueKey does the following: 
     *  - Progress to the next line of dialogue
     *  - Initiate choice selection (when the dialogue hits a choice)
     */
    public static Key dialogueKey = Key.Enter;

    /* commitChoiceKey does the following:
     *  - Commits the currently selected/highlighted choice
     *  
     *  NOTE: this is currently the SAME key as dialogueKey
     */
    public static Key commitChoiceKey = Key.Enter;

    /* mvSelectUp, mvSelectDown:
     *  - for choice selection, these commands move the selector up and down
     *      - note that 'up' and 'down' refer to how the user perceives up and down
     *      - also note that the functions behind this movement are not as consistent
     *          - i.e. ChoiceCanvas Increment means Down, but GridSelector Increment means up
     *              - I should probably change this, but regardless, these issues are documented as they appear below
     */
    public static Key mvSelectUp = Key.UpArrow;
    public static Key mvSelectDown = Key.DownArrow;

    //------------------- Pause Menu Keys ---------------------------

    public static Key openConfigKey = Key.Tab;
    public static Key exitKey = Key.Escape;

    //------------------- Roam State Keys ---------------------------

    public static Key interactKey = Key.Space;

    //--------------------- Debug Keys ------------------------------

    /* debug_forceStartDialogue does the following:
     *  - opens the dialogue and inkstory at wherever it currently is.
     *  
     * debug_forceJumpDialogue does the following:
     *  - opens the dialogue and inkstory at specified knotName
     *      - knotName can be written in the inspector
     *  
     * NOTE: 
     *  - *not* buttons players should have access to.
     *  - can (and will) break dialogue if used haphazardly
     *  - forced jumping will blow up if a non-real knotName is used
     */
    private static Key debug_forceStartDialogue = Key.P;
    private static Key debug_forceJumpDialogue = Key.O;
    public static Key debug_moveSariel = Key.Q;
    [SerializeField] public string forceJumpKnotName;



    private void Start() //has to be start to guarantee it occurs after StateManager.Awake()
    {
        dcManager = StateManager.DCManager;
        pmManager = StateManager.PMManager;
    }


    private void Awake()
    {
        CurrentKeyboard = UnityEngine.InputSystem.Keyboard.current;
    }

    //------------------ The Update function ------------------------
    private void Update()
    {
        if (CurrentKeyboard.anyKey.wasPressedThisFrame)
        {
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if ((!object.Equals(key, null)) && key.wasPressedThisFrame)
                {
                    DistributeInput(key.keyCode);
                    //Debug.Log("Key pressed: " + key.keyCode);
                }
            }
        }
    }

    private void DistributeInput(Key keyPressed)
    {
        switch (StateManager.CurrMenuType())
        {
            //note that this accounts
            case StateManager.MenuInputType.SelectableGrid:
                GridSelectHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey, exitKey);
                break;
            case StateManager.MenuInputType.DirectKey:
                DirectKeyHandler(keyPressed);
                break;
            case StateManager.MenuInputType.None: // if not in any menus
                if (StateManager.GetDialogueStatus()) //if in dialogue
                    DialogueInputHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey, dialogueKey);
                //if not in dialogue
                NotInMenuMiscHandler(keyPressed, openConfigKey, debug_forceStartDialogue, debug_forceJumpDialogue);
                break;
        }
    }



    //during GridSelectableInput, other types of input are not necessarily cutoff.
    //Note that the escape key is not currently an option to use
    private void GridSelectHandler(Key keyPressed, Key upKey, Key downKey, Key chooseKey, Key exitKey)
    {
        if (keyPressed == upKey)
            StateManager.MenuStack.Peek().IncremElement();
        else if (keyPressed == downKey)
            StateManager.MenuStack.Peek().DecremElement();
        else if (keyPressed == chooseKey)
            StateManager.MenuStack.Peek().ChooseSelected();
        else if (keyPressed == exitKey)
            StateManager.SoftExitTopMenu();
    }

    private void DirectKeyHandler(Key keyPressed)
    {
        StateManager.DirectInputAction(keyPressed);
    }

    //all cases of non menuStack 'pseudo menus' (choice/dialogue)
    private void DialogueInputHandler(Key keyPressed, Key upKey, Key downKey, Key chooseKey, Key dialogueKey)
    {
        /* Why does this look so stupid? Observe:
         *  - OnGui gets called every time an input happens, and the code reaches here every keyboard input
         *      - this is the least taxing when non-relevant keypresses only have to check == with a few keys
         *  - This is not in switchcase because the keys might be assigned to the same button (esp choose & dialogue)
         * 
         */
        
        
        if (keyPressed == upKey) //best to filter out by key first
        {
            if(dcManager.IsChoiceActive())
                dcManager.IncremChoiceSelection();
        }
        if (keyPressed == downKey)
        {
            if (dcManager.IsChoiceActive())
                dcManager.DecremChoiceSelection();
        }
        if (keyPressed == chooseKey)
        {
            if (dcManager.IsChoiceActive())
            {
                dcManager.Choose();
                dcManager.AttemptContinue();
            }
        }
        if (keyPressed == dialogueKey)
        {
            if (!dcManager.IsChoiceActive())
            {
                if (!dcManager.AttemptContinue())
                    dcManager.InitiateChoices();
            }
        }
    }

    //currently handles opening the config menu as well as extern-to-dialogue debug keys
    // note d_ means 'debug key' here, FS: Force Start, FJ: Force Jump
    //TODO: Make it so Escape and Tab (or whatev) can actually leave the menu
    //  - prolly entails adding a 'softExitMenu' and making PMManager give access to its menus.
    private void NotInMenuMiscHandler(Key keyPressed, Key togConfigKey, Key d_FSDialogue, Key d_FJDialogue)
    {
        if (keyPressed == togConfigKey)
            pmManager.InitConfigMenu();
        else if (DebugKeysActive) //everything in here is debug
        {
            if (keyPressed == d_FSDialogue)
                dcManager.InitiateDialogueState(null);
            else if (keyPressed == d_FJDialogue)
                dcManager.DivertTo(forceJumpKnotName);
        }
        if (keyPressed == interactKey)
            StateManager.ExecuteInteract();
    }



 


}
