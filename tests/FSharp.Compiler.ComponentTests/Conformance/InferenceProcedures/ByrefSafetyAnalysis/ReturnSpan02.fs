// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let arr = [| 1; 2; 3 |]
    let span = Span<int>(arr)
    span[0] <- 13
    span

if testFunction()[0] <> 13 then failwith "Failed"
