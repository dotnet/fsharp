// #Conformance #PatternMatching 
#light

open System

let nullValue = Type.GetType("this type does not exist")
let result = 
    match nullValue with
    | null -> true
    | _    -> false

if nullValue <> null then exit 1    
if null <> nullValue then exit 1    // This actually is a pattern match
if result <> true then exit 1

let f = fun null -> true
if f nullValue <> true then exit 1

exit 0
