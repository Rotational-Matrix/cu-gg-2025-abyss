//using Ink.Parsed;
using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCanvasManager : MonoBehaviour
{
    /// <summary>
    /// [Cu]'s Documentation
    /// 
    /// For DialogueCanvasManager, nothing is static, 
    /// so everything is always accessed via 'StateManager.DCManager'
    /// 
    /// There are a lot of public methods here, these are not in the order they appear in the file
    ///     - for this file, I have specifically placed the most useful upfront
    /// 
    /// ------- methods useful for non-ProtoInputHandler calls --------
    /// bool InitiateDialogueState(string knotName)
    ///     - Diverts to a knot or knot.stitch in the inkstory, enters dialogue, and calls AttemptContinue()
    ///     - Returns the result of the AttemptContinue()
    ///     - Calling with knotName null is allowed but dangerous and reserved for debugging only
    ///     - This is the only call one needs to make in order to start dialogue
    /// 
    /// void SetInkVar<T>(string variableName, T newVal)
    ///     - Simply sets a variable in the inkStory (name must match VAR name exactly!)
    ///         - [Cu] has already started placing some VARs in ink, use inky or unity's ink inspector to see them
    ///         - it is preferred that unity call ink rather than ink call unity
    ///
    /// T GetInkVar<T>(string variableName)
    ///     - Simply gets a variable from the inkstory (name must match VAR name exactly!)
    /// 
    /// void CreateInkSaveState(int saveStateIndex)
    ///     - sets a saveState to a particular index
    ///     - currently only 3 saveslots are expected (but the method itself works with a arbitrary length list
    ///     
    /// void RunInkSaveState(int saveStateIndex)
    ///     - Loads save state from stored save state
    ///     
    /// bool IsInkSaveStateEmpty(int saveStateIndex)
    ///     - asks if a particular save state is filled or not
    /// 
    /// ----------------------------------------------------------------
    ///     
    /// bool AttemptContinue()
    ///     - Tries to call the next line of inkStory and populate it.
    ///     - Returns true if it gets the next line, false otherwise
    ///         - false indicates a choice ahead, or >>> "STOP_DIALOGUE" encountered
    ///     - On line commands that are not ">>> STOP_DIALOGUE"
    ///         - executes them in order of appearance until encountering a regular line, choice, or stop command
    ///     - NOTE: don't call this directly, the ProtoInputHandler should have it covered
    /// 
    /// bool InitiateChoices()
    ///     - Activates and presents the choice canvas and according choices
    ///     - returns true on success and false on failure
    ///         - false should not occur and indicates a genuine error
    /// 
    /// void Choose()
    ///     - Chooses the currently seleted choice (same as IGridSelector)
    /// 
    /// void IncremChoiceSelection()
    ///     - moves the selector 'up' to the player (was down, but changed to be same as IGridSelector)
    ///     - actually decrements the associated index
    /// 
    /// void DecremChoiceSelection()
    ///     - moves the selector 'down' to the player (was up, but changed to be same as IGridSelector)
    ///     - actually increments the associated index
    /// 
    /// bool IsChoiceActive()
    ///     - returns if the dialogue is currently undergoing a choice
    /// 
    /// 
    /// void DivertTo(string knotName)
    ///     - moves the location in the inkstory without initialising dialogue.
    ///     - NOTE: This is public for debugger keys, please call InitiateDialogueState(string knotName) instead
    ///     
    /// 
    /// </summary>


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
    private List<string> saveStateJsons = new List<string>(); // there are constantly 4, but I dond't want to harcode it

    //Sub-hiererchy UI scripts
    private DialoguePanelHandler dpHandler;
    private ChoiceCanvasHandler ccHandler;

    //Dialogue intermediates (stored here to be processed or applied all at once)
    private Sprite currLeftSprite = null;
    private Sprite currRightSprite = null;
    private string currHeader = "";
    private string intermediateBodyText = "";
    private List<string> currTags = new List<string>();

    //informs how to read text
    private bool readAsStageLines = true;


    private void Awake()
    {
        _inkStory = new Story(inkAsset.text);
        dpHandler = dialoguePanel.GetComponent<DialoguePanelHandler>();
        ccHandler = choiceCanvas.GetComponent<ChoiceCanvasHandler>();
        SetDialogueState(false); //it automatically makes sure it is turned off at start.
        
        for(int i = 0; i < 3; i++)
            saveStateJsons.Add(""); //it is just easier to create a fake buffer.
    }

    public bool InitiateDialogueState(string knotName) //also "knotName.stitchName" is valid
    {
        bool divertBlocked = false;
        if (!Equals(knotName, null))
        {
            divertBlocked = DivertTo(knotName); //in case one wishes to intentionally not jump, but return to dialogue state
        }
        if (!divertBlocked)
        {
            SetDialogueState(true);
            return AttemptContinue();
        }
        return false;  //mimics failing, although divert blocking is acceptable
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
            if (!validText)
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


    public bool DivertTo(string knotName) //or knotName.stitchName
    {
        //Debug.Log("Diverting to knot: " + knotName);
        bool incurBlock = HandleKnotTags(knotName);
        _inkStory.ChoosePathString(knotName);
        return incurBlock;
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
        if (saveStateJsons.Count <= saveStateIndex || saveStateIndex < 0 || string.Equals(saveStateJsons[saveStateIndex], ""))
        {
            throw new System.ArgumentOutOfRangeException("RunInkSaveState doesn't have a savestate at index: " + saveStateIndex);
        }
        else
        {
            _inkStory.state.LoadJson(saveStateJsons[saveStateIndex]);
        }
    }
    public bool IsInkSaveStateEmpty(int index)
    {
        //all considered 'empty' for all intents and purposes
        return saveStateJsons.Count <= index || index < 0 || string.Equals(saveStateJsons[index], "");
    }

    public void SetInkVar<T>(string variableName, T newVal) //literally the name of the variable as it appears in the inkstory
    {
        _inkStory.variablesState[variableName] = newVal; //it...allows this (please make the type of newVal match the variable...)
    }
    public T GetInkVar<T>(string variableName)
    {
        return (T)(_inkStory.variablesState[variableName]); //forceful cast... please match types!
    }

    //##### only on DCM initiated diverting are KnotTags handled #####
    // returns true if divert is blocked
    private bool HandleKnotTags(string knotName) //or knotName.stitchName
    {

        foreach (string tag in _inkStory.TagsForContentAtPath(knotName))
        {
            int result = CheckDivertBlockTags(tag);
            if (result == -1)
                return true; //encountered
            else if (result == 0)
                HandleColonKeyTags(tag);
            // skip handling the tag via colon tags in 'case: 1'
        }
        return false;
    }

    // ContinueDialogue returns a bool
    //  - returns true if its parsed text is valid
    //  - returns false if its parsed text is invalid (encountered line command)
    //      - if encountered the stop command, null is considered valid parsed text
    private bool ContinueDialogue()
    {
        string parsedText = ParseCommands(intermediateBodyText);
        if (!Equals(parsedText, null)) //null is passed by ParseCommands for line commands
        {
            HandleLineTags(); //note that line tags are handled after all actions of ParseCommands
            dpHandler.ProgressDialogue(parsedText, currHeader, currLeftSprite, currRightSprite);
            return true;
        }
        else
            return !StateManager.GetDialogueStatus(); //returns false unless the dialogue has been stopped

    }

    private string ParseCommands(string input)
    {
        //should handle line commands and inline commands.
        if (input.Length >= 3 && input.Substring(0, 3).Equals(">>>")) //hardcoded, bc >>> is expected
        {
            string lineCommand = input.Substring(3).Trim(); //removes >>> and then whitespace at the start and end
            HandleLineCommands(lineCommand);
            return null; //all line commands will return null (even invalid line commands)
        }
        else
            return HandleInlineCommands(input); //no need to even set bucketString = input
    }

    private void HandleLineCommands(string command)
    {
        switch (command)
        {
            case "STOP_DIALOGUE": //wipes dialogue panel
                SetDialogueState(false);
                break;
            case "START_DIALOGUE": //phony, done to allow knot tags to work
                break;
            default:
                Debug.Log("Unknown line command received: " + command);
                break;
        }
    }


    /* Handle Inline Commands
     * Upon finding an inline operator, calls the appropriate methods.
     * In case of no inlines found, returns the same input (after all, most lines will have no inlines
     * 
     * This is primarily here because TMPro supports 'rich text tags' which can affect the conditions of individual words.
     *  - rich text tags will always be misinterpreted by ink, because ink sees <~~~> as not a string
     *  - rich text tags seem to follow the following format:
     *      - <category=value>lorem ipsum solem dicut</category>
     *          - note that the tag will only apply between the two parts of the tag
     *  - our inlines will use the following format:
     *      - $category:value$lorem ipsum solem dicut$/category$
     *          - note that, in case a '$' is needed, the inline will turn $$ -> $
     *          - also, odd numbers of non-$$ $ will leave the last $ as $
     *          - inside $~~~~$, : turns to = to make sure nothing is being assigned
     * 
     * In general, ink seems to ignore '$,' So I intend to use this for indication of inlines (LaTeX approved)
     */
    private string HandleInlineCommands(string input)
    {
        if (readAsStageLines)
            return HandleRichText(HandleInlineSpeaker(input));
        else
            return HandleRichText(input);
    }
    private string HandleInlineSpeaker(string input)
    {
        int colonIndex = tag.IndexOf(':');
        if (colonIndex == -1)
        {
            HandleSpeakerTag("NO_SPEAKER");
            return input;
        }
        string preColon = tag.Substring(0, colonIndex).Trim(); //doesn't include the colon
        string postColon = tag.Substring(colonIndex + 1).Trim();
        //note that, if there exists a colon in the text, either a real speaker or NO_SPEAKER is expected
        HandleSpeakerTag(preColon);
        return postColon;
    }
    private string HandleRichText(string input)
    {
        //currently only made to handle rich text, so it is handled in the main inline function
        int i1 = input.IndexOf('$');
        if (i1 == -1 || i1 == input.Length - 1)
            return input; //so Handle Inline resolves quickly when no inline is present
        //since we have at least 1 '$', we split
        string[] s_split = input.Split('$');
        if (s_split.Length % 2 == 0)
            return input; //means an invalid '$' pattern occurred, in all use cases, split should be odd
        //s_split[0] and s_split[s_split.Length-1] will be eventually concat, but otherwise ignored
        //if a non-edge space is the string "", that means $$ occurred, and that is replaced w/$ [just one]
        int empty_count = 0;
        for(int i = 1; i < s_split.Length - 1; i++)
        {
            if (s_split[i] == "")
                empty_count++;
        }
        string[] s_bucket = new string[s_split.Length - empty_count];
        s_bucket[0] = s_split[0];
        bool place_left_b = true;
        for(int sp_i = 1, bu_i = 1; bu_i < s_bucket.Length; sp_i++, bu_i++)
        {
            if (s_split[sp_i] == "") //this is when $$ is typed for intentional '$' printing
            {
                if (place_left_b)
                    s_bucket[bu_i] = string.Concat("$", s_split[sp_i + 1]);
                else //if still in bracket clause (so : -> = must occur)
                    s_bucket[bu_i] = string.Concat("$", s_split[sp_i + 1].Replace(':', '='));
                sp_i++; 
            }
            else if(place_left_b)
            {
                s_bucket[bu_i] = string.Concat("<", s_split[sp_i].Replace(':','='));
                place_left_b = false;
            }
            else 
            {
                s_bucket[bu_i] = string.Concat(">", s_split[sp_i].Replace(':', '='));
                place_left_b = true;
            }
        }
        return string.Concat(s_bucket);
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
            string postColon = tag.Substring(colonIndex + 1).Trim();
            switch (preColon) //this could always be ToUpper-ed to allow for case insensitivity
            {
                case "speaker":
                    HandleSpeakerTag(postColon);
                    break;
                case "sprite": //this intentionally doesn't break and falls to the lSprite case
                case "lSprite":
                    HandleSpriteTag(postColon, true);
                    break;
                case "rSprite":
                    HandleSpriteTag(postColon, false);
                    break;
                case "audio": //not implemented!
                    HandleAudioTag(postColon);
                    break;
                case "READ_AS_STAGE_LINES":
                    if (postColon.ToUpper().Equals("TRUE"))
                        readAsStageLines = true;
                    else if (postColon.ToUpper().Equals("FALSE"))
                        readAsStageLines = false;
                    else
                        throw new System.ArgumentException("Screwed up READ_AS_STAGE_LINES tag");
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
        {
            if (tag.Equals("NONE"))
                currLeftSprite = null;
            else
                currLeftSprite = Resources.Load<Sprite>(tag);
        }
        else
        {
            if (tag.Equals("NONE"))
                currRightSprite = null;
            else
                currRightSprite = Resources.Load<Sprite>(tag);
        }
    }

    private void HandleAudioTag(string tag)
    {
        throw new System.NotImplementedException("HandleAudioTag isn't implemented yet");
    }



    /* returns 1 on finding a tag and not getting blocked 
     * returns 0 on not finding a tag
     * returns -1 on getting blocked
     */
    private int CheckDivertBlockTags(string tag)
    {
        int result = 0;
        int colonIndex = tag.IndexOf(':');
        if (colonIndex != -1)
        {
            string preColon = tag.Substring(0, colonIndex).ToUpper(); //doesn't include the colon
            string postColon = tag.Substring(colonIndex + 1).Trim();
            switch (preColon) //this could always be ToUpper-ed to allow for case insensitivity
            {
                case "BLOCK_IF_TRUE":
                    if (StateManager.DCManager.GetInkVar<bool>(postColon))
                        result = -1;//blocked by tag
                    else
                        result = 1; // found tag, but not blocked
                    break;
                case "BLOCK_IF_FALSE":
                    if (!StateManager.DCManager.GetInkVar<bool>(postColon))
                        result = -1; //blocked by tag
                    else
                        result = 1; // found tag, but not blocked
                    break;
                default:
                    break; //did not encounter blocker
            }
        }
        return result;
    }



}
