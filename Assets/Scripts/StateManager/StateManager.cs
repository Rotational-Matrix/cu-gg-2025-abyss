using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    /// //[Cu] needs to work this out with John
    /// bool PlayerCanMoveInDialogue
    ///     - defaults false.
    ///     - Right now it is unknown if player will be able to move in dialogue (it may depend on dialogue context)
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
    /// boo; IsInRoam()
    ///     - tells if not in Menu and not in Dialogue
    ///     - literally just uses the above 2 functions
    /// 
    /// StateManager.MenuInputType CurrMenuType() {Usage Not Suggested}
    ///     - ProtoInputHandler calls this to know how to send inputs for each given file
    /// 
    /// void SetDirectAction(Action<UnityEngine.InputSystem.Key> directInputAction) {Usage Not Suggested}
    ///     - this is mostly set by menus that use it (I don't see why this would be used by others)
    /// 
    /// void AddInteraction(InteractableElement interactElem)
    /// void RemoveInteraction(InteractableElement interactElem)
    /// void ClearInteraction()
    ///     - all do the List methods they have in their name to the interactStack
    ///     - interactStack exists because 'interact' is done on a trigger basis, and this solves for when Eve is in >1 trigger
    ///     
    /// void ExecuteInteract()
    ///     - the equivalent of pop, activates the topmost interact (many InteractableElement objs remove themselves on execute)
    /// 
    /// public static void AddMenuStateChangeResponse(IStateManagerListener listener)
    /// public static void RemoveMenuStateChangeResponse(IStateManagerListener listener)
    ///     - If objects implement IStateManagerListener, they can send themself as a listener to execute fns on menu state change
    /// 
    /// 
    /// void PushMenu(IGridSelectable menu)
    ///     - properly pushes the top menu
    ///         - for broadcasting, please call this instead of StackMenu.Push(...)
    /// 
    /// void ExitTopMenu()
    ///     - properly closes the top menu (although all menus can close themselves, and some ought not be closable)
    /// 
    ///
    ///     
    /// void PhonyAction(int useless)
    ///     - Actually does nothing
    ///     - A lot of menus use Action<int'>, and this is a more readable way to call a so-nothing function if needed
    ///     
    /// 
    /// -- special case --
    /// void PopMenu()
    ///     - this has a special purpose for broadcasting reasons. Please use 'ExitTopMenu' instead.
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

    public enum StateFlag
    {
        None = 0,
        MoveAllowedInDialogue = 1,
        InForcedMovement = 2,


    }





    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;

    [SerializeField] private GameObject playerControllerObj;

    //Various objects that StateManager ought know about, but not nee make public.
    [SerializeField] private GameObject masterUICanvas; //exclusively here so it can be turned on instantly
    //[SerializeField] private LeashManager leashManager;
    //[SerializeField] private GameObject leash;


    //these can be called by other fns' Start() to access pmManager & dcManager
    public static DialogueCanvasManager DCManager { get; private set; }
    public static PauseMenuManager PMManager { get; private set; }

    public static PlayerController Eve; //static access to Eve's controller is helpful


    //this is the current stack of menus opened
    public static Stack<IGridSelectable> MenuStack { get; private set; } = new Stack<IGridSelectable>();
    public static Action<UnityEngine.InputSystem.Key> DirectInputAction { get; private set; }





    private static bool isInDialogue = false;
    //note that IsInMenu() serves as as the equivalent for Menus

    //here to handle 'interact' calls
    private static List<InteractableElement> interactStack = new List<InteractableElement>();

    //for handling boradcasting
    private static List<IStateManagerListener> stateListeners = new List<IStateManagerListener>();
    private static StateFlag stateFlag = StateFlag.None;

    public static void SetDialogueStatus(bool dialogueStatus)
    {
        isInDialogue = dialogueStatus;
        BroadcastStateChange(); //broadcasing state change!
    }
    public static bool GetDialogueStatus()
    { return isInDialogue; }

    public static bool IsInMenu()
    {
        return StateManager.MenuStack.Count > 0;
    }
    public static bool IsInRoam()
    {
        return (!IsInMenu()) && (!isInDialogue);
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

    public static void AddInteraction(InteractableElement interactElem)
    { interactStack.Add(interactElem); if (interactStack.Count == 1) PMManager.SetActionPromptActive(true); }
    public static void RemoveInteraction(InteractableElement interactElem)
    {
        if (interactStack.Count != 0)
        {
            interactStack.Remove(interactElem);
            if (interactStack.Count == 0) 
                PMManager.SetActionPromptActive(false);
        }
    }
    public static void ClearInteractStack()
    { interactStack.Clear(); }
    public static void ExecuteInteract()
    {
        if (interactStack.Count != 0)
            interactStack[^1].ExecuteInteract();
    }

    //if a listener adds themself, please let them remove themself too...
    public static void AddStateChangeResponse(IStateManagerListener listener)
    {
        stateListeners.Add(listener);
    }
    public static void RemoveStateChangeResponse(IStateManagerListener listener)
    {
        stateListeners.Remove(listener);
    }

    public static void PushMenu(IGridSelectable menu)
    {
        MenuStack.Push(menu);
        if (MenuStack.Count == 1) //i.e. just entered menu state
            BroadcastStateChange();
    }
    public static void PopMenu()
    {
        MenuStack.Pop();
        if (!IsInMenu()) //i.e. just exited menu state
            BroadcastStateChange();
    }

    //not documented because one should probably not be calling this.
    // this is a change stataeFlag function
    public static void SetPlayerForcedMoveStatus(bool isEntering)
    {
        if (isEntering)
            SetStateFlag(StateFlag.InForcedMovement);
        else
        {
            if(stateFlag == StateFlag.InForcedMovement)
                SetStateFlag(StateFlag.None);
        }
    }

    //Shortcut fns
    public static void ExitTopMenu()
    {
        if (IsInMenu())
        {
            MenuStack.Peek().ExitMenu();
        }
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



    //private broadcasters

    private static void BroadcastStateChange()
    {
        foreach (IStateManagerListener listener in stateListeners)
        {
            listener.OnStateChange(IsInMenu(), isInDialogue, (int)stateFlag);
        }
    }

    private static void SetStateFlag(StateFlag sFlag)
    {
        if (stateFlag != sFlag)
        {
            stateFlag = sFlag;
            BroadcastStateChange();
        }
    }


    public void Start()
    {
        BroadcastStateChange();
    }


    private void Awake()
    {
        //note how, if there are 2 StateManagers, one will overwrite the other! (why would there be 2 of these though...)
        DCManager = dialogueCanvas.GetComponent<DialogueCanvasManager>();
        PMManager = pauseMenuCanvas.GetComponent<PauseMenuManager>();

        Eve = playerControllerObj.GetComponent<PlayerController>();


        masterUICanvas.SetActive(true);
        //leashManager.SetLeashActive(false);
        //leash.SetActive(false);

    }
}
