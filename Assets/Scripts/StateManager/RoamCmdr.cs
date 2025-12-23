using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class RoamCmdr : MonoBehaviour, IStateManagerListener
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private Landmarks landmarks;

    [SerializeField] private LeashManager leashManager;
    [SerializeField] private GameObject leash;

    [SerializeField] private UnityEngine.UI.Image blackBackdrop;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float flatCloseEnoughRadius = 0.1f;

    [Header("Puzzle Settings")]
    [SerializeField] private int totalFlowerNum = 10;
    [SerializeField] private Flower[] FlowerArray; //this includes all 10 (even if the 10th isn't shown)
    [SerializeField] private GameObject FlowerPot; //the flower pot will be goofy FIXXX

    [SerializeField] private CobwebTrigger cobweb;



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

    private static bool sarielDistTriggerActive = false;
    private static float sarielDistTriggerRho = -1;

    private readonly static Dictionary<string,Vector3> locDict = new Dictionary<string,Vector3>();
    
    public static void DeleteThisFunction() //FIXXX 
    {
        CuToDo.DebugLocations(locDict); // yeah I'm sorry
    }

    public static Vector3 ParseMapLocation(string str) //presume the string is all caps, or make it case insensitive!
    {
        return locDict[str]; //make sure the dictionary is set prior to calling this, dimwit
    }

    private void InitLocationDict() //call at awake or earlier (can this be set in the field?)
    {
        //realistically, the y component is almost certainly going to get ignored
        locDict.Add("ANIMAL_AREA",          new Vector3(3, 0, 0));
        locDict.Add("CAVE_ENTRANCE",        new Vector3(18, 0, -20)); // This one is actual!
        locDict.Add("CAVE_INTERIOR",        landmarks.Cave); // USE TRIGGER CYL 1 OR LANDMARK!!!!
        locDict.Add("APPROACHING_KNAVES",   new Vector3(2, 0, 1));
        locDict.Add("FLOWER_AREA_ENTRANCE", new Vector3(2, 0, 2));
        locDict.Add("FLOWER_AREA_SARIEL",   new Vector3(1, 0, 2)); // sariel's location in the flower area puzzle
        locDict.Add("DEMO_FLOWER",          landmarks.DemoFlower);
        locDict.Add("HIDDEN_FLOWER",        landmarks.HiddenFlower);
        locDict.Add("FLOWER_POT",           landmarks.FlowerPot);
        locDict.Add("KNAVE_MUSH1",          landmarks.Mush1); //
        locDict.Add("KNAVE_MUSH2",          landmarks.Mush2); // only needed bc Eve and Sar walk towards them
        locDict.Add("KNAVE_MUSH3",          landmarks.Mush3); // (don't need to set, RCmdr will get transforms)
        locDict.Add("LAMB",                 landmarks.Lamb);
        locDict.Add("COBWEB",               landmarks.Cobweb);


    }

    public void SetLeashActive(bool value) //make leash slackener
    {
        leashManager.SetLeashActive(value);
        leash.SetActive(value);
    }
    public void RespondToLoadSave() //called *after* the new inkfile is present
    {
        bool uninformedInkFile = StateManager.DCManager.GetInkVar<float>("leashMaxDist") <= 0;
        ClearForcedMoves();
        if (uninformedInkFile) //i.e. inkfile leash coefs haven't been set
            WriteInkLeashCoef();
        else
            ReadInkLeashCoef();
        SetLeashActive(StateManager.DCManager.GetInkVar<bool>("leashActive"));
        ReadFlowerCount(); // update Flower puzzle
        CreateCobweb();    // update Cobweb puzzle

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
    // distFrom, offestX, offsetZ overload
    public void StartForcedMove(GameObject objToMove, Vector3 targetPosition, float flatDistAway,
        float offsetX, float offsetZ, float spdFactor)
    {
        if (!InForcedMove(objToMove))
        {
            ForcedMove forcedMove = new ForcedMove(objToMove, targetPosition, flatDistAway, offsetX,
                offsetZ, this.moveSpeed * spdFactor, this.flatCloseEnoughRadius);
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
            if (sarielDistTriggerActive)
            {
                Vector3 ev3 = StateManager.Eve.transform.position;
                Vector3 sv3 = StateManager.Sariel.transform.position;
                ev3.y = sv3.y = 0; // to explicitly ignore y differences
                if (Vector3.Distance(ev3, sv3) > sarielDistTriggerRho - 0.05f)
                {
                    sarielDistTriggerActive = false; //sets the trigger off
                    StateManager.DCManager.InitiateDialogueState("next_scene_knot");
                }
            }

        }

    }

    public void SetSarielDistTrigger(bool isTurningOn, float rho)
    {
        sarielDistTriggerActive = false; //to prevent update triggering mid fn call and ruining everything
        sarielDistTriggerRho = rho;
        sarielDistTriggerActive = isTurningOn;
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
            BaseFMConstructor(objectToMove, targetPosition, isDistProportional, dist, moveSpeed,
                flatCloseEnoughRadius, ignoreY);
        }
        //overloads of constructor
        //constructor that allows flat dist from target and offset
        public ForcedMove(GameObject objectToMove, Vector3 targetPosition, float flatDistAway,
            float offsetX, float offsetZ, float moveSpeed, float flatCloseEnoughRadius)
        {
            Vector3 tempDirection = (targetPosition - this.objectToMove.position).normalized;
            Vector3 modifiedTargetPos = targetPosition - (flatDistAway * tempDirection);
            modifiedTargetPos.x += offsetX;
            modifiedTargetPos.z += offsetZ;
            BaseFMConstructor(objectToMove, modifiedTargetPos, true, 1, moveSpeed, flatCloseEnoughRadius, true);
        }
        private void BaseFMConstructor(GameObject objectToMove, Vector3 targetPosition, bool isDistProportional,
            float dist, float moveSpeed, float flatCloseEnoughRadius, bool ignoreY)
        {
            // this is gross, but it is because Eve base y_pos != sariel base y_pos
            // meaning not only will flying probably occur, but eve will attempt to phase into ground
            // currently the ground forces eve to approx 0.63 if she tries to go low enough
            // sariel is stable at 0, so eve cannot even get close
            if (ignoreY)
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
            else if (objectToMove.TryGetComponent<SarielController>(out SarielController outSC))
            {
                outSC.OnFinishedForcedMove();
            }
        }

    }

    // a shortcut, but a helpful one
    public void TPSariel(Vector3 targetPos) // automatically ignores y
    {
        float tempY = StateManager.Sariel.transform.position.y;
        StateManager.Sariel.transform.position = new(targetPos.x,tempY,targetPos.z);
    }
    public void TPEve(Vector3 targetPos)
    {
        float tempY = StateManager.Eve.transform.position.y;
        Vector3 v3 = new(targetPos.x, tempY, targetPos.z);
        StateManager.Eve.transform.position = v3;
        mainCamera.TryGetComponent<CameraFollow>(out CameraFollow outCF);
        if(!object.Equals(outCF,null))
        {
            outCF.FastMove(StateManager.Eve.gameObject);
        }
        spotLight.transform.position = v3;
    }

    private void FormatStateChangeForInspector(bool inMenu, bool inDialogue, int stateFlag)
    {
        string inMenuStr = inMenu ? "true" : "false";
        string inDialogueStr = inDialogue ? "true" : "false";
        MostRecentStateBroadcast = "(" + inMenuStr + ", " + inDialogueStr + ", " + stateFlag.ToString() + ")";
    }


    // vvvvv begin puzzle section vvvvv

    public bool FlowersPickable()
    {
        return StateManager.DCManager.GetInkVar<bool>("flower_puzzle_start");
    }
    public bool CaveTransitionActive()
    {
        return StateManager.DCManager.GetInkVar<bool>("cave_transition_allowed");
    }
    public bool CobwebCanBeTaken()
    {
        return StateManager.DCManager.GetInkVar<bool>("cobweb_puzzle_start");
    }

    private void CreateCobweb()
    {
        bool cobExists = !StateManager.DCManager.GetInkVar<bool>("cobweb_puzzle_ended"); //spawns if not taken
        cobweb.SetCobwebActive(cobExists);
    }

    public void ReactToCobweb()
    {
        //needs tro call ink dialogue and perish
        //StateManager.DCManager.SetInkVar<bool>("cobweb_puzzle_ended", true) //set in ink
        StateManager.DCManager.InitiateDialogueState("part_II.post_cave"); //stitch played after cobweb
    }
    
    public void IncremFlowerCount() 
    {
        int newFlowerCount = StateManager.DCManager.GetInkVar<int>("flowerCounter") + 1;
        StateManager.DCManager.SetInkVar<int>("flowerCounter", newFlowerCount); //incremFlowerCount

        if (newFlowerCount == totalFlowerNum - 1) //i.e. on last flower
        {
            StateManager.Sariel.SetSarielCanInteract(true); // allows for 'psych' transition
        }
    }

    // [Cu] made a mistake! 

    
    private void CreateFlowers(bool inPuzzle, bool flowersPickedUp)
    {
        /* Preconditions:
         * totalFlowerNum is expected to be 10 (but this behaves with reasonable numbers)
         *  - note that flowerCounter values yield the following params:
         *      0                   -> (false, false)
         *      0 < x < (tF# - 1)   -> (true,  false)
         *      totalFlowerNum - 1, -> (true,  true)
         *      totalFlowerNum      -> (false, true)
         *  - the FlowerArray ought to be full of totalFlowerCount number of flowers
         */
        // note that flowers are always already in the correct position
        // note that flowerCounter tracks collected flower number, not current flowers in scene...
        // remember that FlowerArray[0] is demsontration flower
        // and FlowerArray[totalFlowerNum - 1] is hidden flower
        FlowerArray[0].SetFlowerActive(!(inPuzzle || flowersPickedUp)); // demo flower
        SetHiddenFlowerActive(false); //'hidden flower' turned off
        for (int i = 1; i < totalFlowerNum - 1; i++)
        {
            FlowerArray[i].SetFlowerActive(!flowersPickedUp);
        }
        //FLOWER POT SHOULD BE 'FILLED' when FLOWERS PICKED UP AND NOT IN PUZZLE FIXXX
    }
    public void SetHiddenFlowerActive(bool value) //called as part of a DCM fn
    {
        FlowerArray[totalFlowerNum - 1].SetFlowerActive(value);
    }


    private void ReadFlowerCount()
    {
        int inkFlowerCounter = StateManager.DCManager.GetInkVar<int>("flowerCounter");
        bool flowersPickedUp = inkFlowerCounter >= totalFlowerNum - 1; 
        if (!flowersPickedUp)
        {
            // if someone somehow saves in the middle of the flower puzzle,
            // then I'm going to restart their progress upon loading that save
            inkFlowerCounter = 0;
            StateManager.DCManager.SetInkVar<int>("flowerCounter", inkFlowerCounter);
        }
        SetHiddenFlowerActive(false); //'hidden flower' turned off
        for (int i = 0; i < totalFlowerNum - 1; i++)
        {
            FlowerArray[i].SetFlowerActive(!flowersPickedUp);
        }
    }


}
