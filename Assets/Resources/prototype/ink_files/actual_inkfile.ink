//{this_scene == ->part_I: does it, so it does}
-> admin_powers

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

VAR leashSlackFactor = 1.5


//reminder that knave_puzzle_knot.correct_answer will tell if the player has gotten the answer correct or not

VAR cave_transition_allowed = false
VAR flower_puzzle_start = false
VAR cobweb_puzzle_start = false
VAR cobweb_puzzle_ended = false

VAR flowerCounter = 0 //counter that eve has collected
VAR cobweb_obtained = 0
VAR flowerPotState = 0 // 0, 1, 2, as empty, almost full, full

VAR sariel_can_interact = true //sariel will call this as a check to see if she can interact // TODO : make sure this gets into the update next scene fn!!!!

// Effectively just macros
VAR _step = 0.5 //flat distance of a 'step'

VAR _e = "eve" //this is relatively stupid, but at least it can be ctrl-f-ed out
VAR _s = "sariel" 
VAR _n = "NONE"

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


// 
// 
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

=== function qdfm(character, location_ID, spdFactor)
>>> FORCED_MOVE:{character},QD_{location_ID},{spdFactor}

=== function forced_move_away_off(character, location, flatDistAway, offX, offZ, spdFactor) ===
>>> FORCED_MOVE:{character},{location},{flatDistAway},{offX},{offZ},{spdFactor}

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

=== function flowerpot_set(value) ===
~ flowerPotState = value
>>> FLOWERPOT_SET:{value}
// 0,1,2 by protocol expected to be empty, almost full, full

=== pseudo_done ===
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
>>> STOP_DIALOGUE
-> pseudo_done // this is just to be safe.


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
-> next_scene //changed form being based on acts

/*+ [nothing] <> //this glue makes it so there is no 'blank space'
 - <> The above should not have any key presses which show nothing (nothing!).*/

//primarily stolen from the the doc part
=== part_I ===
#READ_AS_STAGE_LINES:TRUE
//initiate event: new game start
>>> START_DIALOGUE
~ autosave(true, -> part_I)

{backdrop_set(true)} 

There is no sound at first.

Only the faint pressure of existence. The ache of being almost.

Then, a voice. It sings as gently as a fingertip brushing along my jaw, pressing down ever so delicately against my lips.

???,Sariel,silhouette: “Can you hear me?”

I don’t yet know what hearing means, nor what it is to be alive. 

I only know the shape and sound of that voice, smooth and deliberate with utmost clarity. 

It cuts through the fog of my barely formed thoughts, condensing vapor into something tangible, something <i>real</i>. 

The flutter it leaves in my chest suppresses the trace of unease that grazes my heart.

Light follows sound. Blurred at the edges, flickering, like a dwindling candle flame. 

I reach towards it. The light leans closer.

???,Sariel,silhouette: “You’re safe.”

The light’s murmurs are feminine and warm.

???,Sariel,silhouette: “I’ll help you understand.”

I don’t ask what <i>safe</i> means, yet the word takes root in my chest all the same, sprouting into belief. 

The world begins to congeal around me. Damp air, cool soil, and petrichor. 

Me?,Eve,silhouette: “Who are you?” 

As I ask, my voice breaks as it exits my throat. 

Sariel,Sariel,silhouette: “Sariel.” 

The light breathes, almost laughing. 

Sariel,Sariel,silhouette: “And you’re… mine, I think.”

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


Each sound of the forest startles me with its intimacy. The world is a mouth whispering against my ear, and I don’t yet know if it is kind.

Sariel, luminous as ever, trails ahead. 

 
Sariel: “Stay close.” 


Each of her words is draped in fondness.

I nod. It’s easier than asking why.

~ assign_next_scene(-> part_II.lamb_encounter, true)
//~ forced_move(_s,"ANIMAL_AREA", 1) //SARIEL FORCED MOVE TO ANIMAL AREA
~ qdfm(_s,"0",1)
~ qdfm(_s,"1",1)//accomplicshes animal area

>>> STOP_DIALOGUE
//[walking to animal area]
-> pseudo_done

//initiate event: talk to sariel once she finishes her forced move
= lamb_encounter
>>> START_DIALOGUE
~ autosave(true, -> part_II.lamb_encounter)


We come upon a small, white animal in the underbrush, trembling and breathing in broken rhythms.

Its wool is matted with blood, a red too vivid for the incipient palette of my sight.

 
Sariel: “Oh, you poor thing.” 


Sariel’s murmurs are gentle. 

 
Sariel: “An injured lamb. We can help it. Don’t you want to help, Eve?”

 
Eve: “Eve?” 


I echo the name, a lump in my throat. 

I do. The want is immediate, almost desperate, because helping feels like proof of goodness. My previous confusion slips my mind.

Sariel’s hand glides along the side of my neck, guiding my gaze towards the dark mouth of a cave in the distance. 

 
Sariel: “There’s a cave nearby. It’s dark, but you’ll be safe if you hurry. The cobwebs inside can be used to mend wounds.”


I peer into the void. Unease worms its way under my skin.

 
Eve: “If you’re watching the lamb, does that mean I… have to go alone?”


Sariel laughs. It’s gentle and uninhibited, and I find my worries melting under its warmth. 
~ block_init_cave = false
~ cave_transition_allowed = true
~ assign_next_scene(-> part_II.init_cave, false) //sariel NOT RESPONSIBLE for next transition! The darned cave is!
>>> STOP_DIALOGUE //[walking to cave]
-> pseudo_done

= init_cave
//#BLOCK_IF_TRUE: block_init_cave
>>> START_DIALOGUE
~ autosave(true, -> part_II.init_cave)

~ block_init_cave = true
~ cobweb_puzzle_start = true //so that cobweb can be grabbed!
~ cave_transition_allowed = false


Stepping inside feels like drowning upright. The air is far too damp and thick with must. 

I hastily turn to gather the lattices of silver strands that cling to the stone interior.

//[puzzle time]
~ assign_next_scene(-> part_II.post_cave, false) //seems to trigger on cobweb pickup (since there doesn't exist cobwebs yet, rigging via the admin powers is necessary...
>>> STOP_DIALOGUE

-> pseudo_done

= post_cave
>>> START_DIALOGUE
~ autosave(true, -> part_II.post_cave)


My heart finally calms as I finish collecting the last threads the cave has to offer.

Then, static. A flicker at the edge of my sight. Something shifts ahead of me, hoofed and wrong, its silhouette splitting from the fuzzy darkness. //Wait... Where is this occurring? I have not implemented this...

My breath stutters, my heart pounding violently against my ribcage. The dull noise of the cave distills into a single sharp frequency, burrowing itself into my skull.

>>> TELEPORT:SARIEL,CAVE_ENTRANCE,1.0,1.0 

Sariel: “Eve.” //hold up, eve should be in cave, whereas sariel should be in animal area


Sariel’s voice echoes, distant, yet perfectly clear. 

 
Sariel: “Come back to me.”

//{forced_move_dir(_e, "CAVE_INTERIOR", true, 0.7, 1)} //legs obeying
>>> TELEPORT:{_e},CAVE_ENTRANCE,0,0


My legs obey before I do. The world jerks, and my vision fractures into streaks of white noise and almost painful adrenaline.

When I stumble into the light again, Sariel catches me. Her embrace is too tight, almost reverent. 

 
Sariel: “There, there.”

Sariel: “See how dangerous it was without me?”


Her words are soft and breathy.

 
Eve: “Sariel-”


I choke. I try to speak, but my words, a sob, snag in my throat. Sariel brushes a strand of hair away from my face and smiles.

 
Sariel,smile: “You found exactly what I needed. Such a good girl.”


My hands tremble, my heart struggling to come back to a level of normalcy as I watch her tend to the wounded animal, wrapping its leg in the gathered silk. The threads cling beautifully, glowing softly in Sariel’s light.

 
Sariel,smile: “Cobwebs have always been excellent for stopping bleeding,” Sariel explains. “Isn’t that lovely? To be enveloped until you’re whole again.”


I watch the lamb shiver, its leg fully cocooned. An emotion I can’t place ripples somewhere deep within me.

 
Sariel: “I was worried for you. My heart stopped when you suggested going alone.”

 
Eve: “I- I was only-”

 
Sariel: “I know. You just wanted to help.”

Sariel: “And you did. You did so well. But…” 


There’s a pause sharp enough to draw blood. 

 
Sariel,disappointed: “You frightened me, and it seems for good reason.”


The guilt blooms instantly, raw and uncomfortable. 


After a beat, Sariel speaks again.

 
Sariel: “Maybe, it’s best if we make sure you can’t drift away into danger like that again.”

{set_leash_active(true)}


She raises her hand, and something luminescent cinches around my throat. A string of light stretches between us, vanishing into Sariel’s palm. 

 
Sariel: “Try to move now.”

{forced_move_dir(_e, _s, false, -1, 1)}


I step back; the thread tightens, and my heart jumps, nerves alight.

 
Sariel,smile: “See?” 


Sariel’s smile is a wound wrapped in sweetness.

 
Sariel,smile: “Isn’t that better? Don’t you feel <i>safer</i>?”


Her last word echoes through my chest, hollow and obedient. 


She hums softly, almost amused. 

 
Sariel,smile: “Mm, isn’t she beautiful?”


I lift my head, and our eyes meet, but it feels as though her gaze peels back every layer of my being. 

 
Eve: “What?”


Silence. Then, she laughs. 

 
Sariel,laugh: “I’m talking about you.”

 
Sariel: “Now, follow.”


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
//~ forced_move(_s, "APPROACHING_KNAVES", 1) // SARIEL FORCED MOVE TO KNAVE PUZZLE AREA
~ qdfm(_s,"2",1)
~ qdfm(_s, "3",0.75) //knave puzzle area (actual)
~ qdfm(_s, "4",0.75)

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
~ autosave(true, -> part_II.flower_puzzle)
// FORCED_MOVE: <character> <location> <flatDistAway> <offsetX> <offsetZ> <speedFactor>
//~ forced_move_away_off(_s,"FLOWER_AREA_SARIEL", 0, 0, 0, 1)


The trees thin, giving way to an almost impossibly symmetrical glade with flowers sparsely scattered.


I look up from the pale grass bending beneath my feet and spot a large, stone archway.

//sprite: pot FIXXXX
At its feet sits an empty, unassuming clay pot. Its mouth gapes, waiting to be filled.

 
Eve: “{false:UwU }What is this?”

 
Sariel: “A test.” 


As Sariel answers, her feet still for only just a moment. 

 
Sariel: “Everything is.”

~ qdfm(_s, "9", 1)
~ qdfm(_e, "10", 1)
She walks among the flowers with effortless grace, the same way light bends through glass and refracts into a breathtaking spectrum. 

Sariel crouches, lifting a blossom by its stem, and brings it to my face.

 
Sariel: “Smell.”

//#sprite: Overlays/NONE normal flower sprite FIXXX
The fragrance is strange. It’s sweet at first, then metallic, then faintly sharp.

 
Sariel: “This one.”



~ flower_puzzle_start = true //flowers now interactible

 //will pick up first flower


Sariel: “The other kinds won’t do. Fill the pot with these, 10 to be exact, and we will be able to pass through.”

I glance across the meadow. 

I don’t want to question her.

But curiosity tips the scale, outweighing my reluctance.

 
Eve: “How do you know which kind it wants?”


She smiles, soft and unbothered. 

 
Sariel: “Don’t you trust me?”


I do. Before thinking, I nod, though a restless feeling flickers behind my ribs.

Filled with the fervor to please Sariel, I stride towards the edge of the grassy opening. 

~ qdfm(_e,"11",1) //eve picks up first flower
//[walking to flower area]

Kneeling, I begin to gather flowers, inhaling the scents and making mental comparisons to the one Sariel had shown me. The petals cling to my fingers, wet with dew.

//slacken leash
~ set_leash_coef(0,0,0,leashMaxDist * leashSlackFactor, true)


//[puzzle time - collect 8 more flowers]
~ assign_next_scene(-> part_II.last_flower, false) //will get set true by the final flower
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower
>>> START_DIALOGUE
~ autosave(true, -> part_II.last_flower)


Sariel hums as she watches me, low and melodic. 

 
Sariel: “You move so delicately, Eve. Like it’s your own garden you’re tending to.”

 
Eve: “There aren’t any left.” 


I sigh, picking another scentless flower and tossing it aside. 

 
Sariel: “There are always more if you know where to look.”


Her tone is delicate, yet cruel and chastising, causing me to shrink in embarrassment.

I force myself to retrace my steps, familiar blades of grass brushing my calves.

//[walking to entrance of flower area - Sariel does NOT move]
// GOTO FIXXX prolly triggers on either perimeter or on reenter trigger
~ assign_next_scene(-> part_II.last_flower_psych, false)//due to leash stretches
>>> SARIEL_DIST_TRIGGER:TRUE,{3 * leashMaxDist} // NOTE: leashMaxDist does not actually translate well! When MaxDist = 1, I can typically have a natural slack dist of 2 map units, and can reach up to perchance 4 map units away
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower_psych
>>> START_DIALOGUE
~ autosave(true, -> part_II.last_flower_psych)

//note that more of the ROAM state is visible if no sprite is present
#sprite NONE 
The string tightens with disapproval as I step too far, a choked gasp finding itself stuck in my throat. The burn is a gentle, almost affectionate pain.


Sariel’s voice follows, sweet and distant. 

 
Sariel: “Careful. You know what happens when you wander too far.”


I retreat instantly, her hum of disapproval easing into silence. 

 
Eve: “I’m sorry.”

{forced_move_dir(_e, _s, false, 1, 2)} //will face towards sariel when she does this
//[Eve gets moved back to Sariel without roam state when the line above is read - don’t exit dialogue state] //Tusen takk! this is very helpful
// >>> FORCED_MOVE:TO_SARIEL
 
Sariel: “I know you are.” 


Sariel exhales, the sound halfway between amusement and pity. 


{forced_move_dir(_e, "FLOWER_POT_POS", true, 0.7, 1)}
I return to the pot, still missing one bloom. Sariel stands behind me, her chest lightly brushing against my backside, one hand absentmindedly resting on my hip.
//{forced_move_dir(_s, _e, true, 0.7, 1)} NOTE, Make sariel actually move to moving target.

 
Sariel: “Oh, poor thing.” 


She coos, her voice falling somewhere between pity and amusement.

 
Sariel: “You worked so hard.”


From behind, she produces the final flower, perfect and fragrant. 

I stare at the offering in her hand. 

 
Eve: “You… already had it?”

 
Sariel: “Mm.” 


Sariel twirls the stem between her fingers. 

 
Sariel: “You couldn’t do it without me.”

 
Eve: “I didn’t-”

 
Sariel: “You didn’t ask for my help at all.” 


The words glide from her mouth like silk, yet they hit heavy enough to bruise. 

 
Sariel: “I thought you trusted me.”


The guilt lands like a weight in my chest. 

 
Eve: “I do.”

 
Sariel: “Then prove it.”


I shiver from the sudden proximity of her breath against my neck. The hand on my hip tightens painfully before falling away.

Sariel places the stem into my trembling hand. 

 
Sariel: “Now, finish it.”


As the pot receives its final bloom, a heavy creak sounds from the arch.

 
Sariel: “See?” 


I swallow as she whispers against me.

 
Sariel: “When you listen to me, everything is fine.”

Sariel: “You couldn’t have done it without me.”


I nod, unsure whether it’s agreement or surrender. The thread tightens once, almost possessive, yet strangely comforting. 

She smiles delicately, taking a step back. 

 
Sariel: “You’re learning to be good.”

It seems goodness is a script she is sewing into me, thread by thread.

The string around my throat feels warm, and I can’t help but bashfully smile.

I try not to touch it, my fingers restless. It feels too intimate.

Sariel steps ahead, brushing her fingers gently over my shoulder. 

Sariel,Sariel,smile: “Come. There’s one more thing I want you to see.”

Her voice gleams, edged with an excitement too polished for it to be a simple whim.

I follow, but the string urges me along regardless.

//[walk to rose area]
~ assign_next_scene(-> post_pII.rose_bush, true) //Sariel interactable after forced move
~ forced_move(_s,"ROSE_AREA", 1) //SARIEL FORCED MOVE TO ROSE AREA
>>> STOP_DIALOGUE
->pseudo_done


=== knave_puzzle_knot ===
-> pre_puzzle

= pre_puzzle


The branches twist inward like devout believers bent in prayer.

// sprite may actually be the creatures here

They follow the curvature of a very narrow opening, and the function of my brain stutters for a moment as my gaze flits downward.

Ahead sits a cluster of… creatures. 

Three mushroom-like figures. They are humanoid by only the faintest suggestion of their height and overall form. 

Behind them is a heavy wooden gate, the spotty lacquer a testament to its wear.

Their pale mushroom caps glisten with dew, bodies shifting slightly as they notice the two of us approaching.

None blinks. None speaks. They simply watch.

#sprite NONE
A chill crawls up my spine.

 
Eve: “Sariel…?”


She stands just behind me, hands lightly brushing my shoulders, as if positioning me.

 
Sariel: “These three guard the gate.”


Sariel gives a perfunctory glance at the figures before continuing.

 
Sariel: “They’ll open it if you identify their roles correctly.”

 
Eve: “Roles?”

 
Sariel: “One always speaks the truth. One always lies. And one…”


She slightly shifts the angle of her head, breath now warming my neck. I shiver at the contact.

 
Sariel: “One is… unlike the others.”


After waiting a few moments, I realize that’s all she has to offer and stiffen slightly. 

 
Eve: “Unlike the others? Isn’t that too vague?”

 
Sariel,disappointed: “Mm, I thought you were more clever than that, Eve.”


Shame pricks at my skin, hot and uncomfortable.

 
Eve: “I’m sorry.”


I reply before I can even process my mouth moving. My chest constricts, placing my heart in a chokehold.

 
Sariel: “It’s unpredictable. I suppose <i>random</i> is the demotic term.”


Before I can respond, mouth already open, she cuts off my chance.

 
Sariel: “Don’t overthink it. They won’t tell you their nature directly. And besides, wouldn’t that ruin the fun?”

Sariel: “You’ll make me proud, won’t you?”

//maybe mushroom sprite

The mushrooms remain motionless, their beady eyes as unsettling as ever.

 
Eve: “I don’t know how to-”

 
Sariel’s body presses flush against my back, her hands gliding up the curve of my neck until her fingers splay gently on each side of my jaw, cupping my cheeks. 


She turns my gaze to the one on my left, as though prompting me to start already.

 
Sariel: “You can do this.”


She whispers, her breath warm against my ear.

 
Sariel: “Show me how clever you are.”


I swallow.

 
Sariel: “Ask your questions, and they’ll each answer accordingly.”


After pausing for a moment, she taps her fingers on my skin as though she’s had a lightbulb moment.

 
Sariel: “Ah, but I must warn you.”

Sariel: “They will only answer in their native tongue, ‘crrk’ or ‘fmmh,’ instead of ‘yes’ or ‘no.’ Which is which? That’s for you to deduce.”

The noises sound particularly strange coming from her mouth. I wet my lips, fighting back a smile at the absurdity.

 
Sariel notices, and something between a laugh and an exhale exits her nose.

Sariel,laugh: “Laugh if you’d like. I understand.”


I bite my lip, guilt creeping up my throat.

 
Sariel: “You can do it, can’t you? Be good for me.”

Sariel: “If you need help, I’ll be right here.”


Her tone makes simply asking for help feel like complete submission, and my heart thumps heavily.

I peruse the mushrooms. Their eyes are big, wet, and reflective in a way that would almost be cute if not for the size of the creatures. 

 
Eve: “Sariel…”


I exhale, my voice barely audible.

 
Eve: “Which is the random one?”


She laughs breathily, smile blooming too quickly, delighted by the question.

 
Sariel,laugh: “Ah-ah, that would be telling. But… if you want my <i>guess</i>…”


Her lips brush the shell of my ear, and I flinch, heart jumping to my throat.


Anticipation runs through my veins like a heady drug.

 
Sariel: “Aw, did you really think I’d tell you?”


I shrink slightly at the mocking tone she suddenly adopts.

 
Sariel: “Go on. Perform for me, Eve. Let me see how well you can do.”


I take a deep breath.

~ qdfm(_e,"5",1)

Pressing my tongue against the side of my cheek, I step forward.

 
Eve: “The left mushroom. Does ‘crrk’ mean ‘yes’?”


I do my best to copy Sariel’s impression of the strange noise.

Her gaze sharpens with scrutiny, and I quickly add to my question before it’s too late.

 
Eve: “Ah, um, if and only if you are the truthful one… and the, uh, middle mushroom is the random one.”

// creature overlay???

The creature’s cap tilts.

A low, rough syllable escapes its throat.


// add l mushroom sprite FIXXXXX
Left Mushroom: “Fmmh.”


The sound is sticky. My stomach tightens.


Sariel’s hands gently lace over my throat.

 
Sariel: “Mmm… Interesting.”

~ qdfm(_e, "6",1)
I move to the middle mushroom, pulse loud in my ears. 

For a good while, I think about what to say. 

It’s all too fresh. The world and its rules.

My hands clench into fists.

 
Eve: “If and only if cobwebs can stop bleeding... does ‘crrk’ mean ‘yes’?”

//creature overlay

Its mouth opens in a smooth, deliberate motion.

//m mushroom sprite FIXXXX
Middle Mushroom: “Crrk.”

I wince from the high pitch of the noise, expecting the same deep, throaty rumble as before.


Sariel chuckles softly behind me.

 
Sariel: “I suppose I should have warned you.”

 
Eve: “How do you-”


I stop myself before finishing my question. It feels wrong. 

She snaps me out of my thoughts, sensing my hesitation immediately.

 
Sariel: “Aww, Eve.”


Her voice dips into something velvety and condescending.

 
Sariel: “Do you need my help? Or are you going to continue standing there like a fool?”

 
Eve: “O-Oh.”


I stutter, embarrassed.


Her fingers trail down to the thread around my throat, sliding beneath the string in a way that tightens it considerably.

She flicks her fingers up, tugging just enough to steal a breath.

I nod, desperate for her approval.

She smiles, pleased.

 
Sariel: “Ask one more question. To the middle mushroom again.”


I feel lightheaded, her touch and voice too much to process at once.

It’s hard to think.

Where was I going with the last two questions?

I flip through every page of my mind, yet they’re all blank, devoid of any meaning, and the book is titled <color=\#ffffff>Sariel</color>. 

The fear of disappointing her strangles my throat. I need to remember what I wanted to ask.

* [Is ‘fmmh’ ‘yes’ iff. the left one lies?]
     
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if the left mushroom lies?”
    //m mushroom sprite FIXXXX
    Middle Mushroom: “Fmmh.”
    
* [Is ‘crrk’ ‘yes’ iff. the left one is random?]
     
    Eve: “Does ‘crrk’ mean ‘yes’ if and only if the left mushroom is unpredictable?”
    // m mushroom sprite FIXXX
    Middle Mushroom: “Crrk.”
    
* [is ‘fmmh’ ‘yes’ iff. you are the liar?]
     
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if you are the liar?”
    // m mushroom sprite FIXXX
    Middle Mushroom: “Crrk.”

- (asked_3rd_question) {forced_move_dir(_e,_s,true,0.5,1)} I chew the inside of my cheek as it answers, looking back helplessly at Sariel for guidance.

She lets her arms fall slightly, now circling my shoulders, to allow me to face her.

I bring my voice down to a hushed whisper, not wanting the mushrooms to hear my answers just yet.

 
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


A simple smile, heavy with an emotion I can’t read, paints her delicate face.

Sariel looks almost… angelic, but there’s a merciless blade hidden in her gaze that slides between each of my ribs.

{((selected_random == 1) && (selected_truthful == 2) && (selected_lying == 3)): ->correct_answer|->wrong_answer } 


= correct_answer


She sighs sweetly, and her knuckles suddenly brush along my jaw again.

 
Sariel: “Look at you… You really did it.”


She leans close, her hushed voice gently caressing my skin.

 
Sariel: “But you didn’t do as well as I’d hoped.”

Sariel: “It looks like you need me even to solve a simple puzzle.”


Her delivery is paradoxically somewhere between solemn news and a lighthearted remark.

My chest constricts, cramped and painful.

 
Eve: “I’m sor-”


She cuts me off, her smile and words too gentle for her previous tone.

 
Sariel: “But it’s okay. I still love you. I’ll <i>always</i> love you, Eve.”

-> post_answer


= wrong_answer

 
Sariel: “Oh, Eve…”


Her smile suddenly drops.

 
Sariel: “You poor thing.”


A hand slides back up, fingers running over the thread. Her thumb and index suddenly apply pressure on each side of my throat.

 
Sariel: “You really thought that was right?”


A breathy laugh fills the air.

She doesn’t sound surprised, and my shame grows hotter.

 
Sariel: “You <i>really</i> can’t do anything on your own, hm?”


Her tone slides into a whisper, intimate and humiliating.


Sariel’s hand suddenly grabs my chin. Her grip is too firm, bordering on painful.

She tilts my head toward each mushroom, respectively, as she corrects me. Each movement is sudden and harsh.

 
Sariel: “The left mushroom is unpredictable, the middle mushroom is the truth-teller, and the right mushroom is the liar.”


She murmurs the answers in my ear, each word landing like a reprimand simply veiled with affection.

-> post_answer

= post_answer

 
Sariel: “Now, tell them the answer.”


Sariel gestures towards the mushrooms, her arms now dropping to her sides.

 
Eve: “Right. Sorry.”


I swallow, my throat feeling rough and dry.

 
Eve: “The left is random, the middle is truthful, and the right is the liar.”
//creature sprite...

The three mushroom figures suddenly drop to the floor, digging into the ground. 

Finally, one holds up a key, slotting it into the gate. It’s unexpectedly clean, its luster not lost.

As the door swings open, Sariel tuts.

 
Sariel: “I suppose I should have expected you’d need me.”


But then, she smiles. It’s soft, yet it feels venomous.

 
Sariel: “It’s alright. I think I like you better this way.”

 
Eve: “I see.”


I mumble out a response. Her implication cuts deeply, and shame flows freely from the wound.

The path ahead smells of damp earth. I step carefully over the soil, where roots curl into the ground like ribs.

//[walking to end of path before flower area]
//{forced_move(_s,"FLOWER_AREA_SARIEL", 1)}
~ assign_next_scene(-> part_II.flower_puzzle, true) //sariel responsible for next transition after forced move
~ qdfm(_s,"7",1)
~ qdfm(_s,"8",1)

>>> STOP_DIALOGUE
-> pseudo_done


=== post_pII ===

= rose_bush
>>> START_DIALOGUE
~ autosave(true, -> post_pII.rose_bush)

We don’t walk for long before what seems to be Sariel’s item of interest pops up.

The path pauses abruptly, and before it, there is a rosebush.

Peculiarly, the bush has only a single rose, the rest leafy, thick, and green.

The stem is lined top to bottom with large, sharp prickles that glint like wet teeth.

Sariel,Sariel,default: “Pick it.”

My breath catches.

Her authoritative tone leaves no room for misunderstanding or questioning.

No room for anything but obedience.

Eve,Eve,default: “It… looks painful.”

Sariel,Sariel,default: “Most beautiful things are.”

She turns to me fully, hands clasped behind her back.

Her smile is soft, but only in the way the touch of a blade is before it breaks skin.

Sariel,Sariel,smile: “Go on, Eve. I want you to give me a rose.”

I kneel without meaning to.

The prickles are jagged and sizable.

My fingers hover, trembling, refusing to close.

Eve,Eve,sad: “Sariel, if I grab it like this, I’ll-”

Sariel,Sariel,smile: “Bleed?”

Her voice lilts upward with something akin to a childlike delight.

It makes something in my chest curdle.

Sariel,Sariel,default: “I’ve never asked you to do anything meaningless.”

Eve,Eve,default: “But-”

Sariel,Sariel,default: “You trust me, don’t you?”

There it is.

That word again, injected straight into the softest part of me.

Before I know it, my ears are tinged red, hot with guilt and something I can’t place.

I clench my teeth.

Eve,Eve,sad: “I… I do.”

Her gaze weighs heavily, like a silent call for me to prove myself.

My world narrows to a single point.

I reach.

Thorns bite deep immediately, the punctures lighting up my nerves like sparks.

I suck in a shaky breath but don’t make a sound.

I don’t want her to hear me falter.

My hand flinches away on its own, but I force it back like a bad dog.

Pain branches through my hand, threading up my wrist like something trying to root itself in me.

Sariel watches, unreadable.

I grit my teeth. My palm flares with hot, wet agony, blood trickling down my arm.

I can’t do it.

My head pounds with the pressure of held-back tears, and they finally spill over.

I swallow, dizzy.

Sariel grabs my wrist, my blood coating her slender fingers.

A coldness settles into her eyes, sharp and surgical, as she inspects my hand for a moment.

Sariel,Sariel,disappointed: “Do you think you know better than me now?”

Eve,Eve,cry: “No, I just- I thought-”

Sariel,Sariel,disappointed: “You <i>thought</i>?”

Sariel’s nails dig into my wrist, and my breath falters.

Her tone is so different from that childlike glee from before.

The string around my throat cinches.

My vision swims, static crawling in at the corners.

Eve,Eve,cry: “Sariel-!”

Sariel,Sariel,disappointed: “Shh.”

A twist of her wrist.

Air slips away.

My knees buckle, useless.

Sariel catches me even as she strangles me, gently lowering me to the ground with the care of someone setting down a fragile instrument.

A soft laugh, devoid of the cruelty of her actions, is the last thing I hear before everything folds inwards.

~ assign_next_scene(-> post_pII.detention_at_cave, false)
~ set_leash_active(false) // note that leash is broken and should be broken before anyone gets teleported!!!
>>> BACKDROP_TIMER_TRANSITION:5 //number seconds
//[screen fades entirely to black (including text box)] 

//[teleport to cave] 

//~ teleport(_e, "CAVE_INTERIOR", 0, 0) // 0,0 probably the actual cylinder spot FIXXX




>>> STOP_DIALOGUE
->pseudo_done

= detention_at_cave

>>> START_DIALOGUE

~ autosave(true, -> post_pII.detention_at_cave)

Silence, then a ringing. A thin, metallic screech inside my skull.

This scene is oddly familiar.

My eyes snap open to darkness.

It isn’t the natural darkness of the world.

It’s the suffocating dark of a place meant to swallow me.

The cave.

My breath immediately stutters.

My body remembers before I do.

That horned creature.

The static.

Running.

I curl forward, pressing my forehead to my knees, trembling violently.

It’s cold. The only heat comes from the throbbing pain of my hand, and I suddenly realize that the warm thread is gone.

Eve,Eve,cry: “Sariel… Sariel, please…”

Squeezing my eyes shut, I naively wait for her hand on my hair.

Her voice. Her warmth.

Her light.

I receive no answer.

The absence of the thread scrapes against the skin of my throat like a missing limb.

I can’t breathe.

 * [Beg.]
   

 - Eve,Eve,cry: “Sariel- Sariel, I need you- I need-”

My cries collapse into fragmented gasps.

I slightly crack open my eyes, wet sorrow flooding like I’ve opened a dam.

The darkness is suffocating.

Terrified, I move to my knees, assuming a crawling position.

The pressure of my wounded palm against the cold floor causes my elbow to buckle, and I pathetically fall forward, face hitting the ground.

The pain isn’t just blunt. There’s a line of sharpness that makes my eyes blow wide.

I touch my cheek, and there’s a warm wetness thicker than my tears.

Lifting myself back to my knees, I blink in rapid succession, willing my eyes to adjust slightly to the void.

The rose.

It’s no longer tethered to a bush, waiting perfectly before me on the ground.

My heartbeat kicks hard against my ribs.

Did Sariel leave it? Did she want me to-

A thought threads through me quietly like poison.

If I choose it myself, will she come back?

The idea roots itself deep in my chest, a desperate seed.

My hand shakes violently as I reach toward the rose, fingers hovering over it.

My palm still burns from the earlier cuts.

I freeze. Something inside me hesitates, whispering a question if this is the right thing to do.

It’s not the voice of another, but rather a raw instinct.

I clamp my hands over my face, choking on a sob.

Eve,Eve,cry: “I’m scared… I’m scared, Sariel, come back, come back, please, come back-”

Silence.

Nothing but my own ragged breathing.

I can’t tell how much time has passed, but the memory of the previous cave flickers before me, and I tremble.

//[Player choice]
 * [Be good.]

 - The ache of fear grows unbearable, and loneliness chews through the last of my reasoning.

I move.

Fast, desperate, and unthinking, I wrap my hand around the rose.

This time, I don’t hesitate.

Pain erupts instantly. I squeeze until its teeth burrow as deep as possible into the soft flesh of my palm.

Blood drips to the cave floor, and I muffle a scream by biting my sleeve.

All I can hear are my own choked sobs and broken gasps.

But then, a gentle sound breaks my desperate trance.

A soft inhale.

A delighted sigh.

Sariel,Sariel,default: “Eve…”

Sariel’s voice drapes over me like a warm cloth.

I hear the clicks of her steps as she approaches me.

I quickly move to hold up the rose for her, my hand shaking violently.

She gently takes it by the petals and smiles.

I cry out as the prickles are ripped out from my palm.

Sariel kneels before me, cupping my face.

Her thumb traces the shallow cut on my cheek, and I watch as she pulls her hand away, licking the droplet of blood from her fingertip. 

Sariel,Sariel,smile: “Look at what you’ve done. Look how much you love me.”

I sob, collapsing against her.

She holds me tightly. Reverently, almost.

Sariel,Sariel,smile: “Shh. It’s alright. I’m here now.”

Her arms wind around me, gentle and possessive.

Sariel,Sariel,smile: “You must have been so scared. My poor Eve.”

Her breath ghosts over my ear.

Sariel,Sariel,smile: “But I’m here for you now. I’ll help you.”

I nod into her shoulder, tears soaking the fabric of her clothing.

Eve,Eve,cry: “I… I’m sorry. I’m so sorry.”

Sariel,Sariel,smile: “No, no, it’s okay… I’m here now. And you did exactly what you needed to do.”

I swallow back my sobs, trying to steady my breathing.

Eve,Eve,sad: “I want the…”

She presses her forehead to mine, and my words die in my throat.

Sariel,Sariel,smile: “The leash back?”

My brain stutters for a moment at the term she uses, and shame and relief soar through me in equal measure.

Eve,Eve,default: “Yes. Please.”

Her smile softens into something devastating for my heart.

Sariel,Sariel,smile: “Good girl.”

Light curls around my throat, familiar and warm and safe.

Safe.

Sariel kisses my brow as the thread settles into place.

Sariel,Sariel,default: “I’ve missed the sound of your breathing.”

Heat rises to my face. My breathing is nothing short of broken and ragged, but her compliment burns just the same.

Sariel,Sariel,default: “You did so well, didn’t you?”

She waits expectantly, and I hesitantly nod.

Sariel,Sariel,default: “Now, let me keep you safe. Let me have you.”

Her thumb lightly brushes under my eye, catching the last remnant of a tear, and she stands once more.

Sariel’s fingers ghost under my chin, guiding my gaze upward. Her face is adorned with a nearly cherubic smile.

Sariel,Sariel,smile: “There we are. Back where you belong.”

Her words are soft and syrupy, smoothing over the raw edges inside me. The hand I had just cradled in pain falls back to my side.

Sariel,Sariel,default: “Come.”

The thread tugs me to my feet, and I eagerly follow as she leads me out of the cave.

//[walk to the very outside of the cave] FIXXXXX
~ assign_next_scene(-> post_pII.true_ending, true) //Sariel interactable after forced move MAYBE????
//~ forced_move(_s,BUT WHERE, 1) //SUFFER FIXXX
>>> STOP_DIALOGUE
->pseudo_done

= true_ending

>>> START_DIALOGUE
~ autosave(true, -> post_pII.true_ending)

We take a step outside, merely one, before my world as I know it begins to fall apart.

Suddenly, an almost blinding light sputters and flickers like the remnant of a dying flame, illuminating our surroundings in their entirety.

My skin shivers as I see the decrepit state of our surroundings, cracked paint and lacquer coating each object.

Too quickly, the path is plunged back into its familiar darkness.

Eve,Eve,default: “...Sariel?”

Sariel’s hand tightens around mine, almost imperceptibly so.

Sariel,Sariel,default: “You shouldn’t concern yourself with that.”

But I already am.

Eve,Eve,default: “Sariel… What was that?”

She laughs softly, brushing her thumb across my knuckles.

Sariel,Sariel,laugh: “You’re just still disoriented from the cave. Like I said, don’t concern yourself with it. Let’s walk.”

But the world flickers again.

This time, longer.

This time, enough for me to really see the edges of the trees splintering into painted plywood.

My heart drops through my stomach.

Eve,Eve,sad: “No. No, I saw that. Something’s wrong-”

Sariel,Sariel,disappointed: “Eve.”

My name snaps out of her like the warning crack of a whip, and my nerves fracture into a thousand pieces.

Sariel,Sariel,disappointed: “Don’t start imagining things.”

The thread around my neck tightens with a light pressure.

My breath stutters and lungs tremble, but I pull back slightly.

Eve,Eve,sad: “Sariel… What is this place?”

Sariel blinks once, the action deliberate. 

Something inside of her visibly tightens, like a string pulled taut.

Sariel,Sariel,default: “Eve, please. You’re overwhelmed.”

Her voice dips into that kind of maddening softness that makes every protest feel childish.

I almost feel myself getting swayed, but my throat dries.

Eve,Eve,sad: “Sariel. Tell me the truth. Please. What is all this?”

I absentmindedly scratch my palm to calm my nerves, wincing as my body remembers the presence of the wound.

My shoulders tense as I wait for her scolding, but to my surprise, she simply sighs, the sound light and dreamy.

Sariel,Sariel,smile: “You really want to ruin the magic?”

I sputter.

Eve,Eve,sad: “Wh- What do you mean, magic? This world is… None of this is real!”

The moment the word leaves my mouth, her expression shifts.

I find myself flinching in anticipation, but it isn’t anger.

It’s disappointment. 

Sariel,Sariel,disappointed: “Eve. Look at you, trembling over shadows.”

She steps closer, her hand finding my waist, and every inch she closes feels like the tightening of a snare.

Up close like this, it really dawns on me how beautifully seraphic Sariel looks. 

Sariel,Sariel,disappointed: “If I showed you a painted backdrop, you’d swear the world is ending.”

Eve,Eve,sad: “You’re not answering me.”

Sariel,Sariel,default: “Because the answer will only frighten you more.”

Sariel,Sariel,default: “And besides… Would <i>you</i> really understand?”

I swallow the sting of the remark, trying not to lose myself in the frantic, muddy thoughts running through my mind.

The fingers of her other hand slip up my arm, curling delicately around my wrist. Her touch is not forceful, yet it’s no less binding.

Sariel,Sariel,default: “You break so easily. I would’ve told you if you were ready.”

Eve,Eve,sad: “Ready for what?”

Sariel smiles once more. It’s warm as candlelight and just as capable of burning.

Sariel,Sariel,smile: “The truth, Eve.”

I feel her breath on my ear as she leans in.

Sariel,Sariel,smile: “Nothing you’ve seen was ever real.”

My heart stops.

Sariel,Sariel,smile: “Except for <i>me</i>.”

She cups my face in both hands, her touch all too gentle for the cruel enlightenment she bestows upon me.

Sariel,Sariel,smile: “Every tree. Every creature. Every path. All of it was crafted.”

An innocent softness reaches her eyes, the corners crinkling.

Sariel,Sariel,smile: “For you.”

Sariel,Sariel,smile: “Isn’t that the most romantic thing?”

Eve,Eve,cry: “Romantic? You- You put me in danger. You made me bleed-”

Sariel,Sariel,default: “And you came back to me.”

Sariel leans in, her lips almost brushing against mine, and my heart leaps.

Sariel,Sariel,default: “You always come back to me.”

The burning hotness of my face and her close proximity make it hard to think.

I try to alleviate the symptoms and pull away, but the thread tugs me back into her orbit.

Sariel,Sariel,default: “Eve… What frightened you more? The pain, or the thought of losing me?”

The answer claws itself up my throat before I can think. I swallow it down, shaking my head.

Eve,Eve,cry: “That’s not-”

Sariel,Sariel,default: “Everything I’ve done has always been for you.”

She brushes a tear from my cheek, smearing it tenderly with her thumb.

Sariel,Sariel,default: “To teach you that you can rely on me. That you can trust me. That you want to trust me.”

Her voice softens to a whisper.

Sariel,Sariel,default: “And you do, don’t you?”

There’s a tender fragility in her tone that makes my chest ache.

Eve,Eve,cry: “I don’t- I don’t know what to think.”

Sariel,Sariel,default: “Of course you don’t.”

Sariel,Sariel,smile: “That’s why I think for you.”

My eyes widen as the confidence and audacity in her statement take me aback.

Sariel’s hand glides to the back of my neck, fingers threading into the hair at my nape. 

But before I can fully react to her words, she pulls, and my world tilts.

Her soft lips press against mine with a fervor that steals every last shred of breath I thought belonged to me.

It feels as though she’s pouring light into me, like she’s claiming the air in my lungs, my ribs, and the trembling center inside of me.

Her lips move with precision, shaping me like she’s molding clay with her mouth, and I can’t help but feel clumsy in comparison.

I whimper into her.

She deepens the kiss instantly, and I feel almost dizzy. 

Sariel’s hand comes to rest on the side of my neck, her thumb stroking where my pulse thrums frantically beneath my skin.

The touch is possessive, demanding my stillness even as she devours me.

She takes quick, desperate gasps of air against my lips, as if she can’t stand a moment apart.

But I can’t breathe at all with the feeling of her mouth against mine.

I don’t quite know what to do with my hands, but they find themselves scrabbling at her clothing as my lungs begin to burn.

She hums into my mouth, the sound some combination of pleased, amused, and victorious, as though she can feel the way I melt for her.

The vibration travels straight through me, unraveling something tight in my chest.

Finally, her lips part just enough for her breath to ghost over mine, sweet and warm and dizzying.

I greedily suck in the air I’ve been afforded, and I find that it’s hard to think of anything but Sariel.

Sariel,Sariel,default: “Shh.. Let me.”

The words brush against my lips.

Then, Sariel kisses me again, slower this time, but devastating.

Her tongue traces the seam of my mouth with a patience that feels cruel, coaxing me open, not prying, but merely waiting until I give in on my own.

And I do.

The moment I part my lips, she exhales softly like she expected nothing less.

Her tongue meets mine with a slow, languid press that sends heat spiraling downward.

She tastes like something forbidden. Something I should fear but lean into anyway.

Her fingers tighten in my hair, angling my head just so, guiding me exactly where she wants me. 

Her kiss consumes, shapes, and rewrites.

Finally, Sariel pulls back.

Sariel,Sariel,default: “Good girl.”

My breath stutters, my lips tingling, and it feels like she has my heart trembling in the grasp of her hands.

She smears the moisture on my swollen lower lip with her thumb, admiring her work.

And she smiles as though she’s kissed me into being.

My knees weaken. It’s difficult for me to think.

Eve,Eve,default: “You..”

I swallow thickly, head still spinning as I feel the ghost of her lips against mine.

Eve,Eve,sad: “You lied to me.”

Sariel tilts her head, looking genuinely confused, and the cuteness of the action sends a pang straight through my chest.

Sariel,Sariel,default: “How did I lie? I kept you safe. I kept you close.”

Eve,Eve,sad: “Safe? You left me in the cave-”

Sariel,Sariel,default: “To show how much you needed me.”

She lifts my chin with a single finger.

Sariel,Sariel,smile: “And it worked.”

Despite the smugness of her tone, there’s an undeniable blanket of vulnerability draped over her angelic features.

My breath catches.

She leans in, brushing her nose against mine.

Sariel,Sariel,default: “You have nowhere else to go.”

Her whisper threads into my heart like a string drawn through a needle.

Sariel,Sariel,default: “If we’re just performers on a stage, then all I can do is show how much I love you. How much I need you.”

Sariel stares into me, her gaze molten and all-consuming.

Her thumb caresses the thread at my throat. 

Sariel,Sariel,default: “Stay with me. Where it’s warm. Where you’re wanted.”

Sariel,Sariel,default: “Where you’re mine.”

Eve,Eve,default: “...Sariel…”

Sariel,Sariel,default: “Ask yourself, Eve. Without me…”

Her lips graze my ear.

Sariel,Sariel,default: “Who are you?”

The question punches something fragile inside me.

An emotion wells up in my chest, and I can faintly recognize it as yearning.

But yearning is only grief disguised as hope.

I meet Sariel’s gaze again, and she has the same forlorn look in her eyes.

I want to be something real, and she wants to be beside me.

The lights flicker one final time before flipping on, fully collapsing the illusion around us, but she wraps her arms around me before I can fully see the ruins.

Sariel,Sariel,default: “Don’t look at it. Look at me.”

I do.

I can’t help it.

Sariel,Sariel,default: “There’s nothing left for you out there.”

Her words ring too true, and I recognize it’s the same for her.

Sariel presses her forehead to mine, voice soft enough to shatter me completely.

Sariel,Sariel,default: “Let me be your world.”

She waits.

She always waits for my surrender.

And I-

I kneel.

Not because she pulls.

Because something inside me breaks forward.

If we have the same grief, the same hope, the same longing inside…

What’s so wrong with bandaging it together?

Eve,Eve,sad: “...I’m scared.”

Sariel exhales, slow and pleased, brushing her fingers through my hair.

Sariel,Sariel,smile: “Then let me love you the only way that makes you alive.”

Strings bind me, a cage made of light, and I find myself enveloped in her embrace.

The stage lights burn out, and darkness swallows the set.

Only her glow remains.


//>>> ENDING:1
>>> STOP_DIALOGUE
//DON'T FORGOR ABT THE ENDING CALL
->pseudo_done



