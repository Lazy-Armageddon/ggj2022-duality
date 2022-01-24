VAR vignette2 = false

->purgatory


=== purgatory ===

Ghost: okay. you see that trolley there?
Character: yes...

-There is a trolley a few hundred yards away

Ghost: look at the tracks.
-Looking at the tracks there are people there.

Character: OH MY GOD THERE ARE PEOPLE TIED TO THE TROLLEY TRACKS!!

Ghost: yes and you must decide their fate.
Ghost: either let the trolley roll over the 3 people on the tracks...
Character: Ill do that!
Ghost: or hit the switch and let the trolley roll over the 1 person on the other set of tracks

Character: .......
Ghost: QUICK HERE COMES THE TROLLEY!

+let the trolley go ->trolleyGo
+flip the switch ->flip


=== trolleyGo ===
-watching helplessly as the trolley goes along it's path
Character: stop the train! Someone stop the train!
Ghost: no one is going to stop the train. You must make a decision.
Character: I WON'T FLIP THE SWITCH!


->fight

=== flip ===
Character: I CAN'T DO NOTHING
-running quickly the switch is moved without much effort
-the trolley approaches

Ghost: this is your decision.

-watching helplessly as the trolley goes along it's path
Character: NOOOOOOOOOO

->fight

=== fight ===

Character: WHY WOULD YOU MAKE ME DO THAT!?!?!

Ghost: well it's what i do to everyone who comes here before judgement...

Character: YOUR REIGN OF TERROR ENDS NOW!

~vignette2 = true

->END