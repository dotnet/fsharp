// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error when defining an Active Pattern with more than seven 'values'
// This is regression test for FSHARP1.0:3562


let (|One|Two|Three|Four|Five|Six|Seven|Eight|) x = One

let (|A|B|C|D|E|F|G|H|) x =
    match x with
    | 0 -> A
    | 1 -> B
    | 2 -> C
    | 3 -> D
    | 4 -> E
    | 5 -> F
    | 6 -> G
    | _ -> H

