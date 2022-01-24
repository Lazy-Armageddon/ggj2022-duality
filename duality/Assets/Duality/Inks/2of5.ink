VAR vignette = false

->purgatory


=== purgatory ===
Ghost: I welcome you to your past
Character: I remember this! 
Character This is Dinkleberry!
Character: When he took my lunch money!
Character: ...the first time
Ghost: yes, you gave him your lunch money and so he went to you for lunch money every chance he got.

+ I'm mad
    -> mad
+[think quitely] ->think


=== mad ===
Character: That guy owes me like $800!!!! 
Angel: Forgive him
Demon: TAKE VENGENCE!
    
	    -> fight

=== think ===				
Character: hmm...
Ghost: you are quite larger these days...perhaps you can ask for your money back
Character: maybe if I ask nicely...
-> fight

=== fight ===
Dinkleberry: well hello
Character: DINKLEBERRY YOU OWE ME $800 
Character: YOU BLOCKHEAD!
Dinkleberry: TAKE IT FROM ME THEN DINGUS! ILL DESTROY YOU!

-The FIGHT BEGINS

~vignette = true
				


->END