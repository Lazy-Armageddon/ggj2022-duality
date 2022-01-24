VAR vignette2 = false

->unkown

=== unkown ===
Ghost: well, congfratulations on the $800.

-I count my money
Character: thank you.

Ghost: I have one more question for you...

+answer question -> answer
+politely decline -> decline

=== answer ===

    Character: yes. What is your question?
    Ghost: don't get too comfortable. We're going somewhere now
    Character: where too now???
    ->ending

=== decline ===
    Character: uhh no. I have $800 I think I'm the richest person here

->ending

=== ending ===

Ghost: no more talking! We're outta here

~vignette2 = true

->END