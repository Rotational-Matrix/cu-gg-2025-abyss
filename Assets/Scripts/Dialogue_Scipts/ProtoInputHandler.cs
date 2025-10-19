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

    /* moveSelectorUp, moveSelectorDown:
     *  - for choice selection, these commands decrement or increment the selector
     * 
     *  - note that incrementing and decrementing refers to order as ChoiceCanvasHandler perceives
     *      - thus, it will likely be that incrementing means going 'down' on screen and vice versa
     */
    private static KeyCode moveSelectorUp = KeyCode.UpArrow;
    private static KeyCode moveSelectorDown = KeyCode.DownArrow;

    //------------------- Pause Menu Keys ---------------------------

    

    private static KeyCode togglePauseMenu = KeyCode.Tab;

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

    // Update is called once per frame
    //DELETE THIS, it is merely a dummy event handler that has been made merely for 'proto'
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
                if (Input.GetKeyDown(moveSelectorUp))
                {
                    dcManager.DecremChoiceSelection();
                }

                if (Input.GetKeyDown(moveSelectorDown))
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
    }
}
