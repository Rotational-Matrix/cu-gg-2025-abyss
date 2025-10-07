using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProtoInputHandler : MonoBehaviour
{
    [SerializeField] private GameObject dialogueCanvas;

    private DialogueCanvasManager dcManager;

    void Awake()
    {
        dcManager = dialogueCanvas.GetComponent<DialogueCanvasManager>();
    }

    // Update is called once per frame
    //DELETE THIS, it is merely a dummy event handler that has been made merely for 'proto'
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            dcManager.DecremChoiceSelection();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            dcManager.IncremChoiceSelection();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!dcManager.AttemptContinue())
            {
                dcManager.InitiateChoices();
            }
        }
    }
}
