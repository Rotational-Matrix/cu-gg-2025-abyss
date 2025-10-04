using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_canvas_manager : MonoBehaviour
{
    // L + Ratio I haven't implemented anything yet
    /* The plan is to make this the top-level communicator for the whole Dialogue_Canvas
     * So observe the following lower-level files
     *  - The Choice Canvas should handle all choice boxes collectively
     *      - ChoiceBoxHandler will be present for each ChoiceBox
     * 
     * This should be the code that does the following:
     *  - communicates and directs the ink file (obtians strings and #tags from it, too)
     *  - parse strings from said ink file for commands
     *  - populate the main dialogue body
     *      - should handle text crawl and text speed-the-hell-up (i.e. spontaneously ending text crawl)
     *  - fill in the sprites of the speakers (or lacktherof)
     *  - perish
     *  - play sound for dialogue/narration (???)
     *  - Activates/disables the choice canvas, sends the choice strings and receives the choice index
     */

}
