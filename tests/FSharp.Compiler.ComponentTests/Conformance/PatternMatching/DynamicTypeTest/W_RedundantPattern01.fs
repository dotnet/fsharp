// #Regression #Conformance #PatternMatching #TypeTests 
#light

// FSB 1488, Implement redundancy checking for dynamic type test patterns




let _ = 
    match box "3" with 
    | :? string  -> 1 
    | :? string  -> 1  // check this rule is marked as 'never be matched'
    | _ -> 2
    
let _ = 
    match box "3" with 
    | :? System.IComparable -> 1 
    | :? string  -> 1  // check this rule is marked as 'never be matched'
    | _ -> 2
    
exit 0
