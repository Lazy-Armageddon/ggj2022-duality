// variables
VAR name = "Link"
VAR health = 50

-> start

=== start ===
Alice: I am the first NPC.
Alice: What is your name?

*** My name is Bob
-> name_bob
*** My name is Charlie
-> name_charlie
*** My name is Edgar
-> name_edgar

=== name_bob ===
~name = "Bob"
-> meet_you

=== name_charlie ===
~name = "Charlie"
-> meet_you

=== name_edgar ===
~name = "Edgar"
-> meet_you

=== meet_you ===
Alice: Nice to meet you {name}!
Alice: Why don't you jump past me and visit my friend Frank?

-> END
