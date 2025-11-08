using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//DefaultGrid should probably be inherited from
public class DefaultGrid : MonoBehaviour, IGridSelectable
{
    //normally I deliberately serialize GameObjects and not components.
    //  In this instance,the components are being serialized (also note for inheritance it is protected)
    [SerializeField] protected List<ISelectableElement> selectableList = new List<ISelectableElement>();
    [SerializeField] protected GameObject menuPanel; //which in turn serializes the grid back.
    protected int selectedIndex = -1;

    /* ISelectableElement has the following methods:
     *  - void SetSelected(bool)
     *  - bool Choose()
     *  - void SetVisible(bool)
     */

    //NOTE: virtually all methods fail when the list is empty (why would it be empty though?)

    public void InitiateGrid()
    {
        foreach(ISelectableElement element in selectableList)
            element.SetVisible(true);
        selectedIndex = 0;
        if (selectableList.Count > 0)
            selectableList[selectedIndex].SetSelected(true);
        menuPanel.SetActive(true);
        StateManager.MenuStack.Push(this); //notably calls to push itself to MenuStack on init
    }

    /* IncremElement() 
     *  - unselects the current element and selects the element that looks like it should be higher
     */
    
    public void IncremElement() 
    {
        // remember increm here seeks to actually go upwards (unlike choiceCanvas)
        // hence, 'IncremElement()' DECREMENTS the index
        if (selectedIndex > 0)
        {
            selectableList[selectedIndex].SetSelected(false);
            selectableList[--selectedIndex].SetSelected(true);
        }
    }


    /* DecremElement() 
     *  - unselects the current element and selects the element that looks like it should be lower
     */
    public void DecremElement()
    {
        // remember increm here seeks to actually go upwards (unlike choiceCanvas)
        // hence, 'DecremElement()' INCREMENTS the index
        if ((selectedIndex != -1) && (selectedIndex < selectableList.Count - 1))
        {
            selectableList[selectedIndex].SetSelected(false);
            selectableList[++selectedIndex].SetSelected(true);
        }
    }

    /* ChooseSelected()
     *  - should attempt to choose the selected element
     *      - returns false upon faliure (such as when an element is locked)
     *      - returns true upon success
     *  - After choosing, many menus may choose to Exit themselves
     */
    public bool ChooseSelected()
    {
        return selectableList[selectedIndex].Choose();
    }

    /* ExitMenu()
     *  - should effectively tell all of the menuItems to clean themselves
     *  - called to exit the menu
     */
    public void ExitMenu()
    {
        if (selectableList.Count > 0)
            selectableList[selectedIndex].SetSelected(false);
        selectedIndex = -1;
        foreach (ISelectableElement element in selectableList)
            element.SetVisible(false);
        menuPanel.SetActive(false); //only place where menuPanel is called
        StateManager.MenuStack.Pop(); //notably calls to pop the MenuStack itself on exit
    }

    public virtual StateManager.MenuInputType InputType()
    {
        return StateManager.MenuInputType.SelectableGrid;
    }

    public ISelectableElement this[int index]
    {
        get
        {
            if (index >= 0 && index < selectableList.Count)
                return selectableList[index];
            throw new System.ArgumentOutOfRangeException();
        }
    }


    protected void Awake()
    {
        menuPanel.SetActive(false); //all menu Panels should start off unless otherwise specified
    }



}
