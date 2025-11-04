-> debug_knot

VAR strlenConfigKnown = false

=== debug_knot ===

= debug_stitch_1 

Entering Debug Knot (1st stitch)
 + [run debug tests]
    -> debug_test_run
 
 + [attempt dialogue closure]
    Note that you will get kicked, and then, if you rejoin w/o jumping, you will be forced back to the overall debug knot.
    >>> STOP_DIALOGUE
    You seem to have rejoined after being kicked.
    -> debug_knot
 
 + [The intro-writing segment from the doc]
    -> intro_writing
 
= debug_test_run

Entering debug test run
 + [nothing] <> //this glue makes it so there is no 'blank space'
 - <> The above should not have any key presses which show nothing (nothing!).
 This text should be $color:"red"$red$/color$
 Next, we'll test a few tags.
 
 #speaker: Heya! this is the speaker box
 #lSprite: prototype/sprites/proto_A #rSprite: prototype/sprites/proto_B
 The left should be A, the right should be B
 
 
 #speaker: ...
 #lSprite: NONE # rSprite: NONE
 
 Now, however, these details should've changed
 
 -> debug_knot
 

 
= command_explanations
All commands are to be preceded by ">>>"
This will be organised into better scaling categories
but for now, note $color:"green"$spooky text$/color$ should be red.
going back to debug stitch -> debug_stitch_1
/* What needs to be tested:
 * - END_DIALOGUE
 * - attempt innate TMPro inline commands
 * - attempt sprite selection w/tags (a,b,c)
 * - list commands
 * - test selection types 
 */




=== intro_writing ===

#rSprite:prototype/sprites/proto_EVE
There is no sound at first. #speaker:


Only the faint pressure of existence. The ache of being almost.


Then, a voice. It sings as gently as a fingertip brushing along her jaw, pressing down ever so delicately against her lips.


#lSprite: prototype/sprites/proto_SARIEL
#speaker: ???
“Can you hear me?”

#speaker: NO_SPEAKER
She doesn’t yet know what hearing means, nor what it is to be alive. 

She only knows the shape and sound of that voice, smooth and deliberate with utmost clarity. 

It cuts through the fog of her barely formed thoughts, condensing vapor into something tangible, something real. 

The flutter it leaves in her chest suppresses the trace of unease that grazes her heart.

Light follows sound. Blurred at the edges, flickering, like a dwindling candle flame. 

She reaches towards it. The light leans closer.

#speaker: ???
“You’re safe,” it murmurs. “I’ll help you understand.”

#speaker: NO_SPEAKER
She doesn’t ask what safe means, yet the word takes root in her chest all the same, sprouting into belief. 

The world begins to congeal around her. Damp air, cool soil, and petrichor. 

#speaker: Eve
“Who are you?” she asks, her voice breaking as it exits her throat. 

#speaker: Sariel
“Sariel,” the light breathes, almost laughing. “And you’re… mine, I think.”

#speaker: NO_SPEAKER
Something tender opens in her throat. Not fear, not joy, but something in between.


The light brightens, outlining the suggestion of trees and a path that hadn’t been there mere seconds ago.

#speaker: Cooper
#lSprite: NONE #rSprite: NONE
The intro has ended. 
#speaker:
-> debug_knot

-> END //included to quiet system

