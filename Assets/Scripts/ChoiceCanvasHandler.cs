using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;


/* ChoiceCanvasHandler should be able to handle selection of choiceboxes
 *  - choiceboxes should have their own handler, which this will communicate w/
 * 
 * ChoiceCanvasHandler will allow for the following processes
 *  - navigating between available Choiceboxes (currently assumed to be via keys, not mouse)
 *  - enabling and disabling its children (callable by fn)
 *  - selecting a choice and then giving the information
 *  - passing string information to its choice boxes
 *  
 * Per the prototype, hardcoded to have 4 options.
 *  - Some choices such as the choice_boxes being placed as a list are to help altering this.
 * 
 */
public class ChoiceCanvas : MonoBehaviour
{
    [SerializeField] private List<GameObject> choiceBoxes = new List<GameObject>();
    //[SerializeField] private GameObject perish;

    private int currentChoiceSelected = -1;
    private int choicesAvailable = 0;
    private readonly int MAX_POSS_CHOICES = 4;

    //InitiateChoices is how choice options are set up, and it returns a bool indicating its success
    public bool InitiateChoices(string[] choiceArr)
    {
        if(choiceArr.Length > MAX_POSS_CHOICES || choiceArr.Length == 0 || 
            choicesAvailable != 0)
        {
            //if faulty array is passed, or another choice is already active:
            return false;
        }

        choicesAvailable = choiceArr.Length;
        currentChoiceSelected = 0;

        for(int i = 0; i < choiceArr.Length; i++)
        {
            choiceBoxes[i].SetActive(true);
            ChoiceBoxHandler cBoxHandler = choiceBoxes[i].GetComponent<ChoiceBoxHandler>();
            cBoxHandler.SetTextContents(choiceArr[i]);
            if (i == 0) //set the first option selected by default
                cBoxHandler.SetSelected(true);
        }

        return true; // to indicate success
    }

    //attempts to Increment the choice selection. This likely corresponds to 'moving down'
    public void IncremChoiceSelection()
    {
        if(currentChoiceSelected < choicesAvailable - 1)
        {
            choiceBoxes[currentChoiceSelected].GetComponent<ChoiceBoxHandler>().SetSelected(false);

            currentChoiceSelected++;
            choiceBoxes[currentChoiceSelected].GetComponent<ChoiceBoxHandler>().SetSelected(true);
        }
    }

    public void DecremChoiceSelection()
    {
        if (currentChoiceSelected > 0)
        {
            choiceBoxes[currentChoiceSelected].GetComponent<ChoiceBoxHandler>().SetSelected(false);

            currentChoiceSelected--;
            choiceBoxes[currentChoiceSelected].GetComponent<ChoiceBoxHandler>().SetSelected(true);
        }
    }

    public int Choose()
    {
        int retIndex = currentChoiceSelected;
        RemoveChoices();
        return retIndex; //this returns the 0-indexed verion of the index
    }

    private void RemoveChoices()
    {
        if(choicesAvailable != 0)
        {
            foreach(GameObject cBox in choiceBoxes)
            {
                cBox.SetActive(false);
            }
            choicesAvailable = 0;
            currentChoiceSelected = -1;
        }
    }




}
