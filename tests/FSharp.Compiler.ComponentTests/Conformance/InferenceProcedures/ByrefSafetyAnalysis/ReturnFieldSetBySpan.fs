// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let mutable local = 42
    let span = Span<int>(&local)
    span[0] <- 13
    local

if testFunction () <> 13 then failwith "Failed"
