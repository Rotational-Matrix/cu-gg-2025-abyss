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
    [SerializeField] TMPro.TMP_Text ChoiceText; // The actual displayed text in the text box
    [SerializeField] GameObject SelectorImage; // should be a child of the Choice box panel
    //private string internalString = ""; // maybe needed for later to parse commands
    private bool isSelected = false;
    //private isSelectable // only to be added in case of special plot circumstance

    public void SetSelected(bool setSelected)
    {
        this.isSelected = setSelected;
        SelectorImage.SetActive(isSelected);
    }
    public bool IsSelected()
    {
        return isSelected;
    }

    public void SetTextContents(string newContents)
    {
        ChoiceText.text = newContents;
    }

    public void CleanTextContents()
    {
        ChoiceText.text = "";
    }

    private void OnDisable()
    {
        this.CleanTextContents();
        this.SetSelected(false);
    }



}
