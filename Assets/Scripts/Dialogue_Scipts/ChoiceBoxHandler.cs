using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This code should handle all the components of 'choicebox'
/// 
/// Right now, intended to handle 
///     - the choice 'string'
///     - whether it is selected (and the visuals for this)
///     
/// Also as of now, built for up/down selection of choices, NOT MOUSE_CLICK
///     this may be easily rectified should mouse_click be desired.
/// </summary>

public class ChoiceBoxHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text choiceText; // The actual displayed text in the text box
    [SerializeField] private GameObject selectorImage; // should be a child of the Choice box panel
    //private string internalString = ""; // maybe needed for later to parse commands
    private bool isSelected = false;
    private bool isLocked = false; // for special plot circumstances, locked can be viewed, not chosen

    public void SetSelected(bool setSelected)
    {
        this.isSelected = setSelected;
        selectorImage.SetActive(isSelected);
    }
    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetTextContents(string newContents)
    {
        choiceText.text = newContents;
    }

    public void SetLocked(bool setLocked)
    {
        isLocked = setLocked;
    }
    public bool IsLocked()
    {
        return isLocked;
    }

    private void CleanTextContents()
    {
        choiceText.text = "";
    }

    private void OnDisable()
    {
        this.CleanTextContents();
        this.SetSelected(false);
    }

    private void Awake()
    {
        this.CleanTextContents();
        this.SetSelected(false);
    }


}
