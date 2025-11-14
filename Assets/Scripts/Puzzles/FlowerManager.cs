using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FlowerManager : MonoBehaviour
{
    public static event Action<FlowerManager> OnFlowerPuzzleEnd;
    public int flowerCount = 0;
    public int flowersNeeded;
    public void AddFlower()
    {
        flowerCount++;
        Debug.Log(flowerCount + " flowers gotten");
        if (flowerCount == flowersNeeded)
        {
            Debug.Log("Flower requirement met.");
            if (OnFlowerPuzzleEnd != null)
            {
                OnFlowerPuzzleEnd(this);
            }
            //[Cu] assume always talk to Sariel 
            
        }
    }

}
