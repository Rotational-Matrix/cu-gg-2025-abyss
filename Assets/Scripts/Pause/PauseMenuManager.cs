using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField] private GameObject pauseMenuPanel; //presumes there will only be one
    [SerializeField] private TMPro.TMP_Text pauseMenuText;

    [SerializeField] private GameObject menuItemHandler; //presumed to be a collection of menu items w/proper handler


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
    }
    //not implemented!

    
}
