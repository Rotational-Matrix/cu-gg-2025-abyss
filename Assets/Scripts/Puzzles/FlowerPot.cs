using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class FlowerPot : MonoBehaviour
{
    public enum SpriteState
    {
        Empty = 0,
        AlmostFull = 1,
        Full = 2
    };
    
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite almostFullSprite;
    [SerializeField] private Sprite fullSprite;
    private SpriteRenderer sr; // will instantly set to

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetSprite(SpriteState state)
    {
        switch (state)
        {
            case SpriteState.Empty:
                sr.sprite = emptySprite;
                break;
            case SpriteState.AlmostFull:
                sr.sprite = almostFullSprite;
                break;
            case SpriteState.Full:
                sr.sprite = fullSprite;
                break;
            default:
                break; //ignores invalid calls
        }
    }

    
    // should hold the flowerpot sprite and know how to swap it
    // flower pot requires all assoc sprites {empty, all but one, and 'full'}
    

}
