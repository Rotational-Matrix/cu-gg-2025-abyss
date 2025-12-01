using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private static List<ForcedMove> forcedMoves = new List<ForcedMove>();
    private static bool updateForcedMove = false;

    

    public void StartForcedMove(GameObject objToMove, Vector3 targetPosition, float distPortion, float moveSpeed)
    {   
        if (!InForcedMove(objToMove))
        {
            ForcedMove forcedMove = new ForcedMove(objToMove, targetPosition, distPortion, moveSpeed);
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
            foreach (ForcedMove fMove in forcedMoves)
            {
                bool continuing = fMove.IncrementalMove();
                if (!continuing)
                    EndForcedMove(fMove);
            }
        }    
    }


    

    //notably not static
    public void OnStateChange(bool inMenu, bool inDialogue, int stateFlag)
    {
        updateForcedMove = !inMenu; //no input from stateFlag here, since typically this sets it as ForcedMove
    }


    private void Awake()
    {
        StateManager.AddStateChangeResponse(this);
    }

    //for a given forced move,



    private int IndexForcedMove(GameObject gameObject)
    {
        int i = 0;
        foreach(ForcedMove fMove in forcedMoves)
        {
            if(object.Equals(fMove.Identifier(), gameObject))
                    return i;
            i++;
        }
        return -1;
    }
   


    private void EndForcedMove(ForcedMove forcedMove)
    {
        forcedMove.EndForcedMove();
        forcedMoves.RemoveAt(IndexForcedMove(gameObject));
    }


    //RoamCmr has an update call to progress its forced moves

    private class ForcedMove : object
    {
        private Transform objectToMove; //This is the transform that will be updated
        private Vector3 targetPosition;
        private Vector3 direction = Vector3.zero;
        private float moveSpeed = 3f;

        public ForcedMove(GameObject objectToMove, Vector3 targetPosition, float distPortion, float moveSpeed)
        {
            this.objectToMove = objectToMove.transform;
            this.moveSpeed = moveSpeed;
            this.direction = (targetPosition - this.objectToMove.position).normalized;
            this.targetPosition = this.objectToMove.position +
                distPortion * Vector3.Distance(this.objectToMove.position, targetPosition) * direction;

            if (objectToMove.TryGetComponent<NPCAnimationManager>(out NPCAnimationManager outNPCAM) )
            {
                outNPCAM.SetDirection(this.direction);
            }
            else if (objectToMove.TryGetComponent<PlayerAnimationManager>(out PlayerAnimationManager outPAM))
            {
                outPAM.DeclareInForcedMove(true, this.direction);
                StateManager.SetPlayerForcedMoveStatus(true); //FIXXX THIS WAS LAST THING DONE MAKE ON EXIT/END
                
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
            if (Vector3.Distance(objectToMove.position, targetPosition) < 0.01f)
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


}
