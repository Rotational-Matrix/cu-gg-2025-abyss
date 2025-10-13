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

    //The ink story we're wrapping,
    Story _inkStory;

    //Save states of the ink story
    private List<string> saveStateJsons = new List<string>(); //there will prolly be a constant amount of these (but rn it's ambiguous)

    //Sub-hiererchy UI scripts
    private DialoguePanelHandler dpHandler;
    private ChoiceCanvasHandler ccHandler;

    //Dialogue intermediates (stored here to be processed or applied all at once)
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
        StateManager.SetDialogueStatus(setActive);
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

    public void CreateInkSaveState(int saveStateIndex)
    {
        //if the argument is invalid, chooses to append a new savefile (maybe one may use -1 for deliberate appending)
        if (saveStateIndex < 0 || saveStateJsons.Count <= saveStateIndex) //append saveState to saved saveStates
        {
            saveStateJsons.Add(_inkStory.state.ToJson());
        }
        else
        {
            saveStateJsons[saveStateIndex] = _inkStory.state.ToJson();
        }
    }
    public void RunInkSaveState(int saveStateIndex)
    {
        if (saveStateJsons.Count <= saveStateIndex) //The eror occurs here because this situation is always an accident
        {
            throw new System.ArgumentOutOfRangeException("RunInkSaveState doesn't have a savestate at index: " + saveStateIndex);
        }
        else if (saveStateIndex < 0) //using -1 
        {
            _inkStory.state.LoadJson(saveStateJsons[saveStateJsons.Count - 1]);
        }
        else
        {
            _inkStory.state.LoadJson(saveStateJsons[saveStateIndex]);
        }
    }
    

    public void SetInkVar<T>(string variableName, T newVal) //literally the name of the variable as it appears in the inkstory
    {
        _inkStory.variablesState[variableName] = newVal; //it...allows this (please make the type of newVal match the variable...)
    }
    public T GetInkVar<T>(string variableName)
    {
        return (T)(_inkStory.variablesState[variableName]); //forceful cast... please match types!
    }

    //tags for content at path could be implemented


    // ContinueDialogue returns a bool
    //  - returns true if its parsed text is valid
    //  - returns false if its parsed text is invalid (encountered line command)
    //      - if encountered the stop command, null is considered valid parsed text
    private bool ContinueDialogue()
    {
        string parsedText = ParseCommands(intermediateBodyText);
        if (!Equals(parsedText, null)) //null is passed by ParseCommands for line commands
        {
            HandleLineTags();
            dpHandler.ProgressDialogue(parsedText, currHeader, currLeftSprite, currRightSprite);
            return true;
        }
        else
            return !StateManager.GetDialogueStatus(); //returns false unless the dialogue has been stopped

    }

    private string ParseCommands(string input)
    {
        //should handle line commands and inline commands.
        //  - does NOT actively handle inline commands. //NotImplemented
        //string bucketString = null; //(only for inline)
        if (input.Length >= 3 && input.Substring(0, 3).Equals(">>>")) //hardcoded, bc >>> is expected
        {
            string lineCommand = input.Substring(3).Trim(); //removes >>> and then whitespace at the start and end
            HandleLineCommands(lineCommand);
            return null; //all line commands will return null (even invalid line commands)
        }
        else
            return input; //no need to even set bucketString = input
    }

    private void HandleLineCommands(string command)
    {
        //the most common command, and the only one without a colon. [wait, the other commands have large intestines?]
        if (command.Equals("STOP_DIALOGUE"))
        {
            SetDialogueState(false); // This command wipes the dialogue panel
        }
        else
        {
            Debug.Log("Unknown line command received: " + command);
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
