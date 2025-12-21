using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// This has been borne out of the time-crunch

// [Cu] would like to have access to all of Sariel's objects, and does not like attaching every little obj to her
// this will be accessible in StateManager, and exists only to support inevitable funcitons upon her
public class SarielController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject SarielObject;
    [SerializeField] public GameObject SpriteObject;
    [SerializeField] public GameObject LightObject;
    [SerializeField] public GameObject TriggerObject;

    public void OnFinishedForcedMove()
    {
        AlertToUpdateInteract(StateManager.DCManager.GetInkVar<bool>("sariel_can_interact"));
    }

    public void SetSarielCanInteract(bool value)
    {
        bool prevValue = StateManager.DCManager.GetInkVar<bool>("sariel_can_interact");
        if (prevValue != value)
        {
            StateManager.DCManager.SetInkVar<bool>("sariel_can_interact", value);
            AlertToUpdateInteract(value);
        }
    }

    private void AlertToUpdateInteract(bool value)
    {
        if (TriggerObject.TryGetComponent<NPCInteractionManager>(out NPCInteractionManager outNPCIM))
            outNPCIM.AlertToUpdateInteract(value);
        else
            Debug.Log("Failed to find Sariel's NPCInteractionManager. (was it moved from the trigger obj?)");
    }


}
