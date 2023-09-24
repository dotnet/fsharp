// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let mutable local = 42
    let span = Span<int>(&local)
    span[0] <- 13
    span

if testFunction()[0] <> 13 then failwith "Failed"
