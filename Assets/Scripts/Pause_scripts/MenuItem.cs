using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    /* should be composed of 4 parts:
     *  - selector image
     *  - displayed text
     *  - (if applicable) displayed value
     *  - reaction
     */

    [SerializeField] private GameObject selectorImage;
    [SerializeField] private TMPro.TMP_Text itemTextObject;
    [SerializeField] private TMPro.TMP_Text itemValueTextObject;
    //reaction will very much need to be implemented

    private string itemText;
    private int itemValue;
    private StateManager.ConfigItem configItem;

    public void SetSelected(bool setSelected)
    {
        selectorImage.SetActive(setSelected);
    }

    public void SetDisplayedName(string newName)
    {
        itemText = newName;
    }



    /* note that this method isn't creating a component because, if we don't explicitly need to,
     * then we prolly chouldn;t, as creating components at runtime is a little laborius
     */
    public void InitMenuItem(string itemName, StateManager.ConfigItem configItem)
    {
        this.itemText = itemName;
        if(!StateManager.ConfigValues.TryGetValue(configItem, out this.itemValue))
        {
            throw new System.ArgumentException("StateManager.TryGetValue failed");
        }
        this.configItem = configItem;
    }

    public void React()
    {

    }
    
    // as stupid as it is, I can actually make all the methods in this class,
    // and then call them by switchcase based on StateManager.ConfigItem value


}
