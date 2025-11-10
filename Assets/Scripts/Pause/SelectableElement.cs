using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PLS RENAME FILE
public abstract class SelectableElement : MonoBehaviour//, ISelectableElement
{
    //I HAVE SOMEWHAT BLUNDERED
    //I NEED TO MAKE THIS INTO AN ABSTRACt CLASS THAT EVERYTHING INHERITS FROM BASICALLY IN PLACE OF THE INTERFACE
    //SINCE UNITY CANNOT SERILIZE LISTS OF IMPLEMENTING OBJECTS, BUT CAN SERIALIZE INHERITING ONES

    public abstract void SetSelected(bool setSelected);

    /* Choose()
     *  - triggers the response that the item has for being chosen
     *  - returns a bool indicating its success (some items may be conditionally 'unchoosable')
     */
    public abstract bool Choose();

    /* SetVisible(bool setVisible)
     *  - depending on setVisible, visually activates the item, 
     *  - will most likely call SetActive(setPresent) on the gameObject it describes
     *      - object expected to 'clean' itself on Awake() and on Disable()
     *          - cleaning supposes turning off the selector image as well as any temp effects 
     */
    public abstract void SetVisible(bool setVisible);
}
