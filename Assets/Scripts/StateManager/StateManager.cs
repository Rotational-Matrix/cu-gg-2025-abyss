using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    /// <summary>
    /// Part of [Cu]'s documentation for external usage of StateManager
    /// 
    /// note that you can always ctrl-f these to find them in the code.
    /// 
    /// StateManager is effectively static (all interactions with it should be with static methods and fields) 
    /// 
    /// All methods and fields shown here are public static (all fields are public get; private set)
    /// 
    /// Fields:
    /// DialogueCanvasManager DCManager
    ///     - DialogueCanvasManager has its own documentation like this one
    ///     - Calling DCManager via StateManager is the expected way of accessing it
    ///     - DCManager contains the inkstory and all of the functions that operate on it
    ///     - NOTE: this is probably one of the most useful ones!!! Pls go see its documentation!
    /// 
    /// PauseMenuManager PMManager
    ///     - PauseMenuManager has its own documentation like this one
    ///     - Since it isn't actually static, accessing it through StateManager is the way to access it like it is.
    ///     - PMManager contains the functions that all menu screens (including AreYouSure? and ENTER NUMBER popups)
    /// 
    /// Stack<IGridSelectable> MenuStack
    ///     - The stack that handles the MenuState
    ///     - If this is empty, it means no menus are active.
    ///     - The top menu is often peeked in code to make sure only the top-most menu handles input
    /// 
    /// Action<UnityEngine.InputSystem.Key> DirectInputAction
    ///     - Certain menus or gamestates need to specially handle all keyboard input (e.g. for typing numbers)
    ///     - DirectInputAction is currently only related to menus, and is called by ProtoInputHandler for certain menus
    /// 
    /// Methods!
    /// void SetDialogueStatus(bool status)        {Usage Not Suggested}
    ///     - This is called by DCManager whenever dialogue starts or stops. No need to call this.
    ///     
    /// bool GetDialogueStatus()
    ///     - Tells if dialogue is active
    ///     - NOTE: dialogue is *not* a Menu, so this method is used to tell if the game state is currently in dialogue
    /// 
    /// bool IsInMenu()
    ///     - Tells if there are any menus opened.
    ///     - NOTE: only informs if any menus are open, and does *not* say anything about dialogue
    /// 
    /// StateManager.MenuInputType CurrMenuType() {Usage Not Suggested}
    ///     - ProtoInputHandler calls this to know how to send inputs for each given file
    /// 
    /// void SetDirectAction(Action<UnityEngine.InputSystem.Key> directInputAction) {Usage Not Suggested}
    ///     - this is mostly set by menus that use it (I don't see why this would be used by others)
    /// 
    /// Shortcut methods!!
    /// void ExitTopMenu()
    ///     - properly closes the top menu (although all menus can close themselves, and some ought not be closable)
    ///     
    /// void PhonyAction(int useless)
    ///     - Actually does nothing
    ///     - A lot of menus use Action<int'>, and this is a more readable way to call a so-nothing function if needed
    ///     
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>





    /* This is the beginning of the overarching state manager (hooray!)
     * For menus and dialogue (since they can all technically be simultaneously active), they are exhaustively listed as booleans
     *  - note that these menu states will likely have a canvas that handles submenus, but the StateManager will only care abt each canvas
     * 
     * How will things access the stateManager?
     *  - it should primarily be static. Some things necessitate its presence on an intangible GameObject
     * 
     * 
     * For 'Roam State' (or whatever y'all want to call it), feel free to add anything here too!
     * The menus are placed 'over'  'Roam' State, so 'Roam State' never dissappears (unless it wants to), it just has something over it
     * 
     */

    public enum MenuInputType
    {
        None,
        SelectableGrid,
        DirectKey
    }

    [SerializeField] public static bool disablePlayerMotionDuringDialogue = true;
    [SerializeField] private GameObject dialogueCanvas;   //Times of entering and leaving not directly chosen by player (obviously)
    //[SerializeField] private GameObject playerMenuCanvas; //like an inventory in many games, something accesible at 'sorcery speed'
    [SerializeField] private GameObject pauseMenuCanvas;  //accessible at 'instant speed'

    [SerializeField] private GameObject playerControllerObj;

    //these can be called by other fns' Start() to access pmManager & dcManager
    public static DialogueCanvasManager DCManager { get; private set; }
    public static PauseMenuManager PMManager { get; private set; }

    private static PlayerController playerController; //not actively being used


    //this is the current stack of menus opened
    public static Stack<IGridSelectable> MenuStack { get; private set; } = new Stack<IGridSelectable>();
    public static Action<UnityEngine.InputSystem.Key> DirectInputAction { get; private set; }


    private static bool isInDialogue = false;
    //private static bool isInPlayerMenu = false;
    private static bool isInPauseMenu = false;

    //getters and setters could be made onto the attributes, but that is weird!
    public static void SetDialogueStatus(bool status)
    {
        isInDialogue = status;
        //disables player motion during dialogue (preferable? i'm uncertain)
        if (disablePlayerMotionDuringDialogue)
        {
            if (status) playerController.OnDisable();
            else playerController.OnEnable();
        }
    }
    public static bool GetDialogueStatus()
    { return isInDialogue; }

    public static bool IsInMenu()
    {
        return StateManager.MenuStack.Count > 0;
    }
    public static StateManager.MenuInputType CurrMenuType()
    {
        if (IsInMenu())
        {
            return MenuStack.Peek().InputType();
        }
        else
            return MenuInputType.None;
    }

    public static void SetDirectAction(Action<UnityEngine.InputSystem.Key> directInputAction)
    {
        StateManager.DirectInputAction = directInputAction;
    }

    //Shortcut fns
    public static void ExitTopMenu()
    {
        if (IsInMenu())
            MenuStack.Peek().ExitMenu();
    }
    public static void SoftExitTopMenu()
    {
        if (IsInMenu())
            MenuStack.Peek().SoftExitMenu();
    }
    public static void PhonyAction(int useless)
    {
        //This actually *should* to nothing. It is a phony Action<int>
    }


    
    /*public static void SetPlayerMenuStatus(bool status)
    { isInPlayerMenu = status; }
    public static bool GetPlayerMenuStatus()
    { return isInPlayerMenu; }*/
    public static void SetPauseMenuStatus(bool status)
    { isInPauseMenu = status; }
    public static bool GetPauseMenuStatus()
    { return isInPauseMenu; }
    

    



    private void Awake()
    {
        //note how, if there are 2 StateManagers, one will overwrite the other! (why would there be 2 of these though...)
        DCManager = dialogueCanvas.GetComponent<DialogueCanvasManager>();
        PMManager = pauseMenuCanvas.GetComponent<PauseMenuManager>();

        playerController = playerControllerObj.GetComponent<PlayerController>();

        
    }
}
