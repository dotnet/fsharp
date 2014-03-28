// #Conformance #BasicGrammarElements #Constants 
#light

// Verify the ability to specify negative numbers
// (And not get confused wrt subtraction.)

let x = -1

if x + x <> -2 then exit 1
if x - x <>  0 then exit 1
if x * x <>  1 then exit 1
if x / x <>  1 then exit 1

exit 0
