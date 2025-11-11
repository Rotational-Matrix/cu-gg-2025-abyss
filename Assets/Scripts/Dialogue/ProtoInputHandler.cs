using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ProtoInputHandler : MonoBehaviour
{
    //[SerializeField] private GameObject dialogueCanvas; //will instead be accessed via StateManager

    private DialogueCanvasManager dcManager;
    private PauseMenuManager pmManager;


    // PLEASE: observe unity inputManager 1.15 (or any version, really)
    // I have been using the legacy KeyCode all this time w/o realising it, keyboard and key is so much better!

    /* probably the only public part of ProtoInputHandler
     */
    public static UnityEngine.InputSystem.Keyboard CurrentKeyboard;


    //--------------------- Dialogue Keys ---------------------------

    /* dialogueKey does the following: 
     *  - Progress to the next line of dialogue
     *  - Initiate choice selection (when the dialogue hits a choice)
     */
    private static Key dialogueKey = Key.Space;

    /* commitChoiceKey does the following:
     *  - Commits the currently selected/highlighted choice
     *  
     *  NOTE: this is currently the SAME key as dialogueKey
     */
    private static Key commitChoiceKey = Key.Space;

    /* mvSelectUp, mvSelectDown:
     *  - for choice selection, these commands move the selector up and down
     *      - note that 'up' and 'down' refer to how the user perceives up and down
     *      - also note that the functions behind this movement are not as consistent
     *          - i.e. ChoiceCanvas Increment means Down, but GridSelector Increment means up
     *              - I should probably change this, but regardless, these issues are documented as they appear below
     */
    private static Key mvSelectUp = Key.UpArrow;
    private static Key mvSelectDown = Key.DownArrow;

    //------------------- Pause Menu Keys ---------------------------

    

    private static Key openConfigKey = Key.Tab;

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
                if (object.Equals(key,null))
                    Debug.Log("a key is null!");
                else if (key.wasPressedThisFrame)
                {
                    DistributeInput(key.keyCode);
                }
            }
        }
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            Debug.Log("Any key was pressed using New Input System!");
            // You can iterate through Keyboard.current.allKeys to find out which specific key was pressed
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if ((!object.Equals(key, null)) && key.wasPressedThisFrame)
                {
                    Debug.Log($"Key pressed: {key.name}");
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
                GridSelectHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey);
                break;
            case StateManager.MenuInputType.DirectKey:
                DirectKeyHandler(keyPressed);
                break;
            case StateManager.MenuInputType.None:
                //all cases, including dialogue/choice
                //FIXXXXXXXX
                if (StateManager.GetDialogueStatus())
                    DialogueInputHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey, dialogueKey);
                NotInMenuMiscHandler(keyPressed, openConfigKey, debug_forceStartDialogue, debug_forceJumpDialogue);
                break;
        }
    }



    //during GridSelectableInput, other types of input are not necessarily cutoff.
    //Note that the escape key is not currently an option to use
    private void GridSelectHandler(Key keyPressed, Key upKey, Key downKey, Key chooseKey)
    {
        //I am hesitant to have the fn actively call GetKeyDown, but whatev
        if (keyPressed == upKey)
            StateManager.MenuStack.Peek().IncremElement();
        else if (keyPressed == downKey)
            StateManager.MenuStack.Peek().DecremElement();
        else if (keyPressed == chooseKey)
            StateManager.MenuStack.Peek().ChooseSelected();
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
                dcManager.DecremChoiceSelection(); // upKey decrems choice, but increms menu, because foolishness
        }
        if (keyPressed == downKey)
        {
            if (dcManager.IsChoiceActive())
                dcManager.IncremChoiceSelection();
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
        else if (keyPressed == d_FSDialogue)
            dcManager.InitiateDialogueState(null);
        else if (keyPressed == d_FJDialogue)
            dcManager.DivertTo(forceJumpKnotName);
    }



 


}
