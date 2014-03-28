// #Conformance #BasicGrammarElements #Constants 
#light

// Verify the ability to specify negative numbers
// (And not get confused wrt subtraction.)

let ident x = x
let add x y = x + y

if ident -10 <> -10 then exit 1
if add -5 -5 <> -10 then exit 1

exit 0
