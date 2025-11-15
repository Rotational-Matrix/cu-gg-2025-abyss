using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))] //realistically, requires a trigger (i.e. Collider.IsTrigger = true)
public class NPCInteractionManager : InteractableElement
{
    public static event Action<NPCInteractionManager> npcDialogue;
    public GameObject player;
    public float distance = 3.5f;
    public int npcCode;
    private int cyclicalFrameCount;
    private bool wasInDialogue = false;
    // Start is called before the first frame update
    void Start()
    {
        cyclicalFrameCount = 0;
    }

    void LateUpdate()
    {
        cyclicalFrameCount++;
        if (cyclicalFrameCount == 10) cyclicalFrameCount = 0;
        if (cyclicalFrameCount == 0) wasInDialogue = StateManager.GetDialogueStatus();
        Vector3 pPos = player.transform.position;
        Vector3 npcPos = this.gameObject.transform.position;
        npcPos.y = 0f;
        Vector3 distance = pPos - npcPos;
        //Debug.Log("distance: " + distance);
        //later: make sure this doesn't fire if already in dialogue
        if (distance.magnitude < 3.5 && Input.GetKeyDown(KeyCode.Space) && !wasInDialogue)
        {
            if (npcDialogue != null) npcDialogue(this);
        }
        if (StateManager.GetDialogueStatus()) wasInDialogue = true;
    }
    /* not to be ultra goofy, but my plan is as follows:
     * 
     * Each NPC (or interactable thing, really) needs 2 things for dialogue interaction:
     *  - a trigger (similar to a collider, but just tracks if one is inside a volume, no contact needed)
     *  - a string to initiate dialogue
     * 
     * 
     * because I'm an arse, I am going to create a secondary version below to illustrate this (it 
     */

    //needs access to only npc's interactable trigger (the npc will prolly have a smaller, actual collider)
    //[SerializeField] private Collider interactTrigger; The trigger must be on the same object that 
    [SerializeField] private string knotName;

    public override void ExecuteInteract()
    {
        StateManager.DCManager.InitiateDialogueState(knotName);
    }
    //note that, by inheriting from InteractableElement, this inherits OnTriggerEnter and OnTriggerExit








}
