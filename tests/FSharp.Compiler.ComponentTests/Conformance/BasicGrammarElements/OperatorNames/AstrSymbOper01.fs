// #Conformance #BasicGrammarElements #Operators 
#light

// Verify you can create a function named '*' without
// the lexer thinking its a comment.

let (*) x y = x + y

if 5 * 5 <> 10 then failwith "Failed: : 1"
