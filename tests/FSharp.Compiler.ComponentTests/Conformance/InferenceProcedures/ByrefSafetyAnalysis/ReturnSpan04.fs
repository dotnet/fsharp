// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let arr = [| 1; 2; 3 |]
    let span = Span<int>(arr)
    span[0] <- 13
    span

[<EntryPoint>]
let main _ =
    if testFunction()[0] <> 13 then failwith "Failed" else 0
    