using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class DialoguePanelHandler : MonoBehaviour
{
    // The following fields are expected to be children of the dialogue panel (or attached to children)
    [SerializeField] private GameObject leftSprite; 
    [SerializeField] private GameObject rightSprite; 
    [SerializeField] private TMPro.TMP_Text bodyText;
    [SerializeField] private GameObject headerPanel;
    [SerializeField] private TMPro.TMP_Text headerText;
    private Color32 defaultTextColour = new (255, 255, 255, 255);
    private Color32 greyTextColour = new (180, 180, 180, 255);
    //private bool isTextCrawlOn = true; //not implemented, I can implement this later, but not for the proto.
    //if text crawl is added, will have to hold an intermediate string.


    // there could also be an sprite to appear to indicate the need to hit the enter button

    //there will have to be a heft 'on disable' call

    //should be done just prior to enabling the DPanel
    public void ProgressDialogue(string bodyText, string headerText, Sprite lSprite, Sprite rSprite)
    {
        SetBodyText(bodyText);
        SetHeaderText(headerText);
        SetSprite(lSprite, rSprite);
    }

    //overloaded version for updating only one sprite (will happen a lot)
    public void ProgressDialogue(string bodyText, string headerText, Sprite sprite, bool isLeft)
    {
        SetBodyText(bodyText);
        SetHeaderText(headerText);
        SetSprite(sprite, isLeft);
    }

    //overloaded version in case one wants to ignore the Sprites
    public void ProgressDialogue(string bodyText, string headerText)
    {
        SetBodyText(bodyText);
        SetHeaderText(headerText);
    }
    

    public void SetBodyText(string bodyText)
    {
        this.bodyText.text = bodyText;
    }
    public void appendLineToBodyText(string appendText)
    {
        this.bodyText.text += "\n" + appendText; //I don't know how much use this'll be.
    }
    public string GetBodyText() //I don't know why this would be used, but I may as well make a getter
    {
        return this.bodyText.text;
    }

    public void SetSprite(Sprite sprite, bool isLeft)
    {
        if (sprite != null)
        {
            if (isLeft)
            {
                leftSprite.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                leftSprite.SetActive(true);
            }
            else
            {
                rightSprite.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                rightSprite.SetActive(true);
            }
        }
        else //if the sprite is null
        {
            if (isLeft)
                leftSprite.SetActive(false);
            else
                rightSprite.SetActive(false);
        }
    }

    //overloaded version to do both at once
    public void SetSprite(Sprite lSprite, Sprite rSprite)
    {
        SetSprite(lSprite, true);
        SetSprite(rSprite, false);
    }

    //SetHeaderText(headerText) is also responsible for the active state of the header panel
    //  - receiving "" or "NO_SPEAKER" will disable the header panel
    //  - receiving anything else will activate the header panel and set the headerText

    public void SetHeaderText(string headerText)
    {
        this.headerText.text = headerText;
        if (headerText.Equals("") || headerText.Equals("NO_SPEAKER"))
        {
            this.headerText.text = "";
            this.headerPanel.SetActive(false);
        }
        else
        {
            this.headerText.text = headerText;
            this.headerPanel.SetActive(true);
        }
    }

    //for convenience, specifically restores default on passing false as param
    public void GreyOutText(bool becomeGrey)
    {
        if (becomeGrey)
            bodyText.color = greyTextColour;
        else
            bodyText.color = defaultTextColour;
    }


    private void CleanDPanel()
    {
        //This is done so that, once DPanel is re-enabled, its sprites are not shown unless they should be.
        SetSprite(null, null); 
        SetBodyText("");
        SetHeaderText("");
        bodyText.color = defaultTextColour;
    }




    private void OnDisable()
    {
        CleanDPanel();
    }

}
