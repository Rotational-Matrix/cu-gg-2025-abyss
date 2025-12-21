using System.Collections;
using System.Collections.Generic;
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
///     - Sariel's can_interact var needs to be incorporated into the update_next_scene function
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
///     - KNOWN ISSUE: teleporting (especially in save-loading) needs an extra-long-decay-timer for the backdrop
///     - add fns into dialogue
///     - add parts 3, 4, and alternate route and their corresponding puzzles
///     - >>> REACHED_ENDING:(ending-number)
///         - tampers w/ NEWGAME save, has image, etc...
/// 
/// 
/// </summary>
public class CuToDo : Object //that's right, I'm objectifying myself. Beat it, punk!
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
