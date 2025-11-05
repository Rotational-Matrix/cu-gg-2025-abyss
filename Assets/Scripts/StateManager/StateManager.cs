using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
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


    [SerializeField] private GameObject dialogueCanvas;   //Times of entering and leaving not directly chosen by player (obviously)
    [SerializeField] private GameObject playerMenuCanvas; //like an inventory in many games, something accesible at 'sorcery speed'
    [SerializeField] private GameObject pauseMenuCanvas;  //accessible at 'instant speed'

    //these can be called by other fns' Start() to access pmManager & dcManager
    public static DialogueCanvasManager DCManager { get; private set; }
    public static PauseMenuManager PMManager { get; private set; }

    //there would be a player menu canvas here, if it existed!

    //this is the current stack of menus opened
    public static Stack<IGridSelectable> MenuStack { get; private set; } = new Stack<IGridSelectable>();
    


    private static bool isInDialogue = false;
    private static bool isInPlayerMenu = false;
    private static bool isInPauseMenu = false;

    //getters and setters could be made onto the attributes, but that is weird!
    public static void SetDialogueStatus(bool status)
    { isInDialogue = status; }
    public static bool GetDialogueStatus()
    { return isInDialogue; }
    public static void SetPlayerMenuStatus(bool status)
    { isInPlayerMenu = status; }
    public static bool GetPlayerMenuStatus()
    { return isInPlayerMenu; }
    public static void SetPauseMenuStatus(bool status)
    { isInPauseMenu = status; }
    public static bool GetPauseMenuStatus()
    { return isInPauseMenu; }



    private void Awake()
    {
        //note how, if there are 2 StateManagers, one will overwrite the other! (why would there be 2 of these though...)
        DCManager = dialogueCanvas.GetComponent<DialogueCanvasManager>();
        //initialisation for the canvas of the player menu canvas (when it gets implemented)
        PMManager = pauseMenuCanvas.GetComponent<PauseMenuManager>();
        
    }
}
