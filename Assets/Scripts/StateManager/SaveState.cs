using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;


public class SaveState : object
{
    /* SaveState exists exclusively as an object to store the game state for saving and loading
     * Should do absolutely nothing else
     * Stored in DCM only because DCM initially stored the json 'save states'
     * 
     * What do save states need to store?
     *  - the 'state json' (contains the ink story state in its entirety)
     *  - metadata regarding savefiles
     *      - name and info of each savefile the user will observe
     *          - may appear in form of "[SAVE DESCRIPTOR]"
     *      - note that only the _inkStory can look at state (stored jsons cannot)
     *      - Thus certain variables that should be viewed prior to loading are important.
     *  - unity variables (oh no)
     *      - eve and sariel 'positions'
     *      - (save loading should be reasonable enough to use the inkstory)
     * 
     * 
     */

    //SaveState is by and large not static. autoSaveState
    private static SaveState newGameSaveState = new SaveState(); //probably has not index
    private static SaveState autoSaveState = new SaveState(); //the 'index -1'
    
    // the state json
    private string json; //this is where the inkstory is kept

    // meta data for saves
    private DateTime timestamp;
    private int endingNum; // by protocol, 0 is not ended, 1 is 'true', 2 is 'rebel'
    private int actNum; // may be implemented in descriptor as, [~~, ACT {actnum}]

    //exist explicitly for knowing about previous routes
    private bool pastRebelEnding;
    private bool pastTrueEnding;

    // OTHER UNITY DETAILS (PLEASE ADD MORE WHERE NECESSARY)
    /* Further notes:
     *  - I have made the inkFile itself hold values regarding most unity states
     *      - This is because SaveState has no idea what anything else is
     *      - StateManager and DCM should handle applying and updating said information
     *  - Current protocol is iffy as to whether to save states at dialogue 'START' or 'STOP'
     *      - It could honestly be both, however here are the requirements given:
     *          - START autosave:
     *              - guarantees beginning in dialogue
     *              - means a Divert must be called on startup (inkfile must store)
     *              - makes writing save via dialogue easier (i.e. sariel says y'wanna save, kid?)
     *          - STOP autoSave:
     *              - guarantees beginning 'out of dialogue'
     *              - means no divert must be called (but user may be confused as to location)
     *              - start of game must be special call because it starts into dialogue
     *              - makes writing save via menu or non-dialogue easier
     *  - in Inkfile, note:
     *      - VAR eve_x1 = 0    //
     *      - VAR eve_x2 = 0    // eve Vector3 for saving position
     *      - VAR eve_x3 = 0    //
     *      - VAR sariel_x1 = 0 //
     *      - VAR sariel_x2 = 0 // Sariel Vector3 for saving position
     *      - VAR sariel_x3 = 0 //
     *
     *      - VAR leashActive = false // starts 'off'
     */




    // statics
    public static void SetPastEndingOn(int endingNum)
    {
        if (endingNum == 1)
            newGameSaveState.pastTrueEnding = true;
        else if (endingNum == 2)
            newGameSaveState.pastRebelEnding = true;
        //else doesn't matter
    }
    public static void AutoSave(Story inkStory) //probably the current inkStory
    {
        autoSaveState.OverwriteSave(inkStory);
    }
    public static bool LoadAutoSave(Story inkStory)
    {
        return autoSaveState.Load(inkStory);
    }
    public static bool CanLoadAutoSave()
    {
        return autoSaveState.CanLoad();
    }
    public static void InitNewGameSaveState(Story inkStory)
    {
        newGameSaveState.OverwriteSave(inkStory);
    }



    public SaveState()
    {
        this.json = "";
    }



    // typical 'Save' (not 'new story')
    public void OverwriteSave(Story inkStory)
    {
        this.timestamp = DateTime.Now;
        this.json = inkStory.state.ToJson();
        this.endingNum = GetVar<int>(inkStory, "reachedEnding");
        this.actNum = GetVar<int>(inkStory, "actNumber");
        this.pastTrueEnding = GetVar<bool>(inkStory, "pastTrueEnding");
        this.pastRebelEnding = GetVar<bool>(inkStory, "pastRebelEnding");
    }
    public void CopyFromAutoSave() //the autoSave equivalent (done AFTER OverwriteSave(_inkStory) into auto)
    {
        CopyFrom(autoSaveState);
    }

    //up to DCM to store the prisitine story save somewhere...
    //  - also! DCM expected to tamper with pristine story! (i.e. change ending previouusly reached)
    public void CopyFromNewGame()
    {
        CopyFrom(newGameSaveState);
    }

    //returns success at loading (insantly gets blocked if not runnable)
    public bool Load(Story inkStory)
    {
        if (!CanLoad()) 
            return false;
        inkStory.state.LoadJson(this.json);
        SetVar<bool>(inkStory, "pastTrueEnding", pastTrueEnding);
        SetVar<bool>(inkStory, "pastRebelEnding", pastRebelEnding);
        //note that all unity vars are stored in the script!
        return true;
    }


    //when saves are shown 
    public string Descriptor()
    {
        if (IsEmpty())
            return " [EMPTY]"; 
        else if (IsEnded())
            return " [" + TimestampStr() + ", " + EndingStr() + "]";
        else
            return " [" + TimestampStr() + ", " + ActStr() + "]";
    }

    public bool CanLoad()
    {
        return !(IsEmpty() || IsEnded());
    }

    private bool IsEmpty()
    {
        return this.json.Equals("");
    }
    private bool IsEnded()
    {
        return endingNum != 0;
    }

    private string TimestampStr()
    {
        return timestamp.ToString("MM-dd HH:mm");
    }
    private string ActStr()
    {
        if(actNum == 0)
            return "NEW GAME";
        else
            return "ACT " + actNum;
    }
    private string EndingStr()
    {
        return "<color=\"red\">ENDING " + endingNum + "</color>";
    }

    private T GetVar<T>(Story inkStory, string variableName) //shortcut, remember to match types pls!!!
    {
        return (T)(inkStory.variablesState[variableName]);
    }
    private void SetVar<T>(Story inkStory, string variableName, T newVal)
    {
        inkStory.variablesState[variableName] = newVal;
    }

    // REMEMBER! REMEMBER! REMEMBER!
    // When adding fields, update the CopyFrom method!
    // REMEMBER! REMEMBER! REMEMBER!
    private void CopyFrom(SaveState other)
    {
        this.timestamp = DateTime.Now;
        this.json = other.json;
        this.endingNum = other.endingNum;
        this.actNum = other.actNum;
        this.pastTrueEnding = other.pastTrueEnding;
        this.pastRebelEnding = other.pastRebelEnding;
    }



}
