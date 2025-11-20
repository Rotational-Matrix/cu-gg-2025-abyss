-> debug_knot

//ctrl-f the following: Overlays/ <-- route to get sprites
// available sprites: eve1, sariel1, sariel2, sariel3 (1,3)

VAR actNumber = 1 //act as in Act I, Act II, etc
VAR next_scene = -> part_I //will track scene to be called


VAR strlenConfigKnown = false
VAR triedToIncreaseBrightness = false
VAR cave_visited = false

//reminder that knave_puzzle_knot.correct_answer will tell if the player has gotten the answer correct or not

VAR flowerCounter = 0


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

=== function assign_next_scene(-> this_scene) ===
~ next_scene = this_scene

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
-> debug_knot // this is just to be safe.

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
~ assign_next_scene(-> part_II.segment_1)

>>> STOP_DIALOGUE //I am personally adding this
-> pseudo_done

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

~ assign_next_scene(-> part_II.lamb_encounter)
>>> STOP_DIALOGUE
//[walking to animal area]
-> pseudo_done

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

~ assign_next_scene(-> part_II.init_cave)
>>> STOP_DIALOGUE //[walking to cave]
-> pseudo_done

= init_cave
#BLOCK_IF_TRUE: cave_visited
>>> START_DIALOGUE

~ cave_visited = true

#sprite: Overlays/eve1
Stepping inside feels like drowning upright. The air is far too damp and thick with must. 

I hastily turn to gather the lattices of silver strands that cling to the stone interior.

//[puzzle time]
~ assign_next_scene(-> part_II.post_cave)
>>> STOP_DIALOGUE

-> pseudo_done

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
~ assign_next_scene(-> part_II.knave_puzzle)
>>> STOP_DIALOGUE
-> pseudo_done
//CONT AT WALK TO KNAVE AREA

= knave_puzzle
//merely directs to the actual puzzle because the puzzle is much longer
-> knave_puzzle_knot

= flower_puzzle
>>> START_DIALOGUE
#sprite: NONE
The trees thin, giving way to an almost impossibly symmetrical glade with flowers sparsely scattered.

#sprite: Overlays/eve1
I look up from the pale grass bending beneath my feet and spot a large, stone archway.

//sprite: pot
#sprite: NONE
At its feet sits an empty, unassuming clay pot. Its mouth gapes, waiting to be filled.

#sprite: Overlays/eve1
Eve: “{false:UwU} What is this?”

#sprite: Overlays/sariel3
Sariel: “A test.” 

As Sariel answers, her feet still for only just a moment. 

Sariel: “Everything is.”

She walks among the flowers with effortless grace, the same way light bends through glass and refracts into a breathtaking spectrum. 

Sariel crouches, lifting a blossom by its stem, and brings it to my face.

Sariel: “Smell.”

The fragrance is strange. It’s sweet at first, then metallic, then faintly sharp.

Sariel: “This one.”

Sariel: “The other kinds won’t do. Fill the pot with these, 10 to be exact, and we will be able to pass through.”

#sprite: Overlays/eve1
I glance across the meadow. 

I don’t want to question her.

But curiosity tips the scale, outweighing my reluctance.

Eve: “How do you know which kind it wants?”

#sprite: Overlays/sariel3
She smiles, soft and unbothered. 

Sariel: “Don’t you trust me?”

#sprite: Overlays/eve1
I do. Before thinking, I nod, though a restless feeling flickers behind my ribs.

Filled with the fervor to please Sariel, I stride towards the edge of the grassy opening. 

//[walking to flower area]
//many of the [walking] may become forced movement commands, but this one is 100% a forced movement command
// >>> FORCED_MOVE:flower_area TODO FIXXX

Kneeling, I begin to gather flowers, inhaling the scents and making mental comparisons to the one Sariel had shown me. The petals cling to my fingers, wet with dew.

//[puzzle time - collect 8 more flowers]
~ assign_next_scene(-> part_II.last_flower)
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower
>>> START_DIALOGUE

#sprite: Overlays/sariel3
Sariel hums as she watches me, low and melodic. 

Sariel: “You move so delicately, Eve. Like it’s your own garden you’re tending to.”

#sprite: Overlays/eve1
Eve: “There aren’t any left.” 

I sigh, picking another scentless flower and tossing it aside. 

#sprite: Overlays/sariel3
Sariel: “There are always more if you know where to look.”

Her tone is delicate, yet cruel and chastising, causing me to shrink in embarrassment.

#sprite: Overlays/eve1
I force myself to retrace my steps, familiar blades of grass brushing my calves.

//[walking to entrance of flower area - Sariel does NOT move]
// GOTO FIXXX prolly triggers on either perimeter or on reenter trigger
~ assign_next_scene(-> part_II.last_flower_psych)
>>> STOP_DIALOGUE
-> pseudo_done

= last_flower_psych

>>> START_DIALOGUE

//note that more of the ROAM state is visible if no sprite is present
#sprite NONE 
The string tightens with disapproval as I step too far, a choked gasp finding itself stuck in my throat. The burn is a gentle, almost affectionate pain.

#sprite: Overlays/sariel3
Sariel’s voice follows, sweet and distant. 

Sariel: “Careful. You know what happens when you wander too far.”

#sprite: Overlays/eve1
I retreat instantly, her hum of disapproval easing into silence. 

Eve: “I’m sorry.”


//[Eve gets moved back to Sariel without roam state when the line above is read - don’t exit dialogue state] //Tusen takk! this is very helpful
// >>> FORCED_MOVE:TO_SARIEL
#sprite: Overlays/sariel3
Sariel: “I know you are.” 

Sariel exhales, the sound halfway between amusement and pity. 

#sprite: Overlays/eve1
// sounds like a >>> FORCED_MOVE:flower_pot
I return to the pot, still missing one bloom. Sariel stands behind me, her chest lightly brushing against my backside, one hand absentmindedly resting on my hip.

#sprite: Overlays/sariel3
Sariel: “Oh, poor thing.” 

She coos, her voice falling somewhere between pity and amusement.

Sariel: “You worked so hard.”

From behind, she produces the final flower, perfect and fragrant. 

#sprite: Overlays/eve1
I stare at the offering in her hand. 

Eve: “You… already had it?”

#sprite: Overlays/sariel3
Sariel: “Mm.” 

Sariel twirls the stem between her fingers. 

Sariel: “You couldn’t do it without me.”

#sprite: Overlays/eve1
Eve: “I didn’t-”

Sariel: “You didn’t ask for my help at all.” 

The words glide from her mouth like silk, yet they hit heavy enough to bruise. 

Sariel: “I thought you trusted me.”

#sprite: Overlays/eve1
The guilt lands like a weight in my chest. 

Eve: “I do.”

#sprite: Overlays/sariel3
Sariel: “Then prove it.”

#sprite: Overlays/eve1
I shiver from the sudden proximity of her breath against my neck. The hand on my hip tightens painfully before falling away.

#sprite: Overlays/sariel3
Sariel places the stem into my trembling hand. 

Sariel: “Now, finish it.”

#sprite: NONE
As the pot receives its final bloom, a heavy creak sounds from the arch.

#sprite: Overlays/sariel3
Sariel: “See?” 

#sprite: Overlays/eve1
I swallow as she whispers against me.

#sprite: Overlays/sariel3
Sariel: “When you listen to me, everything is fine.”

Sariel: “You couldn’t have done it without me.”

#sprite: Overlays/eve1
I nod, unsure whether it’s agreement or surrender. The thread tightens once, almost possessive, yet strangely comforting. 

#sprite: Overlays/sariel3
She smiles delicately, taking a step back. 

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

#sprite Overlays/eve1
A chill crawls up my spine.

Eve: “Sariel…?”

#sprite: Overlays/sariel3
She stands just behind me, hands lightly brushing my shoulders, as if positioning me.

Sariel: “These three guard the gate.”

Sariel gives a perfunctory glance at the figures before continuing.

Sariel: “They’ll open it if you identify their roles correctly.”

#sprite: Overlays/eve1
Eve: “Roles?”

#sprite: Overlays/sariel3
Sariel: “One always speaks the truth. One always lies. And one…”

She slightly shifts the angle of her head, breath now warming my neck. I shiver at the contact.

Sariel: “One is… unlike the others.”

After waiting a few moments, I realize that’s all she has to offer and stiffen slightly. 

#sprite: Overlays/eve1
Eve: “Unlike the others? Isn’t that too vague?”

#sprite: Overlays/sariel3
Sariel: “Mm, I thought you were more clever than that, Eve.”

#sprite: Overlays/eve1
Shame pricks at my skin, hot and uncomfortable.

Eve: “I’m sorry.”

I reply before I can even process my mouth moving. My chest constricts, placing my heart in a chokehold.

#sprite: Overlays/sariel3
Sariel: “It’s unpredictable. I suppose random is the demotic term.”

Before I can respond, mouth already open, she cuts off my chance.

Sariel: “Don’t overthink it. They won’t tell you their nature directly. And besides, wouldn’t that ruin the fun?”

Sariel: “You’ll make me proud, won’t you?”

//maybe mushroom sprite
#sprite: NONE
The mushrooms remain motionless, their beady eyes as unsettling as ever.

#sprite: Overlays/eve1
Eve: “I don’t know how to-”

#sprite: Overlays/sariel3
Sariel’s body presses flush against my back, her hands gliding up the curve of my neck until her fingers splay gently on each side of my jaw, cupping my cheeks. 

She turns my gaze to the one on my left, as though prompting me to start already.

Sariel: “You can do this.”

She whispers, her breath warm against my ear.

Sariel: “Show me how clever you are.”

#sprite: Overlays/eve1
I swallow.

#sprite: Overlays/sariel3
Sariel: “Ask your questions, and they’ll each answer accordingly.”

After pausing for a moment, she taps her fingers on my skin as though she’s had a lightbulb moment.

Sariel: “Ah, but I must warn you.”

Sariel: “They will only answer in their native tongue, ‘crrk’ or ‘fmmh,’ instead of ‘yes’ or ‘no.’ Which is which? That’s for you to deduce.”

#sprite: Overlays/eve1
The noises sound particularly strange coming from her mouth. I wet my lips, fighting back a smile at the absurdity.

#sprite: Overlays/sariel3
Sariel notices, and something between a laugh and an exhale exits her nose.

Sariel: “Laugh if you’d like. I understand.”

#sprite: Overlays/eve1
I bite my lip, guilt creeping up my throat.

#sprite: Overlays/sariel3
Sariel: “You can do it, can’t you? Be good for me.”

Sariel: “If you need help, I’ll be right here.”

Her tone makes simply asking for help feel like complete submission, and my heart thumps heavily.

#sprite: Overlays/eve1
I peruse the mushrooms. Their eyes are big, wet, and reflective in a way that would almost be cute if not for the size of the creatures. 

Eve: “Sariel…”

I exhale, my voice barely audible.

Eve: “Which is the random one?”

She laughs breathily, smile blooming too quickly, delighted by the question.

#sprite: Overlays/sariel3
Sariel: “Ah-ah, that would be telling. But… if you want my guess…”

Her lips brush the shell of my ear, and I flinch, heart jumping to my throat.

#sprite: Overlays/eve1
Anticipation runs through my veins like a heady drug.

#sprite: Overlays/sariel3
Sariel: “Aw, did you really think I’d tell you?”

#sprite: Overlays/eve1
I shrink slightly at the mocking tone she suddenly adopts.

#sprite: Overlays/sariel3
Sariel: “Go on. Perform for me, Eve. Let me see how well you can do.”

#sprite: Overlays/eve1
I take a deep breath.

Pressing my tongue against the side of my cheek, I step forward.

Eve: “The left mushroom. Does ‘crrk’ mean ‘yes’?”

I do my best to copy Sariel’s impression of the strange noise.

Her gaze sharpens with scrutiny, and I quickly add to my question before it’s too late.

Eve: “Ah, um, if and only if you are the truthful one… and the, uh, middle mushroom is the random one.”

// creature overlay???
#sprite: NONE
The creature’s cap tilts.

A low, rough syllable escapes its throat.

Left Mushroom: “Fmmh.”

#sprite: Overlays/eve1
The sound is sticky. My stomach tightens.

#sprite: Overlays/sariel3
Sariel’s hands gently lace over my throat.

Sariel: “Mmm… Interesting.”

#sprite: Overlays/eve1
I move to the middle mushroom, pulse loud in my ears. 

For a good while, I think about what to say. 

It’s all too fresh. The world and its rules.

My hands clench into fists.

Eve: “If and only if cobwebs can stop bleeding... does ‘crrk’ mean ‘yes’?”

//creature overlay
#sprite: NONE
Its mouth opens in a smooth, deliberate motion.

Middle Mushroom: “Crrk.”

I wince from the high pitch of the noise, expecting the same deep, throaty rumble as before.

#sprite: Overlays/sariel3
Sariel chuckles softly behind me.

Sariel: “I suppose I should have warned you.”

#sprite: Overlays/eve1
Eve: “How do you-”

I stop myself before finishing my question. It feels wrong. 

#sprite: Overlays/sariel3
She snaps me out of my thoughts, sensing my hesitation immediately.

Sariel: “Aww, Eve.”

Her voice dips into something velvety and condescending.

Sariel: “Do you need my help? Or are you going to continue standing there like a fool?”

#sprite: Overlays/eve1
Eve: “O-Oh.”

I stutter, embarrassed.

#sprite: Overlays/sariel3
Her fingers trail down to the thread around my throat, sliding beneath the string in a way that tightens it considerably.

She flicks her fingers up, tugging just enough to steal a breath.

#sprite: Overlays/eve1
I nod, desperate for her approval.

#sprite: Overlays/sariel3
She smiles, pleased.

Sariel: “Ask one more question. To the middle mushroom again.”

#sprite: Overlays/eve1
I feel lightheaded, her touch and voice too much to process at once.

It’s hard to think.

Where was I going with the last two questions?

I flip through every page of my mind, yet they’re all blank, devoid of any meaning, and the book is titled Sariel. 

The fear of disappointing her strangles my throat. I need to remember what I wanted to ask.

* [Is ‘fmmh’ ‘yes’ iff. the left one lies?]
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if the left mushroom lies?”
    Middle Mushroom: “Fmmh.”
* [Is ‘crrk’ ‘yes’ iff. the left one is random?]
    Eve: “Does ‘crrk’ mean ‘yes’ if and only if the left mushroom is unpredictable?”
    Middle Mushroom: “Crrk.”
* [is ‘fmmh’ ‘yes’ iff. you are the liar?]
    Eve: “Does ‘fmmh’ mean ‘yes’ if and only if you are the liar?”
    Middle Mushroom: “Crrk.”

- (asked_3rd_question) I chew the inside of my cheek as it answers, looking back helplessly at Sariel for guidance.

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

#sprite: Overlays/sariel3
A simple smile, heavy with an emotion I can’t read, paints her delicate face.

Sariel looks almost… angelic, but there’s a merciless blade hidden in her gaze that slides between each of my ribs.

{((selected_random == 1) && (selected_truthful == 2) && (selected_lying == 3)): ->correct_answer|->wrong_answer } 


= correct_answer

#sprite: Overlays/sariel3
She sighs sweetly, and her knuckles suddenly brush along my jaw again.

Sariel: “Look at you… You really did it.”

She leans close, her hushed voice gently caressing my skin.

Sariel: “But you didn’t do as well as I’d hoped.”

Sariel: “It looks like you need me even to solve a simple puzzle.”

Her delivery is paradoxically somewhere between solemn news and a lighthearted remark.

My chest constricts, cramped and painful.

#sprite: Overlays/eve1
Eve: “I’m sor-”

#sprite: Overlays/sariel3
She cuts me off, her smile and words too gentle for her previous tone.

Sariel: “But it’s okay. I still love you. I’ll always love you, Eve.”

-> post_answer


= wrong_answer

#sprite: Overlays/sariel3
Sariel: “Oh, Eve…”

Her smile suddenly drops.

Sariel: “You poor thing.”

A hand slides back up, fingers running over the thread. Her thumb and index suddenly apply pressure on each side of my throat.

Sariel: “You really thought that was right?”

A breathy laugh fills the air.

She doesn’t sound surprised, and my shame grows hotter.

Sariel: “You really can’t do anything on your own, hm?”

Her tone slides into a whisper, intimate and humiliating.

Sariel’s hand suddenly grabs my chin. Her grip is too firm, bordering on painful.

She tilts my head toward each mushroom, respectively, as she corrects me. Each movement is sudden and harsh.

Sariel: “The left mushroom is unpredictable, the middle mushroom is the truth-teller, and the right mushroom is the liar.”

She murmurs the answers in my ear, each word landing like a reprimand simply veiled with affection.

-> post_answer

= post_answer

#sprite: Overlays/sariel3
Sariel: “Now, tell them the answer.”

Sariel gestures towards the mushrooms, her arms now dropping to her sides.

#sprite: Overlays/eve1
Eve: “Right. Sorry.”

I swallow, my throat feeling rough and dry.

Eve: “The left is random, the middle is truthful, and the right is the liar.”
//creature sprite...
#sprite: NONE
The three mushroom figures suddenly drop to the floor, digging into the ground. 

Finally, one holds up a key, slotting it into the gate. It’s unexpectedly clean, its luster not lost.

#sprite: Overlays/sariel3
As the door swings open, Sariel tuts.

Sariel: “I suppose I should have expected you’d need me.”

But then, she smiles. It’s soft, yet it feels venomous.

Sariel: “It’s alright. I think I like you better this way.”

#sprite: Overlays/eve1
Eve: “I see.”

I mumble out a response. Her implication cuts deeply, and shame flows freely from the wound.

The path ahead smells of damp earth. I step carefully over the soil, where roots curl into the ground like ribs.

//[walking to end of path before flower area]
~ assign_next_scene(-> part_II.flower_puzzle)
>>> STOP_DIALOGUE
-> pseudo_done














