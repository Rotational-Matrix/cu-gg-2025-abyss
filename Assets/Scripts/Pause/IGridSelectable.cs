using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridSelectable
{
    /* IGridSelectable is the interface for anythong that selects through selectable elements
     *  - Each of these should communicate with objects that implement ISelectableElement
     *      - ChoiceCanvasHandler is an exception, it will implement IGridSelectable anyway
     *  - Generally, IGridSelectable is here so that the input handler can properly handle several layers of menus
     *      - the input handler will prolly just have a stack of menus.
     *  
     *  - Note that the functions of IGridSelectable are based off of ChoiceCanvasHandler's funtions because lazy
     */

    /* InitiateGrid()
     *  - Makes elements appear on screen (generally with topmost element pre-selected)
     *  - Pushes itself onto StateManager's menu stack
     */
    public void InitiateGrid();

    /* IncremElement() 
     *  - unselects the current element and selects the element that looks like it should be higher
     */
    public void IncremElement();


    /* DecremElement() 
     *  - unselects the current element and selects the element that looks like it should be lower
     */
    public void DecremElement();

    /* ChooseSelected()
     *  - should attempt to choose the selected element
     *      - returns false upon faliure (such as when an element is locked)
     *      - returns true upon success
     *  - After choosing, many menus may choose to Exit themselves
     */
    public bool ChooseSelected();

    /* ExitMenu()
     *  - should effectively tell all of the menuItems to clean themselves
     *  - called to exit the menu
     */
    public void ExitMenu();




}
