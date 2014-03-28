// #Conformance #PatternMatching 
#light

open System.Collections.Generic

// Captured values for refence types
let test1() =
    // Need to use List<T> since F# overrides
    // reference semantics for most other types
    let x = new List<int>()
    x.Add(1)
    x.RemoveAt(0)
    if x.Count <> 0 then exit 1  

    match x with
    | _ as newBoundValue 
        -> if newBoundValue <> x then exit 1
           if not <| newBoundValue.Equals(x) then exit 1
           ()

// Captured values for value types
let test2() =
    let mutable i = 0
    match i with
    | _ as newBoundValue -> if i <> newBoundValue then exit 1
                            i <- 1
                            if i = newBoundValue then exit 1
                            ()
 


test1()
test2()

exit 0
