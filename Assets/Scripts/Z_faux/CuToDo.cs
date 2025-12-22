using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Well, well, well...
/// [Cu] created this during finals (on dec 9th, anyhow) in order to easily access a local TODO page!
/// 
/// This is stupid, but it works, please don't put actual funcitons here (debuggers/test-fns are okay)
///     and please don't delete this file until the game is done...
///     
/// 
/// ACTUAL TODO:
///     - Sariel's can_interact var needs to be incorporated into the update_next_scene function (should exist now)
///         - in general the actual transitions need be implemented
///     - cobweb needs to exist for loading sake and for puzzle sake
///         - needs to be inaccessible/not interactable prior to puzzle
///     - Flower Puzzle:
///         - flower creation needs to exist for loading sake and for puzzle sake
///             - needs to be localised around FLOWER_AREA_SARIEL, inaccessible/not interactable prior to puzzle
///         - maybe make 'fake flowers' (like script suggests)
///         - need POT (and pot that fills w/ flowers)
///     - Cave trigger must be made to exist in some trivial fashion
///     - Forced moves may need the following features
///         - perpendicular left/right movement given some target position
///         - sprite direction specification (particularly AFTER move is over)
///         - offset vectors (i.e. go to location "WHEREVER" but +2 in the x direction, -1 in Z)
///         - Less likely, but possible features:
///             - multi-forcedMoves (this will be a pain though)
///             - forced moves towards continuously moving object (not as much of a pain)
///     - learn how to save unity files between runs
///     - make debugger in CuToDo
///     - KNOWN ISSUE: teleporting (especially in save-loading) needs an extra-long-decay-timer for the backdrop
///     - add fns into dialogue
///     - add parts 3, 4, and alternate route and their corresponding puzzles
///     - >>> HIDDEN_FLOWER_ACTIVE:(bool value)
///     - >>> REACHED_ENDING:(ending-number)
///         - tampers w/ NEWGAME save, has image, etc...
///         
/// </summary>


public class CuToDo : MonoBehaviour //that's right, I'm objectifying myself. Beat it, punk!
{
    // Start is called before the first frame update
    [SerializeField] private GameObject flatImage; //should have sprite renderer w/sprite = null
    private static GameObject staticFlatImage;
    private static List<GameObject> locationMarkerList = new List<GameObject>();
    void Start()
    {
        
    }

    private void Awake()
    {
        staticFlatImage = flatImage; //Instantiate(flatImage);
    }

    public static void DebugLocations(Dictionary<string,Vector3> locDict) //called to 
    {
        float min_x = 0, min_z = 0, max_x = 0, max_z = 0, locNum = 0;

        foreach (Vector3 loc in locDict.Values)
        {
            if (loc.x > max_x) max_x = loc.x;
            if (loc.x < min_x) min_x = loc.x;
            if (loc.z > max_z) max_z = loc.z;
            if (loc.z < min_z) min_z = loc.z;
            //I don't actually care about min or max y bc y is mostly irrelevant here
            locNum++; // will be used for y instead
            //presume the dictionary has no duplicate values...
        }
        float range_x = max_x - min_x, range_z = max_z - min_z;

        float kvpNum = 0;
        foreach (KeyValuePair<string,Vector3> locKvp in locDict)
        {
            GameObject newMarker;
            newMarker = Instantiate(staticFlatImage, locKvp.Value, Quaternion.identity);
            newMarker.name = locKvp.Key;
            SpriteRenderer sr = newMarker.GetComponent<SpriteRenderer>();
            Color clr = new Color(
                (locKvp.Value.x - min_x) / range_x, 
                (kvpNum++) / locNum, 
                (locKvp.Value.z - min_z) / range_z
                );
            sr.color = clr;
            locationMarkerList.Add(newMarker);
        }
    }
}
