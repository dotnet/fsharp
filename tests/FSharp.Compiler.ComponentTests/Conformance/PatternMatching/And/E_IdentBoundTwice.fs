// #Regression #Conformance #PatternMatching #PatternMatchingGuards 
#light



let test input =
    match input with
    | (1, _) & (_, 1) -> 1
    | (x, _) & (_, x) -> 2
    
exit 1
