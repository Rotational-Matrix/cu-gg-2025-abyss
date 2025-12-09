using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class RoamCmdr : MonoBehaviour, IStateManagerListener
{
    /* RoamCmdr (I cannot be bothered to type RoamCommander)
     * 
     * the goal of this is to be able to do the following:
     *      - allow for usage of the MOVE command
     *          - needs access to eve & sariel GameObjects (see DemoMotion for 'b-line' code)
     *              - perchance change b-line?
     *      - allow for forced teleportation (not necessarily trigger focused)
     */

    // May have this hold the LeashManager and leash
    [SerializeField] private LeashManager leashManager;
    [SerializeField] private GameObject leash;

    [SerializeField] private UnityEngine.UI.Image blackBackdrop;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float flatCloseEnoughRadius = 0.1f;

    [Header("Last State Broadcast (inMenu, inDialogue, stateFlag)")]
    [SerializeField] private string MostRecentStateBroadcast = "";

    private float defaultInertia;// = 1f;     //
    private float defaultDamping;// = 0.05f;  // all stored in anticipation of slack function
    private float defaultStrength;// = 1f;    // (will actually set default in awake to whatev stored in leashManager)
    private float defaultMaxDist;// = 2f;     //

    private static List<ForcedMove> forcedMoves = new List<ForcedMove>();
    private static bool updateForcedMove = false; // stolen to mean 'in non menustate'
    private static bool killForcedMovesAtUpdate = false; //

    private static bool backdropFading = false;

    private readonly static Dictionary<string,Vector3> locDict = new Dictionary<string,Vector3>();
    

    public static Vector3 ParseMapLocation(string str) //presume the string is all caps, or make it case insensitive!
    {
        return locDict[str]; //make sure the dictionary is set prior to calling this, dimwit
    }

    private void InitLocationDict() //call at awake or earlier (can this be set in the field?)
    {
        //realistically, the y component is almost certainly going to get ignored
        locDict.Add("ANIMAL_AREA",          new Vector3(3, 0, 0));
        locDict.Add("CAVE_ENTRANCE",        new Vector3(0, 0, 0));
        locDict.Add("CAVE_INTERIOR",        new Vector3(0, 0, 3));
        locDict.Add("APPROACHING_KNAVES",   new Vector3(2, 0, 1));
        locDict.Add("FLOWER_AREA_ENTRANCE", new Vector3(2, 0, 2));
        locDict.Add("FLOWER_AREA_SARIEL",   new Vector3(1, 0, 2)); // sariel's location in the flower area puzzle
        locDict.Add("KNAVE_MUSH1",          new Vector3(0, 0, -1)); //
        locDict.Add("KNAVE_MUSH2",          new Vector3(0, 0, -2)); // only needed bc Eve and Sar walk towards them
        locDict.Add("KNAVE_MUSH3",          new Vector3(0, 0, -3)); // (don't need to set, I'll get RCmdr transforms)
        //locDict.Add("LAMB_SPRITE", )


    }

    public void SetLeashActive(bool value) //make leash slackener
    {
        leashManager.SetLeashActive(value);
        leash.SetActive(value);
    }
    public void RespondToLoadSave() //called after the new inkfile is present
    {
        bool uninformedInkFile = StateManager.DCManager.GetInkVar<float>("leashMaxDist") <= 0;
        ClearForcedMoves();
        if (uninformedInkFile) //i.e. inkfile leash coefs haven't been set
            WriteInkLeashCoef();
        else
            ReadInkLeashCoef();
        SetLeashActive(StateManager.DCManager.GetInkVar<bool>("leashActive"));

    }

    public void ReadInkLeashCoef() // read from inkfile to leashManager
    {
        SetLeashCoef(
            StateManager.DCManager.GetInkVar<float>("leashInertia"),
            StateManager.DCManager.GetInkVar<float>("leashDamping"),
            StateManager.DCManager.GetInkVar<float>("leashStrength"),
            StateManager.DCManager.GetInkVar<float>("leashMaxDist")
            );
    }
    public void WriteInkLeashCoef() // write from inkfile to leashMananger
    {
        StateManager.DCManager.SetInkVar<float>("leashInertia", leashManager.inertia);
        StateManager.DCManager.SetInkVar<float>("leashDamping", leashManager.damping);
        StateManager.DCManager.SetInkVar<float>("leashStrength", leashManager.strength);
        StateManager.DCManager.SetInkVar<float>("leashMaxDist", leashManager.maxDist);
    }
    public void SetLeashCoef(float inertia, float damping, float strength, float maxDist)
    {
        leashManager.inertia = inertia;
        leashManager.damping = damping;
        leashManager.strength = strength;
        leashManager.maxDist = maxDist;
    }
    public void SetLeashCoef()
    {
        SetLeashCoef(defaultInertia, defaultDamping, defaultStrength, defaultMaxDist);
    }

    public void StartForcedMove(GameObject objToMove, Vector3 targetPosition, bool isProp, 
        float distPortion, float spdFactor)
    {   
        if (!InForcedMove(objToMove))
        {
            ForcedMove forcedMove = new ForcedMove(objToMove, targetPosition, isProp, distPortion,
                this.moveSpeed * spdFactor, this.flatCloseEnoughRadius, true);
            forcedMoves.Add(forcedMove);
        }
    }
    public bool InForcedMove(GameObject gameObject)
    {
        return IndexForcedMove(gameObject) != -1;
    }

    private void Update()
    {
        if (updateForcedMove)
        {
            int fMove_origCount = forcedMoves.Count;
            for (int i = forcedMoves.Count - 1; i >= 0; i--) //decrem to account for deletion without issue
            {
                bool continuing = forcedMoves[i].IncrementalMove();
                if (!continuing)
                    EndForcedMove(forcedMoves[i]);
            }
            if (killForcedMovesAtUpdate) //ensures killing forced moves doesn't cause an error w/update
            {
                while (forcedMoves.Count > 0)
                    EndForcedMove(forcedMoves[0]);
                killForcedMovesAtUpdate = false; 
            }
            if (backdropFading)
            {
                AlphaDecrement(0.01f);
            }    

        }

    }

    public void SetBackdropActive(bool value)
    {
        if(backdropFading)
        {
            if (value) //setting active while fading stops the fading
            {
                backdropFading = false;
                Color clr = blackBackdrop.color;
                blackBackdrop.color = new Color(clr.r, clr.g, clr.b, 1);
            }
            // else setting inactive while fading just lets it keep fading
        }
        else
        {
            if (value)
                blackBackdrop.gameObject.SetActive(true);
            else
                backdropFading = true;
        }
    }
    private void AlphaDecrement(float alphaDecrementUnit)
    {
        //float currAlpha = blackBackdrop.color.a;
        //blackBackdrop.color = currAlpha;
        Color clr = blackBackdrop.color;
        float alpha = clr.a - alphaDecrementUnit;
        if (alpha > 0)
        {
            blackBackdrop.color = new Color(clr.r, clr.g, clr.b, alpha);
        }
        else
        {
            blackBackdrop.gameObject.SetActive(false);
            blackBackdrop.color = new Color(clr.r, clr.g, clr.b, 1);
            backdropFading = false;
        }
    }
    
    

    //notably not static
    public void OnStateChange(bool inMenu, bool inDialogue, int stateFlag)
    {
        updateForcedMove = !inMenu; //no input from stateFlag here, since typically this sets it as ForcedMove
        FormatStateChangeForInspector(inMenu, inDialogue, stateFlag); //literally just for inspector
    }


    private void Awake()
    {
        StateManager.AddStateChangeResponse(this);
        InitLocationDict();
        
        SetLeashActive(false);
        defaultInertia = leashManager.inertia;
        defaultDamping = leashManager.damping;
        defaultStrength = leashManager.strength;
        defaultMaxDist = leashManager.maxDist;
        //WriteInkLeashCoef(); //will probably cause error bc RCommander is above DCM in hierarchy
    }

    //for a given forced move,



    private int IndexForcedMove(GameObject gameObject)
    {
        for (int i = forcedMoves.Count - 1;  i >= 0; i--)
        {
            if (object.Equals(forcedMoves[i].Identifier(), gameObject))
                    return i;
        }
        return -1;
    }
   


    private void EndForcedMove(ForcedMove forcedMove)
    {
        GameObject identifier = forcedMove.Identifier();
        forcedMoves.RemoveAt(IndexForcedMove(identifier));
        forcedMove.EndForcedMove(); // must be AFTER it is removed from the list 
        // (ordering bc externally fMove only seems 'done' when not in list)
    }
    private void ClearForcedMoves()
    {
        killForcedMovesAtUpdate = true; //sets up all forced moves to be cleared out
    }

    private class ForcedMove : object
    {
        private Transform objectToMove; //This is the transform that will be updated
        private Vector3 targetPosition;
        private Vector3 direction = Vector3.zero;
        private float moveSpeed = 3f;
        private float flatCloseEnoughRadius;

        //note that, if isProp, dist is from 0 to 1 as a proportion ofthe course.
        //           else, dist is flat distance
        public ForcedMove(GameObject objectToMove, Vector3 targetPosition, bool isDistProportional, 
            float dist, float moveSpeed, float flatCloseEnoughRadius, bool ignoreY)
        {
            // this is gross, but it is because Eve base y_pos != sariel base y_pos
            // meaning not only will flying probably occur, but eve will attempt to phase into ground
            // currently the ground forces eve to approx 0.63 if she tries to go low enough
            // sariel is stable at 0, so eve cannot even get close
            if(ignoreY)
                targetPosition.y = objectToMove.transform.position.y;

            this.objectToMove = objectToMove.transform;
            this.moveSpeed = moveSpeed;
            this.flatCloseEnoughRadius = flatCloseEnoughRadius;
            this.direction = (targetPosition - this.objectToMove.position).normalized;
            if (isDistProportional)
                this.targetPosition = this.objectToMove.position +
                    dist * Vector3.Distance(this.objectToMove.position, targetPosition) * direction;
            else
                this.targetPosition = this.objectToMove.position + dist * direction;

            if (objectToMove.TryGetComponent<NPCAnimationManager>(out NPCAnimationManager outNPCAM))
            {
                outNPCAM.SetDirection(this.direction);
            }
            else if (objectToMove.TryGetComponent<PlayerAnimationManager>(out PlayerAnimationManager outPAM))
            {
                outPAM.DeclareInForcedMove(true, this.direction);
                StateManager.SetPlayerForcedMoveStatus(true);

            }
                
        }

        //returns true whilst it is still going.
        public bool IncrementalMove()
        {
            objectToMove.position = Vector3.MoveTowards(
                objectToMove.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            direction = (targetPosition - objectToMove.position).normalized;
            if (Vector3.Distance(objectToMove.position, targetPosition) < flatCloseEnoughRadius)
            {
                direction = Vector3.zero;
                return false;
            }
            else
                return true;
        }
        
        public GameObject Identifier()
        {
            return objectToMove.gameObject;
        }
        public void EndForcedMove()
        {
            if (objectToMove.TryGetComponent<PlayerAnimationManager>(out PlayerAnimationManager outPAM))
            {
                outPAM.DeclareInForcedMove(false, this.direction);
                StateManager.SetPlayerForcedMoveStatus(false);
            }
        }

    }



    private void FormatStateChangeForInspector(bool inMenu, bool inDialogue, int stateFlag)
    {
        string inMenuStr = inMenu ? "true" : "false";
        string inDialogueStr = inDialogue ? "true" : "false";
        MostRecentStateBroadcast = "(" + inMenuStr + ", " + inDialogueStr + ", " + stateFlag.ToString() + ")";
    }

}
