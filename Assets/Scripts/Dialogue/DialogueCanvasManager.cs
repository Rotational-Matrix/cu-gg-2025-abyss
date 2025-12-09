//using Ink.Parsed;
using Ink.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows;

public class DialogueCanvasManager : MonoBehaviour
{
    /// <summary>
    /// [Cu]'s Documentation
    /// 
    /// Dec 7th note - DCM now stands for "using Dictionaries would've been Considerably More effective..."
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
    private List<SaveState> saveStates = new List<SaveState>(); // there are constantly 4, but I dond't want to harcode it

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

    private Dictionary<string, Func<string[], bool>> colonLineCommands = new();


    private void Awake()
    {
        InitInk(); // creates inkstory, and 
        dpHandler = dialoguePanel.GetComponent<DialoguePanelHandler>();
        ccHandler = choiceCanvas.GetComponent<ChoiceCanvasHandler>();
        SetDialogueState(false); //it automatically makes sure it is turned off at start.
    }

    // creates the inkstory whilst also properly informing the newgame save state and providing it with proper external vals
    private void InitInk()
    {
        _inkStory = new Story(inkAsset.text);
        StateManager.RCommander.WriteInkLeashCoef();
        SetInkCharPos(); // initialise eve and sariel position
        SaveState.InitNewGameSaveState(_inkStory);
        for (int i = 0; i < 3; i++)
            saveStates.Add(new SaveState()); // make save options
        colonLineCommands.Add("FORCED_MOVE", ForcedMoveCmd);
        colonLineCommands.Add("AUTOSAVE", AutosaveCmd);
        colonLineCommands.Add("LEASH_SET", LeashSetActiveCmd);
        //colonLineCommands.Add("LEASH_COEF", LeashSetCoefCmd); merged into LEASH_SET
        colonLineCommands.Add("BACKDROP_SET", BackdropSetCmd);
    }

    public void ResponseToLoadSave()
    {
        if (GetInkVar<bool>("is_start_save"))
            InitiateDialogueState("save_load_knot");
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

    public void CreateInkSaveState(int saveIndex)
    {
        if (saveIndex < 0)
        {
            SaveState.AutoSave(_inkStory);
        }
        else if (saveIndex < saveStates.Count)
        {
            saveStates[saveIndex].CopyFromAutoSave();
        }
        else
            throw new ArgumentOutOfRangeException("Gave too large of an index to CreateInkSaveState");
    }
    public void SetAsNewGame(int saveIndex)
    {
        if (saveIndex >= 0 && saveIndex < saveStates.Count) //i.e. valid call
            saveStates[saveIndex].CopyFromNewGame();
        else
            throw new ArgumentOutOfRangeException("Gave invalid argument to SetAsNewGame");
    }
    public bool RunInkSaveState(int saveIndex)
    {

        if (saveIndex < 0) //-1 corresponds ot autosave json...
        {
            return SaveState.LoadAutoSave(_inkStory);
        }
        else if (saveIndex < saveStates.Count)
        {
            return saveStates[saveIndex].Load(_inkStory);
        }
        else // somehow chose an index impossibly large
            return false;
    }

    public string SaveStateDescriptor(int index)
    {
        if (index >= 0 && index < saveStates.Count) //note that autoSave shouldn't be showing its descriptor
        {
            return saveStates[index].Descriptor();
        }
        else
            throw new ArgumentOutOfRangeException("screwed up SaveStateDescriptor call (did you try to use it on autoSave?)");
    }
    public bool CanLoadInkSaveState(int index)
    {
        //all considered 'empty' for all intents and purposes
        if (index < 0)
            return SaveState.CanLoadAutoSave();
        return index < saveStates.Count && saveStates[index].CanLoad();
    }

    public void SetInkVar<T>(string variableName, T newVal) //literally the name of the variable as it appears in the inkstory
    {
        _inkStory.variablesState[variableName] = newVal; //it...allows this (please make the type of newVal match the variable...)
    }
    public T GetInkVar<T>(string variableName)
    {
        return (T)(_inkStory.variablesState[variableName]); //forceful cast... please match types!
    }

    public void SetInkCharPos() //overload for setting both current positions
    { SetInkCharPos(true); SetInkCharPos(false); }
    public void SetInkCharPos(bool isEve) // overload for current position
    {
        Vector3 v3 = isEve ? StateManager.Eve.transform.position : StateManager.Sariel.transform.position;
        SetInkCharPos(isEve, v3);
    }
    public void SetInkCharPos(bool isEve, Vector3 v3) //I'd've used character dicts, but if bools work, they work
    {
        string charBase = isEve ? "eve_x" : "sariel_x"; // based on ink var names
        for (int i = 0; i < 3; i++)
            SetInkVar<float>(charBase + (i + 1).ToString(), v3[i]);
    }
    public Vector3 GetInkCharPos(bool isEve)
    {
        string charBase = (isEve) ? "eve_x" : "sariel_x"; // based on ink var names
        Vector3 v3 = new Vector3();
        for (int i = 0; i < 3; i++)
            v3[i] = GetInkVar<float>(charBase + (i + 1).ToString());
        return v3;
    }

    //##### only on DCM initiated diverting are KnotTags handled #####
    // returns true if divert is blocked
    private bool HandleKnotTags(string knotName) //or knotName.stitchName
    {
        List<string> knotTagList = _inkStory.TagsForContentAtPath(knotName);
        if (object.Equals(knotTagList, null) || knotTagList.Count == 0)
            return false; // quick exit in case the tagList either doesn't exist or is empty
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
        bool handled = true;
        switch (command)
        {
            case "STOP_DIALOGUE": //wipes dialogue panel
                SetDialogueState(false);
                break;
            case "START_DIALOGUE": //phony, done to allow knot tags to work
                break;
            default:
                handled = false;
                break;
        }
        if (!handled)
        {
            if(!LineColonCommands(command))
                Debug.Log("Line command was not recognised: " + command);
        }
    }

    //returns success at handling command
    private bool LineColonCommands(string command)
    {
        //protocol for these line commands are: COMMAND:ARG1,ARG2,ARG3,...,ARGN
        //all parts of any command are expected to be case insensitive
        int colonIndex = command.IndexOf(':'); // should != -1
        if (colonIndex == -1 || command.IndexOf(':', colonIndex + 1) != -1)
            return false; // a quick check to guard against accidents. Can be altered if double colons are allowed

        //now time to generate argv
        char[] separators = new char[] { ':', ',' };
        string[] argv = command.ToUpper().Split(separators);
        for (int i = 0; i < argv.Length; i++) // I bet C programmers love seeing 'argv.Length'
            argv[i] = argv[i].Trim();
        try
        {
            return colonLineCommands[argv[0]](argv);
        }
        catch (KeyNotFoundException)
        {
            return false;
        }
    }
    
    private bool ForcedMoveCmd(string[] argv)
    {
        /* Expected args:
         * FORCED_MOVE: <character> <location> <isProportional> <distance> <speedFactor>
         */
        //StartForcedMove(GameObject objToMove, Vector3 targetPosition, bool isProp, float distPortion)
        GameObject character = CapsToCharacter(argv[1]);
        Vector3 location = CapsToLocation(argv[2]);
        bool isProportional = CapsToBool(argv[3]);
        float distance = float.Parse(argv[4]);
        float spdFactor = float.Parse(argv[5]);
        if (object.Equals(character, null) || object.Equals(location, null))
            return false;
        StateManager.RCommander.StartForcedMove(character, location, isProportional, distance, spdFactor);
        return false;
    }
    private bool AutosaveCmd(string[] argv)
    {
        /* Expected args:
         * AUTOSAVE: <isStartType>
         */
        if (string.Equals(argv[1], "START"))
        {
            SetInkCharPos(); //set all characters positions prior to 'start' save

            StateManager.Autosave();
            return true;
        }
        else
        {
            throw new NotImplementedException("Autosave non-start type not implemented");
        }
    }
    private bool LeashSetActiveCmd(string[] argv)
    {
        bool settingActive = CapsToBool(argv[1]);
        
        if (argv.Length == 2) //i.e. cmd:<active value>
        {
            if (settingActive) //meaning going from OFF to ON, (or just staying ON) 
            {
                StateManager.RCommander.ReadInkLeashCoef(); //so read while OFF
                StateManager.RCommander.SetLeashActive(settingActive);
            }
            else
            {
                StateManager.RCommander.SetLeashActive(settingActive);
                StateManager.RCommander.ReadInkLeashCoef(); //always read while OFF
            }
            return true;
        }
        else 
            return false;
    }
    private bool BackdropSetCmd(string[] argv)
    {
        if (argv.Length == 2) //i.e. cmd:<active value>
        {
            StateManager.RCommander.SetBackdropActive(CapsToBool(argv[1]));
            return true;
        }
        else
            return false;

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
        int colonIndex = input.IndexOf(':');
        if (colonIndex == -1)
        {
            HandleSpeakerTag("NO_SPEAKER");
            HandleSpriteTag("NONE", true);
            dpHandler.GreyOutText(true);
            return "<i>" + input + "</i>";
        }
        string preColon = input.Substring(0, colonIndex).Trim(); //doesn't include the colon
        string postColon = input.Substring(colonIndex + 1).Trim();
        //note that, if there exists a colon in the text, either a real speaker or NO_SPEAKER is expected
        HandleSpeakerTag(preColon);
        if (string.Equals(preColon,"NO_SPEAKER"))
        {
            postColon = "<i>" + postColon + "</i>";
            dpHandler.GreyOutText(true);
            HandleSpriteTag("NONE", true);
        }
        else
            dpHandler.GreyOutText(false); //reverts to default
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

    private bool CapsToBool(string str)
    {
        if (string.Equals(str, "TRUE"))
            return true;
        else if (string.Equals(str, "FALSE") || string.Equals(str, "NONE")) //allows NONE, but meant for FALSE
            return false;
        else
            throw new ArgumentException("CapsToBool received:" + str);
    }
    
    private GameObject CapsToCharacter(string str)
    {
        if (string.Equals(str, "EVE"))
            return StateManager.Eve.gameObject;
        else if (string.Equals(str, "SARIEL"))
            return StateManager.Sariel.gameObject;
        else if (string.Equals(str, "NONE"))
            return null;
        else
            throw new ArgumentException("CapsToCharacter received:" + str);
    }
    private Vector3 CapsToLocation(string str)
    {
        // in accordance with Forced_Move and Demo_Motion, transform.position is used!
        if (string.Equals(str, "EVE"))
            return StateManager.Eve.transform.position;
        else if (string.Equals(str, "SARIEL"))
            return StateManager.Sariel.transform.position;
        else if (str.IndexOf('#') == 0) //in case it is needed, #floatx#floaty#floatz is valid
        {
            string[] numStrs = str.Split('#', StringSplitOptions.RemoveEmptyEntries);
            float[] nums = new float[3]; //bc it is a vector 3
            for (int i = 0; i < 3; i++)
                nums[i] = float.Parse(numStrs[i]);
            return new Vector3(nums[0], nums[1], nums[2]);
        }
        return RoamCmdr.ParseMapLocation(str);
    }
    

}
