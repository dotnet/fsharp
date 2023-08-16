// #Regression #Conformance #PatternMatching #PatternMatchingGuards 
#light

//<Expects id="FS0038" status="error">'x' is bound twice in this pattern</Expects>

let test input =
    match input with
    | (1, _) & (_, 1) -> 1
    | (x, _) & (_, x) -> 2
    
exit 1
