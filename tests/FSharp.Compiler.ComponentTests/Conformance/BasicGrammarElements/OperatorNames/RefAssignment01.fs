// #Regression #Conformance #BasicGrammarElements #Operators 
// Regression for FSHARP1.0:5809
// Lexfilter doesn't respect := operator

module T

let tup = (1, 2, 3)
let x = 
    let (b, _, _) = tup
    b

// used to get a compilation error on this one
let y = ref 0
y :=
    let (b, _, _) = tup
    b
