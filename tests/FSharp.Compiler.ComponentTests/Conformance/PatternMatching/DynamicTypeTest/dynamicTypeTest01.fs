// #Conformance #PatternMatching #TypeTests 
#light

open System

// Verify "If present the identifier after as is bound to the value coerced to the given type."
let testException (ex : obj) =
    let orgType = ex.GetType().ToString()
    match ex with
    | :? System.NotSupportedException as dynamicType -> 
        let typeStr = dynamicType.GetType().ToString()
        if typeStr <> orgType then
            false
        else
            true
    | :? System.Exception as dynamicType ->
        let typeStr = dynamicType.GetType().ToString()
        if typeStr <> orgType then
            false
        else
            true
    | _ -> false
            
if testException (new ArgumentException("") :> obj) <> true then exit 1
if testException (new NotSupportedException("") :> obj) <> true then exit 1

exit 0
