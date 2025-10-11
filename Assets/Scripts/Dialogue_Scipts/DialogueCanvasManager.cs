using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueCanvasManager : MonoBehaviour
{
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


    private void Start()
    {
        _inkStory = new Story(inkAsset.text);
        dpHandler = dialoguePanel.GetComponent<DialoguePanelHandler>();
        ccHandler = choiceCanvas.GetComponent<ChoiceCanvasHandler>();
    }

    public bool InitiateDialogueState(string knotName) //also "knotName.stitchName" is valid
    {
        SetDialogueState(true);
        DivertTo(knotName);
        return AttemptContinue();
    }

    private void SetDialogueState(bool setActive)
    {
        dialoguePanel.SetActive(setActive);
        //THIS NEEDS TO TELL WHATEVER OVERALL ARCHITECTURE THAT THE DIALOGUE STATE HAS CHANGED //NotImplemented
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
            bool validText = ContinueDialogue(); //if text is invalid (as in 'encontered line command'), recurse!

            //the recursion allows AttemptContinue to process each line command,
            //while ultimately skipping past all of them from the user's perspective 
            if(!validText)
                canContinue = AttemptContinue();
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
    public bool IsChoiceActive()
    {
        return ccHandler.IsDisplaying();
    }


    public void DivertTo(string knotName) //or knotName.stitchName
    {
        _inkStory.ChoosePathString(knotName);
    }

    /* TO BE IMPLEMENTED
     string savedJson = _inkStory.state.ToJson();
     _inkStory.state.LoadJson(savedJson);
     */
    public string CreateInkSaveState()
    {
        throw new System.NotImplementedException("CreateInkSaveState isn't implemented yet");
    }
    public void RunInkSaveState(string savedJson)
    {
        throw new System.NotImplementedException("RunInkSaveState isn't inplemented");
    }
    

    public void SetInkVar()
    {
        throw new System.NotImplementedException("SetInkVar isn't inplemented");
    }
    public void GetInkVar()
    {
        throw new System.NotImplementedException("GetInkVar isn't inplemented");
    }

    //tags for content at path could be implemented


    // ContinueDialogue returns a bool
    //  - returns true if its parsed text is valid
    //  - returns false if its parsed text is invalid (encountered line command)
    //      - if encountered the stop command, null is considered valid parsed text
    private bool ContinueDialogue()
    {
        string parsedText = ParseCommands(intermediateBodyText);
        if (!parsedText.Equals(null)) //null is passed by ParseCommands for line commands
        {
            HandleLineTags();
            dpHandler.ProgressDialogue(parsedText, currHeader, currLeftSprite, currRightSprite);
            return true;
        }
        else
            return !dialoguePanel.activeSelf; //returns false unless the dialogue panel is also disabled

    }

    private string ParseCommands(string input)
    {
        //should handle line commands and inline commands.
        //  - does NOT actively handle inline commands. //NotImplemented
        //string bucketString = null; //(only for inline)
        string linePrefix = input.Substring(0, 3); //hardcoded, bc >>> is expected //FIXXXXXXXXXXXXX
        if (linePrefix.Equals(">>>"))
        {
            //note that line commands will likely return either null
            //  or call _inkStory.Continue() and then return that instead
            string lineCommand = input.Substring(3).Trim(); //removes >>> and then whitespace at the start and end
            HandleLineCommands(lineCommand);
            return null;
        }
        else
            return input; //no need to even set bucketString = input
    }

    private void HandleLineCommands(string command)
    {
        //the most common command, and the only one without a colon.
        if (command.Equals("STOP_DIALOGUE"))
        {
            SetDialogueState(false); // This command wipes the dialogue panel
        }
        else
        {
            //where other commands would go.
            //they are distinct because virtually all commands will have a ':'
        }
    }
    

    /* Note that HandleLineTags actually queries the inkstory for if there are ink tags on its call.
     *  - currTags are ONLY updated with new tags from the inkstory when this is called
     *      - this is because currTags are meant for regular lines of text
     *      - ParseCommands line commands may call _ink.Story.currentTags, but won't touch currTags
     *  - These features are intended for ParseCommands being called prior to HandleLineTags when reading text
     *      - ParseCommands will likely call _inkStory.Continue() for line commands
     *          - in this case, HandleLineTags should get the tags for the new line, not the old one
     */
    private void HandleLineTags()
    {
        currTags = _inkStory.currentTags;
        foreach(string tag in currTags)
        {
            if (!HandleColonKeyTags(tag))
            {
                /* only colon key tags are being handled right now.
                 *  - other types of handlers would go here
                 *  - this area is only reached when a tag is NOT a colon key tag
                 */
            }
        }
    }


    // for the sub tag interpret methods
    // if finds tag and handles it, returns true
    // else returns false
    private bool HandleColonKeyTags(string tag)
    {
        //checks a single string for a colon,
        // if it has it, checks if the terms before match
        bool isHandled = true;
        int colonIndex = tag.IndexOf(':');
        if (colonIndex != -1)
        {
            string preColon = tag.Substring(0, colonIndex); //doesn't include the colon
            string postColon = tag.Substring(colonIndex + 1);
            switch (preColon) //this could always be ToUpper-ed to allow for case insensitivity
            {
                case "speaker":
                    HandleSpeakerTag(postColon);
                    break;
                case "lSprite":
                    HandleSpriteTag(postColon, true);
                    break;
                case "rSprite":
                    HandleSpriteTag(postColon, false);
                    break;
                case "audio": //no shot this is implemented
                    HandleAudioTag(postColon);
                    break;
                default:
                    isHandled = false;
                    break;
            }
        }
        else
            isHandled = false;
        return isHandled;
    }

    private void HandleSpeakerTag(string tag)
    {
        //note that there will be many instances of non-character text
        //we may choose to have "" or "..." or whatever the protags name is
        currHeader = tag; 
    }

    /* note that for non-speaker tags, the tags should be FILEPATHS
     *  - the filepaths start as if they originate at the Resources folder
     *  - the filepaths ultimately do NOT include the extension
     */
    private void HandleSpriteTag(string tag, bool isLeft)
    {
        //for errors here, note that file order must be preserved in Resources subfiles
        //  - so check there!
        if (isLeft)
            currLeftSprite = Resources.Load<Sprite>(tag);
        else
            currRightSprite = Resources.Load<Sprite>(tag);
    }

    private void HandleAudioTag(string tag)
    {
        throw new System.NotImplementedException("HandleAudioTag isn't implemented yet");
    }







}
