
VAR brekky_meter = 0

//I'm instantly diverting this to the tutorial, ignore the 'once_upon' stuff
 -> collection



/* Ignore  all of this immediately below. You can use the 'knot browser in the top left to find something called 'collection' to go to */
Once upon a time...
 - (once_upon)
 * There were two choices.
 * There were four lines of content.
 * I hijacked the story merely to test out the process of writing.
   Literally why would you do this?
    * * because I'm cool
    * * because I'm not cool
    * * becuase[...] I have severe issues
        -> nope
    * * pain?
    - - ya didn't choose right
    * * wait really?
 * 4th choice.
    * * literally one option
    - - shouldn't only '4th choice' direct here?
 + 5th choice []which triggered <>
    <> {once_upon} times... -> once_upon
 + 6th choice[] which is raising the brekky_meter to {brekky_meter}
    ~ brekky_meter++
    actually to {brekky_meter} and that means...
    + + [means what????] {brekky(brekky_meter)}
    * * spare me
    - - 
     -> once_upon 
 * 7th choice
    This is mostly here tot test how long an individual line may actually be considered by the sitor, as it technically only gives strings of 'one line at a time' an that could be nice for allowing things to fit in the textbox.
    Or
    I could have this proceed
    Exceptionally
    Slowly
    - - (7th) and involve user response!
    + + ...[{and what?|why is it repeating|oh god let me out| GET OUT OF MY SKIN | AUUUUGGH!}]
        {7th < 5: -> 7th}
        lived...
        -> once_upon
* The actual tutorial[, not the accursed brainrot]
    -> collection
     
- They lived happily ever after.
    -> END
    

   
    
=== function brekky(a) ===
    {
    - a == 0:
      pancake
    - a == 1: 
      waffle
    - a == 2:
      bacon
    - a == 3:
      cosmic horror
    - else:
      fren toast
    }
    
    
    
=== nope ===
Hello! 
    * So this [is a choice] ligma balls
    + I'm not [falling for that one] {falling for that one | ligma balls}
    
    - (stupid) You totally are
        {stupid < 3: GETTING CYCLED -> nope | okay fine -> END}



=== collection ===
The real tutorial knot was selected.
Way to go.
 -> init_collection //this is a divert!
= init_collection

What would you like to observe?
+ I would like a fallback choice example!
    Observe:
    - - (fallback_opts) //declared exclusively for easy looping
    * * option 1... //must NOT be sticky (i.e. '+') lest they not get used up!
    * * option 2... // same here!
    + + -> //this looks goofy, but it is the syntax for a fallback
        Congrats, you ran out of options!
        {fallback_opts < 4: -> init_collection}
        
    - - {fallback_opts < 4: -> fallback_opts}
        so you're back! notice how options 1 and 2 weren't sticky, so they went away after you first called them, even though you went out of the choice and came back. (btw, see how the fallback '+ + \->' is sticky? if it wasn't (which would be '* * \->'), the story would have broken upon your re-entering, because the 'fallback' would be 'used-up')
        Ink automatically tracks how many times you've visited every choice, knot, stitch (sub-knot), gather, and more! This is the kind of data that is continuously stored and updated in ink, and can be readily called as a variable.
    + + [okay...]
    - - This is why fallbacks are useful, since non-sticky choices may reasonably go away until the whole story restarts, and the ink file won't know what to do when it has no choices.
        Speaking of how your visits to each location are tracked, observe the following: 
        you visited init_collection {init_collection} times, visited  the upper 'gather' of the fallback tutorial {fallback_opts} times, and you visited the 2nd page of the tutorial {scnd_collection} times. (Note that these numbers are saved when an 'ink-story' makes a save-file)
    + + [understood.]
    - - The fallback tutorial is over, diverting to the initial tutorial page...
        -> init_collection

 + General tips on writing and using the editor.
    Note that the editor updates as you write!
    So feel free to change values or input debugger values like the number of times a certain knot has been visited, and it will populate for the previous answers in the editor!
    + + [okay...]
    - - The other thing I'd recommend is that there is a pdf in the discord server under the writing channel which is a tutorial on how to use ink.
        The manual is large (like 260 pages), but most of it is irrelevant for us, and is more useful for 'ctrl + f'-ing for certain terms (e.g. functions, conditionals, sequences, variables, gathers, etc)
    + + [...]
    - - Additionally, feel free to update or alter this tutorial (especially treat the once_upon section as a sandbox).
        Honestly, most of the benefit of the tutorial isn't the direct information in the text walls, but it is observing how the editor responds to the syntax of said text walls.
    + + [...]
    - - getting diverted back
        -> init_collection
 + Implementation specific to our unity game
    + + overall explanation
    -> unity_game_implementation.basic_implementation
    + + database of string commands
    -> unity_game_implementation.string_commands
        

+ [I would like to see some other features]
    -> scnd_collection

- -> init_collection //only here to direct choices to an infinite loop!

= scnd_collection //btw, this is not a not but a 'stitch' (a sub-knot)

+ Explain what is wrong with the non-tutorial
    The other part of this ink file is something I made to test various features myself. Apparently when I (Cooper) do this, I produce inconceivably inane content, but I left it here because it its still helpful, and making a reasonable tutorial takes longer.
    The content in the once_upon section is still useful, and it goes over using functions a little, as well as multiline conditionals, and how complex nested choices (the '*'s (or '+' if sticky) and gathers (the '-'s) behave.
    The most useful part is that it behaves like a sandbox.
    Would you like to go there now?
    + + [yes] -> once_upon
    + + [no]
    - - 'no' is a good choice.
        -> scnd_collection
    
    
    
    
+ Actually I want to see the initial options again
    -> init_collection


=== unity_game_implementation ===
= basic_implementation
    First, observe that the dialogue/narrative system in the prototype is a large textbox with 4 choice-boxes.
    This means we must limit our choices available for each option down to 4. 
    There are many ways to do this, one of which is the common 'show me the next set of choices' tactic used in other games. This is present in the tutorial too! (it is the reason why there is a 'scnd_collection')
    + [...] 
    - The editor innately responds and provides a log of previous choices in the story, which looks really nice, but in reality, ink will just pass our unity game line-by-line strings until it hits a choice.
    This is nice because then we can pass special commands that are ignored by inky, but can be recieved by our game, such as:
    >>> CLOSE_DIALOGUE
    The above isn't a real command, but we will need something like that to know to disengage in communicating with the ink file and allow our character to 'roam' or do whatever.
    There could also be inline commands like ^\red:^this^\red^, but the exact syntax is up to us to determine. (if whoever is writing content would like to add a command or feature, note that they should add it (or tell me to add it) to a database somewhere (even in this tutorial somewhere, perhaps) so that people can understand what they do, and their syntax).
    + [...]
    - Also, while I keep on giving a '...' choice to break-up the editor responses, note that we may prefer to have each individual line require a response from the user (like 'enter' or something so that the dialogue doesn't skip away or so that we can show only one line at a time and not overfill the text box).
    + [...]
    - Very importantly, note that, during dialogue, we would like to know:
    Who (if anyone) is speaking? #speaker:tyli
    What sprite to show for said person if they are speaking? #sprite:tyli_mischevious
    Where to show said sprite (right or left) #display_loc:left
    We would also like to know about what soundfiles to play incase dialogue is voiced, or in the case of other noise effects.
    To incorporate this, it may be easier to use 'tags' (the things with hashtags) and ink has a process for telling our game-system when a tag is encountered. If we're going to be constantly determining a speaker and putting up their sprites and changing them, it may be better to simply include a list of hashtags at the end then include the more clunky string commands.
    + How will the game know where to start and end in the ink story?[]
    - We will not need to connect every possible dialoge in a series of diverts (the '\->' thing) like this tutorial does (that also means we will do a lot less recursive looping, or other goofy tactics).
    Instead, we will mostly be asking the game to jump ('divert,' technically) to whatever knot or stitch we want via unity. 
    This means the -name- of knots and stitches for individual scenes or conversations that are split up by other game-play either shouldn't be changed, or whoever is changing them should make sure the unity element that is calling them is changed too.
    + [...] 
    - Speaking of diverting, I'm going to go on a divert on a tangent and mention variables.
    Ink innately allows for defined variables which arithmetic can be performed on.
    Ink's feature of innately tracking visits will be helpful, but having counters that are shared by the massive ink file that will be produced are also helpful.
    Note that unity can get and set any variable it wants in the inkfile at any time it wants, but this requires knowing and using the name of the variable as it appears in ink, which can lead to difficult debugging errors in the code if this is used too frequently.
    I should mention here that Ink can also store diverts as a variable type. This feels useful to mention, especially for save-state information.
    + Why store anything in ink at all?
    - ink can govern dialogue story easily with its own variables, and has convenient save files. All the visit counts and variable values will be duplicated as a .json file (at runtime, ink files are .json files).
    This means to not ever use \-> END (include it somewhere unreachable so that the editor won't yell at you for not including an \-> END). I'm pretty sure that \-> END and \-> DONE will kill our story if they reach such a point, so that no save-state can be made. 
        
        
    - going back to the tutorial knot: 
        -> collection.init_collection
= string_commands
    Select String Commands.
    * there aren't any here yet.
    - going back to the tutorial knot: 
        -> collection.init_collection




