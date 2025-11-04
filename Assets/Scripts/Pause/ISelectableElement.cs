using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectableElement
{
    /* ISelectable is the interface for anything that can be put in a list and selected.
     *  - NOTE: this does not apply to choiceBoxes because they were made before this and I'm lazy
     */


    /* SetSelected should control the 'highlight' the item (i.e. should have some indicator hovering over it)
     *  - the bool param setSelected should indicate whether the object is being selected or unselected.
     *  - implementing objects don't necessarily need to know if they are selected besides displaying that they are
     */
    void SetSelected(bool setSelected);

    /* Choose()
     *  - triggers the response that the item has for being chosen
     *  - returns a bool indicating its success (some items may be conditionally 'unchoosable')
     */
    bool Choose();

    /* SetVisible(bool setVisible)
     *  - depending on setVisible, visually activates the item, 
     *  - will most likely call SetActive(setPresent) on the gameObject it describes
     *      - object expected to 'clean' itself on Awake() and on Disable()
     *          - cleaning supposes turning off the selector image as well as any temp effects 
     */
    void SetVisible(bool setVisible);




}
