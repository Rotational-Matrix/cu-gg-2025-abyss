//{this_scene == ->part_I: does it, so it does}
-> debug_knot

//ctrl-f the following: Overlays/ <-- route to get sprites
// available sprites: eve1, sariel1, sariel2, sariel3 (1,3)

VAR actNumber = 1 //act as in Act I, Act II, etc
VAR next_scene = -> part_I //will track scene to be called (mostly debugger)
VAR this_scene = -> part_I //will actually track scene to be called

VAR is_start_save = true //currently should always be true, add only to account for potential implementation of Dialogue based- non start saves. Note that, if is_start_save, StateManager will attempt to place the save into dialogue using this_scene (this DOES mean that the 'visit number' can arbitrarily become 2 (instead of 1) for autosave starts)


VAR strlenConfigKnown = false
VAR triedToIncreaseBrightness = false
VAR block_init_cave = true

// note that 0 is not ended, 1 is 'true' and 2 is 'rebel'
VAR reachedEnding = 0


// collection of meta info...
VAR pastTrueEnding = false
VAR pastRebelEnding = false

// various states concerning unity status and saving
// current protocol is that saving is done at >>> STOP DIALOGUE
// This is goofy, but these will be informed by unity and fns here
VAR eve_x1 = 0.0    //
VAR eve_x2 = 0.0    // eve Vector3 for saving position
VAR eve_x3 = 0.0    //
VAR sariel_x1 = 0.0 //
VAR sariel_x2 = 0.0 // Sariel Vector3 for saving position
VAR sariel_x3 = 0.0 //

VAR leashActive = false // starts 'off'

VAR leashInertia = 0.0  //
VAR leashDamping = 0.0  // all communicate with RoamCmdr
VAR leashStrength = 0.0 //
VAR leashMaxDist = 0.0  // probably the only used coef




//reminder that knave_puzzle_knot.correct_answer will tell if the player has gotten the answer correct or not

VAR cave_transition_allowed = false
VAR flower_puzzle_start = false
VAR cobweb_puzzle_start = false

VAR flowerCounter = 0 //counter that eve has collected
VAR cobweb_obtained = 0

VAR sariel_can_interact = true //sariel will call this as a check to see if she can interact // TODO : make sure this gets into the update next scene fn!!!!

// Effectively just macros
VAR _step = 0.5 //flat distance of a 'step'

VAR _e = "eve" //this is relatively stupid, but at least it can be ctrl-f-ed out
VAR _s = "sariel" 
VAR _n = "NONE"

=== debug_knot ===

= debug_stitch_1 
#READ_AS_STAGE_LINES:FALSE
>>> START_DIALOGUE
Entering Debug Knot (1st stitch)
 + [run debug tests]
    -> debug_test_run
 
 + [attempt dialogue closure]
    Note that you will get kicked, and then, if you rejoin w/o jumping, you will be forced back to the overall debug knot.
    Oi wait there is no shot it skips the first dialogue after a coice every time
    >>> STOP_DIALOGUE
    You seem to have rejoined after being kicked.
    -> debug_knot
 
 + [The intro-writing segment]
    -> intro_writing
    
 + [Go through next main dialogue]
    -> next_scene
 

= debug_test_run
#{sprite("NONE",1)}
#{sprite("eve",1)}
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
 
 
=== admin_powers ===
= init
What do you want to do?
+ [Commands]
    ->commands
+ [Scene Selection]
    ->scene_select
+ [Leave]
    >>> STOP_DIALOGUE
    ->pseudo_done
= commands
What commands to forcefully attempt (will break game)
+ [Leash]
    Set Active?
    + + [True]
        {set_leash_active(true)}
        ->commands
    + + [False]
        {set_leash_active(false)}
        ->commands
    + + [Back To Cmds]
        ->commands

+ [Leave]
    >>> STOP_DIALOGUE
    ->pseudo_done

= scene_select
Which scene to head to? (note, this is added for convenience, will almost certainly break the game)
+ [Next Scene]
    ->next_scene
+ [Any Other Scene]
    Okay, so this could be implemented, but right now it isn't the most important thing. I have kept this here primarily to have instant access to the 'next scene' operator (that sariel used to provide).
    ->scene_select
+ [Leave]
    >>> STOP_DIALOGUE
    ->pseudo_done
 
=== save_load_knot ===
//#READ_AS_STAGE_LINES:TRUE
>>> START_DIALOGUE
-> this_scene

=== function assign_next_scene(-> scene, sarInitiatesNext) ===
~ next_scene = scene
~ sariel_can_interact = sarInitiatesNext

=== function assign_this_scene(-> scene) ===
~ this_scene = scene


//#{sprite(_e, 0)}
//#{sprite(_s, 0)}
=== function sprite(character, type) ===
//~ temp spriteFile = 
sprite: {character == "NONE":NONE|Overlays/{character}_new}

// doesn't touch the positional locations of eve, sar
// note that it is legal to say forced_move(_e,_s)
=== function forced_move(character, location, spdFactor) ===
>>> FORCED_MOVE:{character},{location},TRUE,1,{spdFactor}

//is_prop means is proportional (if not, the dist is flat)
=== function forced_move_dir(character, location, is_prop, dist, spdFactor) ===
>>> FORCED_MOVE:{character},{location},{is_prop:TRUE,{dist}|FALSE,{_step * dist}},{spdFactor}

=== function teleport(character, location, x_offset, z_offset) ===
>>> TELEPORT:{character},{location},{x_offset},{z_offset}
// 
=== function autosave(is_start_type, -> scene)
{ this_scene != scene:
  ~ is_start_save = is_start_type
  ~ assign_this_scene(scene)
  >>> AUTOSAVE:{is_start_type:START|NONE}
}

=== function set_leash_active(value) ===
~ leashActive = value //sets value of leash for saving
>>> LEASH_SET:{value:TRUE|FALSE}

=== function set_leash_coef(inertia, damping, strength, maxDist, activeVal) ===
{inertia > 0:
~ leashInertia = inertia
}
{damping > 0:
~ leashDamping = damping
}
{strength > 0: 
~ leashStrength = strength
}
{maxDist > 0:
~ leashMaxDist = maxDist
}
~ set_leash_active(activeVal) //notably NOT in a conditional




=== function backdrop_set(value) ===
>>> BACKDROP_SET:{value:TRUE|FALSE}

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


-> this_scene


=== pseudo_done ===
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
-> debug_knot // this is just to be safe.


=== next_scene_knot ===
#READ_AS_STAGE_LINES:TRUE

>>> START_DIALOGUE
-> next_scene

=== sariel_interact ===
#READ_AS_STAGE_LINES:TRUE

>>> START_DIALOGUE
//the unity object mere needs to call sariel_interact
//or sariel_interact.context_assign
-> next_scene
= context_assign
{ actNumber:
- 1: -> next_scene
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
//initiate event: new game start
>>> START_DIALOGUE
~ autosave(true, -> part_I)

{backdrop_set(true)} //fyi, to coordinate syntax, many '>>> COMMANDS' will be actually ran as functions in the code! I will try to leave START and STOP alone.

#sprite: NONE
NO_SPEAKER: There is no sound at first.

Only the faint pressure of existence. The ache of being almost.

Then, a voice. It sings as gently as a fingertip brushing along my jaw, pressing down ever so delicately against my lips.

//maybe a sillhouette? I'm going to place sariel here temporarily
#{sprite(_s, 0)}
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
#{sprite(_s, 0)} 
???: “You’re safe.”

#sprite: NONE
The light’s murmurs are feminine and warm.

#{sprite(_s, 0)} 
???: “I’ll help you understand.”

#sprite: NONE
I don’t ask what safe means, yet the word takes root in my chest all the same, sprouting into belief. 

The world begins to congeal around me. Damp air, cool soil, and petrichor. 

#{sprite(_e, 0)}
Me?: “Who are you?” 

#sprite: NONE
As I ask, my voice breaks as it exits my throat. 

#{sprite(_s, 0)}
Sariel: “Sariel.” 


#sprite: NONE
The light breathes, almost laughing. 

#{sprite(_s, 0)}
Sariel: “And you’re… mine, I think.”

#sprite: NONE
Something tender opens in my throat. Not fear, not joy, but something in between.

{backdrop_set(false)}

The light brightens, outlining the suggestion of trees and a path that hadn’t been there mere seconds ago.
~ assign_next_scene(-> part_II.segment_1, true)
>>> SARIEL_INSTANT_INTERACT:TRUE //bc sariel responsible for next transition and not after forced move
>>> STOP_DIALOGUE
-> pseudo_done

=== part_II ===
//initiate event: talk to sariel
= segment_1 //ends w/ walking to animal area to find lamb
>>> START_DIALOGUE
~ autosave(true, -> part_II.segment_1)

#sprite: NONE
Each sound of the forest startles me with its intimacy. The world is a mouth whispering against my ear, and I don’t yet know if it is kind.

Sariel, luminous as ever, trails ahead. 

#{sprite(_s, 0)}
Sariel: “Stay close.” 

#sprite: NONE
Each of her words is draped in fondness.

I nod. It’s easier than asking why.

~ assign_next_scene(-> part_II.lamb_encounter, true)
~ forced_move(_s,"ANIMAL_AREA", 1) //SARIEL FORCED MOVE TO ANIMAL AREA
>>> STOP_DIALOGUE
//[walking to animal area]
-> pseudo_done

//initiate event: talk to sariel once she finishes her forced move
= lamb_encounter
>>> START_DIALOGUE
~ autosave(true, -> part_II.lamb_encounter)

#sprite: NONE
We come upon a small, white animal in the underbrush, trembling and breathing in broken rhythms.

Its wool is matted with blood, a red too vivid for the incipient palette of my sight.

#{sprite(_s, 0)}
Sariel: “Oh, you poor thing.” 

#sprite: NONE
Sariel’s murmurs are gentle. 

#{sprite(_s, 0)}
Sariel: “An injured lamb. We can help it. Don’t you want to help, Eve?”

#{sprite(_e, 0)}
Eve: “Eve?” 

#sprite: NONE
I echo the name, a lump in my throat. 

I do. The want is immediate, almost desperate, because helping feels like proof of goodness. My previous confusion slips my mind.

Sariel’s hand glides along the side of my neck, guiding my gaze towards the dark mouth of a cave in the distance. 

#{sprite(_s, 0)}
Sariel: “There’s a cave nearby. It’s dark, but you’ll be safe if you hurry. The cobwebs inside can be used to mend wounds.”

#sprite: NONE
I peer into the void. Unease worms its way under my skin.

#{sprite(_e, 0)}
Eve: “If you’re watching the lamb, does that mean I… have to go alone?”

#sprite: NONE
Sariel laughs. It’s gentle and uninhibited, and I find my worries melting under its warmth. 
~ block_init_cave = false
~ cave_transition_allowed = true
~ assign_next_scene(-> part_II.init_cave, false) //sariel NOT RESPONSIBLE for next transition! The darned cave is!
>>> STOP_DIALOGUE //[walking to cave]
-> pseudo_done

= init_cave
#BLOCK_IF_TRUE: block_init_cave
>>> START_DIALOGUE
~ autosave(true, -> part_II.init_cave)

~ block_init_cave = true
~ cobweb_puzzle_start = true //so that cobweb can be grabbed!
~ cave_transition_allowed = false

#sprite: NONE
Stepping inside feels like drowning upright. The air is far too damp and thick with must. 

I hastily turn to gather the lattices of silver strands that cling to the stone interior.

//[puzzle time]
~ assign_next_scene(-> part_II.post_cave, false) //seems to trigger on cobweb pickup (since there doesn't exist cobwebs yet, rigging via the admin powers is necessary...
>>> STOP_DIALOGUE

-> pseudo_done

= post_cave
>>> START_DIALOGUE
~ autosave(true, -> part_II.post_cave)

#sprite: NONE
My heart finally calms as I finish collecting the last threads the cave has to offer.

Then, static. A flicker at the edge of my sight. Something shifts ahead of me, hoofed and wrong, its silhouette splitting from the fuzzy darkness. //Wait... Where is this occurring? I have not implemented this...

My breath stutters, my heart pounding violently against my ribcage. The dull noise of the cave distills into a single sharp frequency, burrowing itself into my skull.

#{sprite(_s, 0)}
Sariel: “Eve.” //hold up, eve should be in cave, whereas sariel should be in animal area

#sprite: NONE
Sariel’s voice echoes, distant, yet perfectly clear. 

#{sprite(_s, 0)}
Sariel: “Come back to me.”

{forced_move_dir(_e, _s, true, 0.5, 1)} //legs obeying

#sprite: NONE
My legs obey before I do. The world jerks, and my vision fractures into streaks of white noise and almost painful adrenaline.

When I stumble into the light again, Sariel catches me. Her embrace is too tight, almost reverent. 

#{sprite(_s, 0)}
Sariel: “There, there.”

Sariel: “See how dangerous it was without me?”

#sprite: NONE
Her words are soft and breathy.

#{sprite(_e, 0)}
Eve: “Sariel-”

#sprite: NONE
I choke. I try to speak, but my words, a sob, snag in my throat. Sariel brushes a strand of hair away from my face and smiles.

#{sprite(_s, 0)}
Sariel: “You found exactly what I needed. Such a good girl.”

#sprite: NONE
My hands tremble, my heart struggling to come back to a level of normalcy as I watch her tend to the wounded animal, wrapping its leg in the gathered silk. The threads cling beautifully, glowing softly in Sariel’s light.

#{sprite(_s, 0)}
Sariel: “Cobwebs have always been excellent for stopping bleeding,” Sariel explains. “Isn’t that lovely? To be enveloped until you’re whole again.”

#sprite: NONE
I watch the lamb shiver, its leg fully cocooned. An emotion I can’t place ripples somewhere deep within me.

#{sprite(_s, 0)}
Sariel: “I was worried for you. My heart stopped when you suggested going alone.”

#{sprite(_e, 0)}
Eve: “I- I was only-”

#{sprite(_s, 0)}
Sariel: “I know. You just wanted to help.”

Sariel: “And you did. You did so well. But…” 

#sprite: NONE
There’s a pause sharp enough to draw blood. 

#{sprite(_s, 0)}
Sariel: “You frightened me, and it seems for good reason.”

#sprite: NONE
The guilt blooms instantly, raw and uncomfortable. 

#sprite: NONE
After a beat, Sariel speaks again.

#{sprite(_s, 0)}
Sariel: “Maybe, it’s best if we make sure you can’t drift away into danger like that again.”

{set_leash_active(true)}

#sprite: NONE
She raises her hand, and something luminescent cinches around my throat. A string of light stretches between us, vanishing into Sariel’s palm. 

#{sprite(_s, 0)}
Sariel: “Try to move now.”

{forced_move_dir(_e, _s, false, -1, 1)}

#sprite: NONE
I step back; the thread tightens, and my heart jumps, nerves alight.

#{sprite(_s, 0)}
Sariel: “See?” 

#sprite: NONE
Sariel’s smile is a wound wrapped in sweetness.

#{sprite(_s, 0)}
Sariel: “Isn’t that better? Don’t you feel safer?”

#sprite: NONE
Her last word echoes through my chest, hollow and obedient. 

#sprite: NONE
She hums softly, almost amused. 

#{sprite(_s, 0)}
Sariel: “Mm, isn’t she beautiful?”

#sprite: NONE
I lift my head, and our eyes meet, but it feels as though her gaze peels back every layer of my being. 

#{sprite(_e, 0)}
Eve: “What?”

#sprite: NONE
Silence. Then, she laughs. 

#{sprite(_s, 0)}
Sariel: “I’m talking about you.”

#{sprite(_s, 0)}
Sariel: “Now, follow.”

#sprite: NONE
Her tone shifts suddenly to strict and authoritative, making my back straighten reflexively.

Sariel promptly strides ahead. The thread tugs on my neck. 

Heat rises to my face, a mixture of humiliation and a pleasant nervousness I can’t place.

It feels like I’m her dog, following orders and being pulled along by a leash.

She turns back for a moment, wordless. 

Then, she slightly squints her eyes, amusedly, as if she knows what I’m thinking. 

My heart pounds uncomfortably against my ribcage. 

Her gaze has a quality that makes me feel pried open, exposed, and collected. 

Not giving more thought to it, I scurry after her, my teeth lightly pinching the tip of my tongue.

//[walk to knave puzzle area]
~ forced_move(_s, "APPROACHING_KNAVES", 1) // SARIEL FORCED MOVE TO KNAVE PUZZLE AREA
~ assign_next_scene(-> part_II.knave_puzzle, true) //sariel is responsible for initiating the next scene (after forcedMove)
>>> STOP_DIALOGUE
-> pseudo_done
//CONT AT WALK TO KNAVE AREA

= knave_puzzle
>>> START_DIALOGUE
~ autosave(true, -> part_II.knave_puzzle)
//merely directs to the actual puzzle because the puzzle is much longer
-> knave_puzzle_knot

= flower_puzzle
>>> START_DIALOGUE
~ flower_puzzle_start = true 
~ autosave(true, -> part_II.flower_puzzle)

#sprite: NONE
The trees thin, giving way to an almost impossibly symmetrical glade with flowers sparsely scattered.

#sprite: NONE
I look up from the pale grass bending beneath my feet and spot a large, stone archway.

//sprite: pot
#sprite: NONE
At its feet sits an empty, unassuming clay pot. Its mouth gapes, waiting to be filled.

#{sprite(_e, 0)}
Eve: “{false:UwU }What is this?”

#{sprite(_s, 0)}
Sariel: “A test.” 

#sprite: NONE
As Sariel answers, her feet still for only just a moment. 

#{sprite(_s, 0)}
Sariel: “Everything is.”

#sprite: NONE
She walks among the flowers with effortless grace, the same way light bends through glass and refracts into a breathtaking spectrum. 

Sariel crouches, lifting a blossom by its stem, and brings it to my face.

#{sprite(_s, 0)}
Sariel: “Smell.”

#sprite: Overlays/NONE
The fragrance is strange. It’s sweet at first, then metallic, then faintly sharp.

#{sprite(_s, 0)}
Sariel: “This one.”

Sariel: “The other kinds won’t do. Fill the pot with these, 10 to be exact, and we will be able to pass through.”

#sprite: NONE
I glance across the meadow. 

I don’t want to question her.

But curiosity tips the scale, outweighing my reluctance.

#{sprite(_e, 0)}
Eve: “How do you know which kind it wants?”

#sprite: NONE
She smiles, soft and unbothered. 

#{sprite(_s, 0)}
Sariel: “Don’t you trust me?”

#sprite: NONE
I do. Before thinking, I nod, though a restless feeling flickers behind my ribs.

Filled with the fervor to please Sariel, I stride towards the edge of the grassy opening. 

//[walking to flower area]
//many of the [walking] may become forced movement commands, but this one is 100% a forced movement command
// >>> FORCED_MOVE:flower_area TODO FIXXX

Kneeling, I begin to gather flowers, inhaling the scents and making mental comparisons to the one Sariel had shown me. The petals cling to my fingers, wet with dew.

//[puzzle time - collect 8 more flowers]
~ assign_next_scene(-> part_II.last_flower, false) //will get set true by the final flower
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower
>>> START_DIALOGUE
~ autosave(true, -> part_II.last_flower)

#sprite: NONE
Sariel hums as she watches me, low and melodic. 

#{sprite(_s, 0)}
Sariel: “You move so delicately, Eve. Like it’s your own garden you’re tending to.”

#{sprite(_e, 0)}
Eve: “There aren’t any left.” 

#sprite: NONE
I sigh, picking another scentless flower and tossing it aside. 

#{sprite(_s, 0)}
Sariel: “There are always more if you know where to look.”

#sprite: NONE
Her tone is delicate, yet cruel and chastising, causing me to shrink in embarrassment.

I force myself to retrace my steps, familiar blades of grass brushing my calves.

//[walking to entrance of flower area - Sariel does NOT move]
// GOTO FIXXX prolly triggers on either perimeter or on reenter trigger
~ assign_next_scene(-> part_II.last_flower_psych, false)//due to leash stretches
>>> SARIEL_DIST_TRIGGER:TRUE,{leashMaxDist} //to simulate attempting to stretch the leash
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower_psych
>>> START_DIALOGUE
~ autosave(true, -> part_II.last_flower_psych)

//note that more of the ROAM state is visible if no sprite is present
#sprite NONE 
The string tightens with disapproval as I step too far, a choked gasp finding itself stuck in my throat. The burn is a gentle, almost affectionate pain.

#sprite: NONE
Sariel’s voice follows, sweet and distant. 

#{sprite(_s, 0)}
Sariel: “Careful. You know what happens when you wander too far.”

#sprite: NONE
I retreat instantly, her hum of disapproval easing into silence. 

#{sprite(_e, 0)}
Eve: “I’m sorry.”

{forced_move_dir(_e, _s, false, 1, 2)} //will face towards sariel when she does this
//[Eve gets moved back to Sariel without roam state when the line above is read - don’t exit dialogue state] //Tusen takk! this is very helpful
// >>> FORCED_MOVE:TO_SARIEL
#{sprite(_s, 0)}
Sariel: “I know you are.” 

#sprite: NONE
Sariel exhales, the sound halfway between amusement and pity. 


{forced_move_dir(_e, "FLOWER_POT_POS", true, 0.7, 1)}
I return to the pot, still missing one bloom. Sariel stands behind me, her chest lightly brushing against my backside, one hand absentmindedly resting on my hip.
//{forced_move_dir(_s, _e, true, 0.7, 1)} NOTE, Make sariel actually move to moving target.

#{sprite(_s, 0)}
Sariel: “Oh, poor thing.” 

#sprite: NONE
She coos, her voice falling somewhere between pity and amusement.

#{sprite(_s, 0)}
Sariel: “You worked so hard.”

#sprite: NONE
From behind, she produces the final flower, perfect and fragrant. 

I stare at the offering in her hand. 

#{sprite(_e, 0)}
Eve: “You… already had it?”

#{sprite(_s, 0)}
Sariel: “Mm.” 

#sprite: NONE
Sariel twirls the stem between her fingers. 

#{sprite(_s, 0)}
Sariel: “You couldn’t do it without me.”

#{sprite(_e, 0)}
Eve: “I didn’t-”

#{sprite(_s, 0)}
Sariel: “You didn’t ask for my help at all.” 

#sprite: NONE
The words glide from her mouth like silk, yet they hit heavy enough to bruise. 

#{sprite(_s, 0)}
Sariel: “I thought you trusted me.”

#sprite: NONE
The guilt lands like a weight in my chest. 

#{sprite(_e, 0)}
Eve: “I do.”

#{sprite(_s, 0)}
Sariel: “Then prove it.”

#sprite: NONE
I shiver from the sudden proximity of her breath against my neck. The hand on my hip tightens painfully before falling away.

Sariel places the stem into my trembling hand. 

#{sprite(_s, 0)}
Sariel: “Now, finish it.”

#sprite: NONE
As the pot receives its final bloom, a heavy creak sounds from the arch.

#{sprite(_s, 0)}
Sariel: “See?” 

#sprite: NONE
I swallow as she whispers against me.

#{sprite(_s, 0)}
Sariel: “When you listen to me, everything is fine.”

Sariel: “You couldn’t have done it without me.”

#sprite: NONE
I nod, unsure whether it’s agreement or surrender. The thread tightens once, almost possessive, yet strangely comforting. 

She smiles delicately, taking a step back. 

#{sprite(_s, 0)}
Sariel: “You’re learning to be good.”

>>> STOP_DIALOGUE
->pseudo_done


=== knave_puzzle_knot ===
-> pre_puzzle

= pre_puzzle

#sprite: NONE
The branches twist inward like devout believers bent in prayer.

// sprite may actually be the creatures here
#sprite: NONE
They follow the curvature of a very narrow opening, and the function of my brain stutters for a moment as my gaze flits downward.

Ahead sits a cluster of… creatures. 

Three mushroom-like figures. They are humanoid by only the faintest suggestion of their height and overall form. 

Behind them is a heavy wooden gate, the spotty lacquer a testament to its wear.

Their pale mushroom caps glisten with dew, bodies shifting slightly as they notice the two of us approaching.

None blinks. None speaks. They simply watch.

#sprite NONE
A chill crawls up my spine.

#{sprite(_e, 0)}
Eve: “Sariel…?”

#sprite: NONE
She stands just behind me, hands lightly brushing my shoulders, as if positioning me.

#{sprite(_s, 0)}
Sariel: “These three guard the gate.”

#sprite: NONE
Sariel gives a perfunctory glance at the figures before continuing.

#{sprite(_s, 0)}
Sariel: “They’ll open it if you identify their roles correctly.”

#{sprite(_e, 0)}
Eve: “Roles?”

#{sprite(_s, 0)}
Sariel: “One always speaks the truth. One always lies. And one…”

#sprite: NONE
She slightly shifts the angle of her head, breath now warming my neck. I shiver at the contact.

#{sprite(_s, 0)}
Sariel: “One is… unlike the others.”

#sprite: NONE
After waiting a few moments, I realize that’s all she has to offer and stiffen slightly. 

#{sprite(_e, 0)}
Eve: “Unlike the others? Isn’t that too vague?”

#{sprite(_s, 0)}
Sariel: “Mm, I thought you were more clever than that, Eve.”

#sprite: NONE
Shame pricks at my skin, hot and uncomfortable.

#{sprite(_e, 0)}
Eve: “I’m sorry.”

#sprite: NONE
I reply before I can even process my mouth moving. My chest constricts, placing my heart in a chokehold.

#{sprite(_s, 0)}
Sariel: “It’s unpredictable. I suppose random is the demotic term.”

#sprite: NONE
Before I can respond, mouth already open, she cuts off my chance.

#{sprite(_s, 0)}
Sariel: “Don’t overthink it. They won’t tell you their nature directly. And besides, wouldn’t that ruin the fun?”

Sariel: “You’ll make me proud, won’t you?”

//maybe mushroom sprite
#sprite: NONE
The mushrooms remain motionless, their beady eyes as unsettling as ever.

#{sprite(_e, 0)}
Eve: “I don’t know how to-”

#{sprite(_s, 0)}
Sariel’s body presses flush against my back, her hands gliding up the curve of my neck until her fingers splay gently on each side of my jaw, cupping my cheeks. 

#sprite: NONE
She turns my gaze to the one on my left, as though prompting me to start already.

#{sprite(_s, 0)}
Sariel: “You can do this.”

#sprite: NONE
She whispers, her breath warm against my ear.

#{sprite(_s, 0)}
Sariel: “Show me how clever you are.”

#sprite: NONE
I swallow.

#{sprite(_s, 0)}
Sariel: “Ask your questions, and they’ll each answer accordingly.”

#sprite: NONE
After pausing for a moment, she taps her fingers on my skin as though she’s had a lightbulb moment.

#{sprite(_s, 0)}
Sariel: “Ah, but I must warn you.”

Sariel: “They will only answer in their native tongue, ‘crrk’ or ‘fmmh,’ instead of ‘yes’ or ‘no.’ Which is which? That’s for you to deduce.”

#sprite: Overlays/NONE
The noises sound particularly strange coming from her mouth. I wet my lips, fighting back a smile at the absurdity.

#{sprite(_s, 0)}
Sariel notices, and something between a laugh and an exhale exits her nose.

Sariel: “Laugh if you’d like. I understand.”

#sprite: NONE
I bite my lip, guilt creeping up my throat.

#{sprite(_s, 0)}
Sariel: “You can do it, can’t you? Be good for me.”

Sariel: “If you need help, I’ll be right here.”

#sprite: NONE
Her tone makes simply asking for help feel like complete submission, and my heart thumps heavily.

I peruse the mushrooms. Their eyes are big, wet, and reflective in a way that would almost be cute if not for the size of the creatures. 

#{sprite(_e, 0)}
Eve: “Sariel…”

#sprite: NONE
I exhale, my voice barely audible.

#{sprite(_e, 0)}
Eve: “Which is the random one?”

#sprite: NONE
She laughs breathily, smile blooming too quickly, delighted by the question.

#{sprite(_s, 0)}
Sariel: “Ah-ah, that would be telling. But… if you want my guess…”

#sprite: NONE
Her lips brush the shell of my ear, and I flinch, heart jumping to my throat.

#sprite: NONE
Anticipation runs through my veins like a heady drug.

#{sprite(_s, 0)}
Sariel: “Aw, did you really think I’d tell you?”

#sprite: NONE
I shrink slightly at the mocking tone she suddenly adopts.

#{sprite(_s, 0)}
Sariel: “Go on. Perform for me, Eve. Let me see how well you can do.”

#sprite: NONE
I take a deep breath.

Pressing my tongue against the side of my cheek, I step forward.

#{sprite(_e, 0)}
Eve: “The left mushroom. Does ‘crrk’ mean ‘yes’?”

#sprite: NONE
I do my best to copy Sariel’s impression of the strange noise.

Her gaze sharpens with scrutiny, and I quickly add to my question before it’s too late.

#{sprite(_e, 0)}
Eve: “Ah, um, if and only if you are the truthful one… and the, uh, middle mushroom is the random one.”

// creature overlay???
#sprite: NONE
The creature’s cap tilts.

A low, rough syllable escapes its throat.

Left Mushroom: “Fmmh.”

#sprite: NONE
The sound is sticky. My stomach tightens.

#sprite: NONE
Sariel’s hands gently lace over my throat.

#{sprite(_s, 0)}
Sariel: “Mmm… Interesting.”

#sprite: NONE
I move to the middle mushroom, pulse loud in my ears. 

For a good while, I think about what to say. 

It’s all too fresh. The world and its rules.

My hands clench into fists.

#{sprite(_e, 0)}
Eve: “If and only if cobwebs can stop bleeding... does ‘crrk’ mean ‘yes’?”

//creature overlay
#sprite: NONE
Its mouth opens in a smooth, deliberate motion.

Middle Mushroom: “Crrk.”

I wince from the high pitch of the noise, expecting the same deep, throaty rumble as before.

#sprite: NONE
Sariel chuckles softly behind me.

#{sprite(_s, 0)}
Sariel: “I suppose I should have warned you.”

#{sprite(_e, 0)}
Eve: “How do you-”

#sprite: NONE
I stop myself before finishing my question. It feels wrong. 

She snaps me out of my thoughts, sensing my hesitation immediately.

#{sprite(_s, 0)}
Sariel: “Aww, Eve.”

#sprite: NONE
Her voice dips into something velvety and condescending.

#{sprite(_s, 0)}
Sariel: “Do you need my help? Or are you going to continue standing there like a fool?”

#{sprite(_e, 0)}
Eve: “O-Oh.”

#sprite: NONE
I stutter, embarrassed.

#sprite: NONE
Her fingers trail down to the thread around my throat, sliding beneath the string in a way that tightens it considerably.

She flicks her fingers up, tugging just enough to steal a breath.

I nod, desperate for her approval.

She smiles, pleased.

#{sprite(_s, 0)}
Sariel: “Ask one more question. To the middle mushroom again.”

#sprite: NONE
I feel lightheaded, her touch and voice too much to process at once.

It’s hard to think.

Where was I going with the last two questions?

I flip through every page of my mind, yet they’re all blank, devoid of any meaning, and the book is titled Sariel. 

The fear of disappointing her strangles my throat. I need to remember what I wanted to ask.

* [Is ‘fmmh’ ‘yes’ iff. the left one lies?]
    #{sprite(_e, 0)}
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if the left mushroom lies?”
    #sprite: NONE
    Middle Mushroom: “Fmmh.”
    
* [Is ‘crrk’ ‘yes’ iff. the left one is random?]
    #{sprite(_e, 0)}
    Eve: “Does ‘crrk’ mean ‘yes’ if and only if the left mushroom is unpredictable?”
    #sprite: NONE
    Middle Mushroom: “Crrk.”
    
* [is ‘fmmh’ ‘yes’ iff. you are the liar?]
    #{sprite(_e, 0)}
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if you are the liar?”
    #sprite: NONE
    Middle Mushroom: “Crrk.”

- (asked_3rd_question) I chew the inside of my cheek as it answers, looking back helplessly at Sariel for guidance.

She lets her arms fall slightly, now circling my shoulders, to allow me to face her.

I bring my voice down to a hushed whisper, not wanting the mushrooms to hear my answers just yet.

#{sprite(_e, 0)}
Eve: “The left mushroom, it’s…”

//because we don't have a text log rn, I have coded it like this to help the player, at the vary least, to remember what they already chose.
//Actually I could have just labelled the choices and use those, but I forgot I could do that (this works, so I don't need ot change it...)
// note that (random = 1, truthful = 2, lying = 3) is correct

~ temp selected_truthful = 0
~ temp selected_random = 0
~ temp selected_lying = 0
* [The truthful one]
    
    Eve: “The left mushroom is the truthful one.”
    ~ selected_truthful = 1
* [The random one]
    Eve: “The left mushroom is the random one.”
    ~ selected_random = 1 //notably the correct answer
* [The lying one]
    Eve: “The left mushroom is the lying one.”
    ~ selected_lying = 1
    
- (answer_1_given) Eve: “So the middle mushroom is…”

* {not selected_truthful} [The truthful one]
    Eve: “The middle mushroom is the truthful one.”
    ~ selected_truthful = 2 //notably the correct answer
* {not selected_random} [The random one]
    Eve: “The middle mushroom is the random one.”
    ~ selected_random = 2
* {not selected_lying} [The lying one]
    Eve: “The middle mushroom is the lying one.”
    ~ selected_lying = 2

- (answer_2_given) Eve: “Which means the right mushroom is…”
* {not selected_truthful} [The truthful one]
    Eve: “The right mushroom is the truthful one.”
    ~ selected_truthful = 3
* {not selected_random} [The random one]
    Eve: “The right mushroom is the random one.”
    ~ selected_random = 3
* {not selected_lying} [The lying one]
    Eve: “The right mushroom is the lying one.”
    ~ selected_lying = 3

- (answer_3_given) Eve: “Right?”

#sprite: NONE
A simple smile, heavy with an emotion I can’t read, paints her delicate face.

Sariel looks almost… angelic, but there’s a merciless blade hidden in her gaze that slides between each of my ribs.

{((selected_random == 1) && (selected_truthful == 2) && (selected_lying == 3)): ->correct_answer|->wrong_answer } 


= correct_answer

#sprite: NONE
She sighs sweetly, and her knuckles suddenly brush along my jaw again.

#{sprite(_s, 0)}
Sariel: “Look at you… You really did it.”

#sprite: NONE
She leans close, her hushed voice gently caressing my skin.

#{sprite(_s, 0)}
Sariel: “But you didn’t do as well as I’d hoped.”

Sariel: “It looks like you need me even to solve a simple puzzle.”

#sprite: NONE
Her delivery is paradoxically somewhere between solemn news and a lighthearted remark.

My chest constricts, cramped and painful.

#{sprite(_e, 0)}
Eve: “I’m sor-”

#sprite: NONE
She cuts me off, her smile and words too gentle for her previous tone.

#{sprite(_s, 0)}
Sariel: “But it’s okay. I still love you. I’ll always love you, Eve.”

-> post_answer


= wrong_answer

#{sprite(_s, 0)}
Sariel: “Oh, Eve…”

#sprite: NONE
Her smile suddenly drops.

#{sprite(_s, 0)}
Sariel: “You poor thing.”

#sprite: NONE
A hand slides back up, fingers running over the thread. Her thumb and index suddenly apply pressure on each side of my throat.

#{sprite(_s, 0)}
Sariel: “You really thought that was right?”

#sprite: NONE
A breathy laugh fills the air.

She doesn’t sound surprised, and my shame grows hotter.

#{sprite(_s, 0)}
Sariel: “You really can’t do anything on your own, hm?”

#sprite: NONE
Her tone slides into a whisper, intimate and humiliating.

#sprite: NONE
Sariel’s hand suddenly grabs my chin. Her grip is too firm, bordering on painful.

She tilts my head toward each mushroom, respectively, as she corrects me. Each movement is sudden and harsh.

#{sprite(_s, 0)}
Sariel: “The left mushroom is unpredictable, the middle mushroom is the truth-teller, and the right mushroom is the liar.”

#sprite: NONE
She murmurs the answers in my ear, each word landing like a reprimand simply veiled with affection.

-> post_answer

= post_answer

#{sprite(_s, 0)}
Sariel: “Now, tell them the answer.”

#sprite: NONE
Sariel gestures towards the mushrooms, her arms now dropping to her sides.

#{sprite(_e, 0)}
Eve: “Right. Sorry.”

#sprite: NONE
I swallow, my throat feeling rough and dry.

#{sprite(_e, 0)}
Eve: “The left is random, the middle is truthful, and the right is the liar.”
//creature sprite...
#sprite: NONE
The three mushroom figures suddenly drop to the floor, digging into the ground. 

Finally, one holds up a key, slotting it into the gate. It’s unexpectedly clean, its luster not lost.

As the door swings open, Sariel tuts.

#{sprite(_s, 0)}
Sariel: “I suppose I should have expected you’d need me.”

#sprite: NONE
But then, she smiles. It’s soft, yet it feels venomous.

#{sprite(_s, 0)}
Sariel: “It’s alright. I think I like you better this way.”

#{sprite(_e, 0)}
Eve: “I see.”

#sprite: NONE
I mumble out a response. Her implication cuts deeply, and shame flows freely from the wound.

The path ahead smells of damp earth. I step carefully over the soil, where roots curl into the ground like ribs.

//[walking to end of path before flower area]
{forced_move(_s,"FLOWER_AREA_SARIEL", 1)}
~ assign_next_scene(-> part_II.flower_puzzle, true) //sariel responsible for next transition after forced move
>>> STOP_DIALOGUE
-> pseudo_done














