// #Conformance #PatternMatching 
#light

let result = 
    match 42 with
    | _  -> true

if result <> true then exit 1

exit 0    
