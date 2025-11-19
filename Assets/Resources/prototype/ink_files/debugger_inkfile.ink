-> debug_knot

//ctrl-f the following: Overlays/ <-- route to get sprites
// available sprites: eve1, sariel1, sariel2, sariel3 (1,3)

VAR actNumber = 1 //act as in Act I, Act II, etc
VAR currentScene = -> debug_knot //will track currentScene


VAR strlenConfigKnown = false
VAR triedToIncreaseBrightness = false
VAR cave_visited = false

VAR flowerCounter = 0

=== debug_knot ===

= debug_stitch_1 
#READ_AS_STAGE_LINES:FALSE
Entering Debug Knot (1st stitch)
This is an extra buffer
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

=== pseudo_done ===
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
-> DONE // this is just to be safe.

=== sariel_interact ===
#READ_AS_STAGE_LINES:TRUE
//the unity object mere needs to call sariel_interact
//or sariel_interact.context_assign
<> ->context_assign
= context_assign
{ actNumber:
- 1: -> act_1
- 2: -> act_2
- 3: two
- else: lots
}
= act_1
lorem ipsum solem dicut
test, etc.
-> debug_knot

= act_2
lorem ipsum solem dicut
-> debug_knot
/*+ [nothing] <> //this glue makes it so there is no 'blank space'
 - <> The above should not have any key presses which show nothing (nothing!).*/

//primarily stolen from the the doc part
=== part_I ===
#READ_AS_STAGE_LINES:TRUE

>>> START_DIALOGUE

NO_SPEAKER: There is no sound at first.

Only the faint pressure of existence. The ache of being almost.

Then, a voice. It sings as gently as a fingertip brushing along my jaw, pressing down ever so delicately against my lips.

//maybe a sillhouette? I'm going to place sariel here temporarily
#sprite: Overlays/sariel1
???: “Can you hear me?”

//I have deliberately chosen to display NONE as sprite bc string of thoughts
#sprite: NONE
I don’t yet know what hearing means, nor what it is to be alive. 

I only know the shape and sound of that voice, smooth and deliberate with utmost clarity. 

It cuts through the fog of my barely formed thoughts, condensing vapor into something tangible, something real. 

The flutter it leaves in my chest suppresses the trace of unease that grazes my heart.

Light follows sound. Blurred at the edges, flickering, like a dwindling candle flame. 

I reach towards it. The light leans closer.

// again, maybe sillhouette
#sprite: Overlays/sariel1 
???: “You’re safe.”

The light’s murmurs are feminine and warm.

???: “I’ll help you understand.”

I don’t ask what safe means, yet the word takes root in my chest all the same, sprouting into belief. 

The world begins to congeal around me. Damp air, cool soil, and petrichor. 

#sprite: Overlays/eve1
Me?: “Who are you?” 

As I ask, my voice breaks as it exits my throat. 

#sprite: Overlays/sariel3
Sariel: “Sariel.” 

The light breathes, almost laughing. 

Sariel: “And you’re… mine, I think.”

Something tender opens in my throat. Not fear, not joy, but something in between.

The light brightens, outlining the suggestion of trees and a path that hadn’t been there mere seconds ago.

>>> STOP_DIALOGUE //I am personally adding this
<> -> pseudo_done

=== part_II ===
//#READ_AS_STAGE_LINES:TRUE
= segment_1 //ends w/ walking to animal area to find lamb
>>> START_DIALOGUE

#sprite: NONE
Each sound of the forest startles me with its intimacy. The world is a mouth whispering against my ear, and I don’t yet know if it is kind.

Sariel, luminous as ever, trails ahead. 

#sprite: Overlays/sariel3
Sariel: “Stay close.” 

Each of her words is draped in fondness.

I nod. It’s easier than asking why.

>>> STOP_DIALOGUE
//[walking to animal area]
<> -> pseudo_done

= lamb_encounter
>>> START_DIALOGUE
#sprite: NONE
We come upon a small, white animal in the underbrush, trembling and breathing in broken rhythms.

Its wool is matted with blood, a red too vivid for the incipient palette of my sight.

#sprite: Overlays/sariel3
Sariel: “Oh, you poor thing.” 

Sariel’s murmurs are gentle. 

Sariel: “An injured lamb. We can help it. Don’t you want to help, Eve?”

#sprite: Overlays/eve1
Eve: “Eve?” 

I echo the name, a lump in my throat. 

I do. The want is immediate, almost desperate, because helping feels like proof of goodness. My previous confusion slips my mind.

Sariel’s hand glides along the side of my neck, guiding my gaze towards the dark mouth of a cave in the distance. 

#sprite: Overlays/sariel3
Sariel: “There’s a cave nearby. It’s dark, but you’ll be safe if you hurry. The cobwebs inside can be used to mend wounds.”

#sprite: Overlays/eve1
I peer into the void. Unease worms its way under my skin.

Eve: “If you’re watching the lamb, does that mean I… have to go alone?”

#sprite: Overlays/sariel3
Sariel laughs. It’s gentle and uninhibited, and I find my worries melting under its warmth. 

>>> STOP_DIALOGUE //[walking to cave]
<> -> pseudo_done

= init_cave
#BLOCK_IF_TRUE: cave_visited
>>> START_DIALOGUE

~ cave_visited = true

#sprite: Overlays/eve1
Stepping inside feels like drowning upright. The air is far too damp and thick with must. 

I hastily turn to gather the lattices of silver strands that cling to the stone interior.

//[puzzle time]

>>> STOP_DIALOGUE

<> -> pseudo_done

= post_cave

>>> START_DIALOGUE

#sprite: Overlays/eve1
My heart finally calms as I finish collecting the last threads the cave has to offer.

Then, static. A flicker at the edge of my sight. Something shifts ahead of me, hoofed and wrong, its silhouette splitting from the fuzzy darkness.

My breath stutters, my heart pounding violently against my ribcage. The dull noise of the cave distills into a single sharp frequency, burrowing itself into my skull.

#sprite: Overlays/sariel3
Sariel: “Eve.” 

Sariel’s voice echoes, distant, yet perfectly clear. 

Sariel: “Come back to me.”

#sprite: Overlays/eve1
My legs obey before I do. The world jerks, and my vision fractures into streaks of white noise and almost painful adrenaline.

When I stumble into the light again, Sariel catches me. Her embrace is too tight, almost reverent. 

#sprite: Overlays/sariel3
Sariel: “There, there.”

Sariel: “See how dangerous it was without me?”

Her words are soft and breathy.

#sprite: Overlay/eve1
Eve: “Sariel-”

I choke. I try to speak, but my words, a sob, snag in my throat. Sariel brushes a strand of hair away from my face and smiles.

#sprite: Overlay/sariel1
Sariel: “You found exactly what I needed. Such a good girl.”

#sprite: Overlay/eve1
My hands tremble, my heart struggling to come back to a level of normalcy as I watch her tend to the wounded animal, wrapping its leg in the gathered silk. The threads cling beautifully, glowing softly in Sariel’s light.

#sprite: Overlay/sariel3
Sariel: “Cobwebs have always been excellent for stopping bleeding,” Sariel explains. “Isn’t that lovely? To be enveloped until you’re whole again.”

#sprite: Overlay/eve1
I watch the lamb shiver, its leg fully cocooned. An emotion I can’t place ripples somewhere deep within me.

#sprite: Overlay/sariel3
Sariel: “I was worried for you. My heart stopped when you suggested going alone.”

#sprite: Overlay/eve1
Eve: “I- I was only-”

#sprite: Overlay/sariel3
Sariel: “I know. You just wanted to help.”

Sariel: “And you did. You did so well. But…” 

There’s a pause sharp enough to draw blood. 

Sariel: “You frightened me, and it seems for good reason.”

#sprite: Overlay/eve1
The guilt blooms instantly, raw and uncomfortable. 

#sprite: Overlay/sariel3
After a beat, Sariel speaks again.

Sariel: “Maybe, it’s best if we make sure you can’t drift away into danger like that again.”

She raises her hand, and something luminescent cinches around my throat. A string of light stretches between us, vanishing into Sariel’s palm. 

Sariel: “Try to move now.”

#sprite: Overlay/eve1
I step back; the thread tightens, and my heart jumps, nerves alight.

#sprite: Overlay/sariel3
Sariel: “See?” 

Sariel’s smile is a wound wrapped in sweetness.

Sariel: “Isn’t that better? Don’t you feel safer?”

#sprite: Overlay/eve1
Her last word echoes through my chest, hollow and obedient. 

#sprite: Overlay/sariel3
She hums softly, almost amused. 

Sariel: “Mm, isn’t she beautiful?”

#sprite: Overlay/eve1
I lift my head, and our eyes meet, but it feels as though her gaze peels back every layer of my being. 

Eve: “What?”

#sprite: Overlay/sariel3
Silence. Then, she laughs. 

Sariel: “I’m talking about you.”

Sariel: “Now, follow.”

Her tone shifts suddenly to strict and authoritative, making my back straighten reflexively.

Sariel promptly strides ahead. The thread tugs on my neck. 

#sprite: Overlay/eve1
Heat rises to my face, a mixture of humiliation and a pleasant nervousness I can’t place.

It feels like I’m her dog, following orders and being pulled along by a leash.

#sprite: Overlay/sariel3
She turns back for a moment, wordless. 

Then, she slightly squints her eyes, amusedly, as if she knows what I’m thinking. 

#sprite: Overlay/eve1
My heart pounds uncomfortably against my ribcage. 

Her gaze has a quality that makes me feel pried open, exposed, and collected. 

Not giving more thought to it, I scurry after her, my teeth lightly pinching the tip of my tongue.

//[walk to knave puzzle area]
>>> STOP_DIALOGUE

//CONT AT WALK TO KNAVE AREA



