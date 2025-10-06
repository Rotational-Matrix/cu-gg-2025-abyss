using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class Dialogue_canvas_manager : MonoBehaviour
{
    // L + Ratio I haven't implemented anything yet
    /* The plan is to make this the top-level communicator for the whole Dialogue_Canvas
     * So observe the following lower-level files
     *  - The Choice Canvas should handle all choice boxes collectively
     *      - ChoiceBoxHandler will be present for each ChoiceBox
     * 
     * This should be the code that does the following:
     *  - communicates and directs the ink file (obtains strings and #tags from it, too)
     *  - parse strings from said ink file for commands
     *  - populate the main dialogue body
     *      - should handle text crawl and text speed-the-hell-up (i.e. spontaneously ending text crawl)
     *  - fill in the sprites of the speakers (or lacktherof)
     *  - perish
     *  - play sound for dialogue/narration (???)
     *  - Activates/disables the choice canvas, sends the choice strings and receives the choice index
     */


    //this script file will get rather massive, as it will control the following:
    // - the choice canvas
    // - the dialogue panel (and the things it handles)
    // - the Inkstory

    [SerializeField] private GameObject choiceCanvas; // expected to have proper Handler
    [SerializeField] private GameObject dialoguePanel;// expected to have proper Handler

    //this should be set to the compiled json asset (not the ink itself)
    public TextAsset inkAsset;

    //The ink story we're wrapping
    Story _inkStory;

    private DialoguePanelHandler dpHandler;
    private ChoiceCanvasHandler ccHandler;


    private Sprite currLeftSprite = null;
    private Sprite currRightSprite = null;
    private string currHeader = "";
    private string intermediateBodyText = "";
    private List<string> currTags = new List<string>();


    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        dpHandler = dialoguePanel.GetComponent<DialoguePanelHandler>();
        ccHandler = choiceCanvas.GetComponent<ChoiceCanvasHandler>();
    }

    //attempts to continue and returns if it failed.
    //this is done because we may desire to allow choice selection at this point
    //however, it is normally a good idea to separate the commands for coninue & choose
    public bool AttemptContinue()
    {
        bool canContinue = _inkStory.canContinue;
        if (canContinue)
        {
            intermediateBodyText = _inkStory.Continue();
            ContinueDialogue();
        }
        return canContinue;
    }


    //The following are the choice handler methods:
    public bool InitiateChoices()
    {
        if (_inkStory.currentChoices.Count > 0)
        {
            string[] arr = new string[_inkStory.currentChoices.Count];
            for (int i = 0; i < _inkStory.currentChoices.Count; ++i)
            {
                Choice choice = _inkStory.currentChoices[i];
                arr[i] = choice.text;
            }
            return ccHandler.InitiateChoices(arr);
        }
        else 
            return false;
    }

    public void Choose()
    {
        //currently presumes choices are 0-indexed
        _inkStory.ChooseChoiceIndex(ccHandler.Choose());
    }
    public void IncremChoiceSelection()
    {
        ccHandler.IncremChoiceSelection();
    }
    public void DecremChoiceSelection()
    {
        ccHandler.DecremChoiceSelection();
    }


    public void DivertTo(string knotName) //or knotName.stitchName
    {
        _inkStory.ChoosePathString(knotName);
    }

    /* TO BE IMPLEMENTED
     string savedJson = _inkStory.state.ToJson();
     _inkStory.state.LoadJson(savedJson);
     */

    public void SetInkVar()
    {
        //NOT IMPLEMENTED
    }
    public void GetInkVar()
    {
        //NOT IMPLEMENTED
    }

    //tags for content at path could be implemented



    private void ContinueDialogue()
    {
        string parsedText = ParseCommands(intermediateBodyText);
        interpretTags();
        dpHandler.ProgressDialogue(parsedText, currHeader, currLeftSprite, currRightSprite);
    }

    private string ParseCommands(string input)
    {
        //since there are no commands yet, parsing for commands is pretty empty, I'm still going to call it though
        return input;
    }


    private void interpretTags()
    {
        currTags = _inkStory.currentTags;
        if (currTags.Count > 0)
        {
            //pls for loop through each tag and report 
            //  if they start with any designated starters
        }
    }







}
