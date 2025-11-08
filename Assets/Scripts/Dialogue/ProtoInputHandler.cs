using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProtoInputHandler : MonoBehaviour
{
    //[SerializeField] private GameObject dialogueCanvas; //will instead be accessed via StateManager

    private DialogueCanvasManager dcManager;
    private PauseMenuManager pmManager;

    //--------------------- Dialogue Keys ---------------------------

    /* dialogueKey does the following: 
     *  - Progress to the next line of dialogue
     *  - Initiate choice selection (when the dialogue hits a choice)
     */
    private static KeyCode dialogueKey = KeyCode.Space;

    /* commitChoiceKey does the following:
     *  - Commits the currently selected/highlighted choice
     *  
     *  NOTE: this is currently the SAME key as dialogueKey
     */
    private static KeyCode commitChoiceKey = KeyCode.Space;

    /* mvSelectUp, mvSelectDown:
     *  - for choice selection, these commands move the selector up and down
     *      - note that 'up' and 'down' refer to how the user perceives up and down
     *      - also note that the functions behind this movement are not as consistent
     *          - i.e. ChoiceCanvas Increment means Down, but GridSelector Increment means up
     *              - I should probably change this, but regardless, these issues are documented as they appear below
     */
    private static KeyCode mvSelectUp = KeyCode.UpArrow;
    private static KeyCode mvSelectDown = KeyCode.DownArrow;

    //------------------- Pause Menu Keys ---------------------------

    

    private static KeyCode openConfigKey = KeyCode.Tab;

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
    private static KeyCode debug_forceStartDialogue = KeyCode.P;
    private static KeyCode debug_forceJumpDialogue = KeyCode.O;
    [SerializeField] public string forceJumpKnotName;


    private void Start() //has to be start to guarantee it occurs after StateManager.Awake()
    {
        dcManager = StateManager.DCManager;
        pmManager = StateManager.PMManager;
    }

    //------------------ The OnGUI function -------------------------
    private void OnGUI()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            KeyCode keyPressed = e.keyCode;
            DistributeInput(keyPressed);
        }
    }

    private void DistributeInput(KeyCode keyPressed)
    {
        switch (StateManager.CurrMenuType())
        {
            //note that this accounts
            case StateManager.MenuInputType.SelectableGrid:
                GridSelectHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey);
                break;
            case StateManager.MenuInputType.DirectKeyCode:
                DirectKeyCodeHandler(keyPressed);
                break;
            case StateManager.MenuInputType.None:
                //all cases, including dialogue/choice
                if (StateManager.GetDialogueStatus())
                    DialogueInputHandler(keyPressed, mvSelectUp, mvSelectDown, commitChoiceKey, dialogueKey);
                NotInMenuMiscHandler(keyPressed, openConfigKey, debug_forceStartDialogue, debug_forceJumpDialogue);
                break;
        }
    }



    //during GridSelectableInput, other types of input are not necessarily cutoff.
    //Note that the escape key is not currently an option to use
    private void GridSelectHandler(KeyCode keyPressed, KeyCode upKey, KeyCode downKey, KeyCode chooseKey)
    {
        //I am hesitant to have the fn actively call GetKeyDown, but whatev
        if (keyPressed == upKey)
            StateManager.MenuStack.Peek().IncremElement();
        else if (keyPressed == downKey)
            StateManager.MenuStack.Peek().DecremElement();
        else if (keyPressed == chooseKey)
            StateManager.MenuStack.Peek().ChooseSelected();
    }

    private void DirectKeyCodeHandler(KeyCode keyPressed)
    {
        StateManager.DirectInputAction(keyPressed);
    }

    //all cases of non menuStack 'pseudo menus' (choice/dialogue)
    private void DialogueInputHandler(KeyCode keyPressed, KeyCode upKey, KeyCode downKey, 
                                        KeyCode chooseKey, KeyCode dialogueKey)
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
    private void NotInMenuMiscHandler(KeyCode keyPressed, KeyCode togConfigKey, 
                                        KeyCode d_FSDialogue, KeyCode d_FJDialogue)
    {
        if (keyPressed == togConfigKey)
            pmManager.InitConfigMenu();
        else if (keyPressed == d_FSDialogue)
            dcManager.InitiateDialogueState(null);
        else if (keyPressed == d_FJDialogue)
            dcManager.DivertTo(forceJumpKnotName);
    }


    




    // Update is called once per frame
    //DELETE THIS, it is merely a dummy event handler that has been made merely for 'proto'
    /*
    void Update()
    {
        if(Input.GetKeyDown(togglePauseMenu))
        {
            pmManager.ToggleMenu();
        }
        
        if (StateManager.GetDialogueStatus())
        {

            if (dcManager.IsChoiceActive())
            {
                if (Input.GetKeyDown(mvSelectUp))
                {
                    dcManager.DecremChoiceSelection();
                }

                if (Input.GetKeyDown(mvSelectDown))
                {
                    dcManager.IncremChoiceSelection();
                }

                if (Input.GetKeyDown(commitChoiceKey))
                {
                    dcManager.Choose();
                    dcManager.AttemptContinue();
                }
            }
            else if (Input.GetKeyDown(dialogueKey))
            {
                if (!dcManager.AttemptContinue())
                {
                    dcManager.InitiateChoices();
                }
            }
        }
        else //when dialogue state is not active
        {
            if (Input.GetKeyDown(debug_forceStartDialogue))
            {
                dcManager.InitiateDialogueState(null);
            }

            if(Input.GetKeyDown(debug_forceJumpDialogue))
            {
                dcManager.DivertTo(forceJumpKnotName);
            }
        }
    }*/


}
