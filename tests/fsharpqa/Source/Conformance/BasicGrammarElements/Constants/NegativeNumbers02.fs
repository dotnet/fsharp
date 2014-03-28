// #Conformance #BasicGrammarElements #Constants 
#light

// Verify the ability to specify negative numbers
// (And not get confused wrt subtraction.)

let fiveMinusSix   = 5 - 6
let fiveMinusSeven = 5-7
let negativeSeven  = -7

if fiveMinusSix   <> -1     then exit 1
if fiveMinusSeven <> -2     then exit 1
if negativeSeven  <> -1 * 7 then exit 1

exit 0
