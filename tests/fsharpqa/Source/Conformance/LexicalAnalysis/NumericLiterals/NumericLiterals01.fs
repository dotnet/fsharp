// #Regression #Conformance #LexicalAnalysis #Constants 
#light

// Verify numeric literals work as expected

let i = 000000000000000000000000000000000000000000000000000000000000000000000000000000042

if i <> 42 then exit 1

// Decimal
let zero = 0.0M
let one  = 1.0M

// (lower case m suffix)
if zero + one + one <> 2.0m then exit 1

// Big int
if 111111111111111111111111111111111111111111111I *
                                               7I <> 777777777777777777777777777777777777777777777I
then exit 1

exit 0
