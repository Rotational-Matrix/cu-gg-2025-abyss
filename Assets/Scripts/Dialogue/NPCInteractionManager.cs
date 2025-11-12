using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCInteractionManager : MonoBehaviour
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
}
