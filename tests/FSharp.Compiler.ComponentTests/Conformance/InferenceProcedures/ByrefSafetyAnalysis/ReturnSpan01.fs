// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let arr = [| 1; 2; 3 |]
    let span = Span<int>(arr)
    span[0] <- 13
    span

do
    let result = testFunction()
    let x = &result[0] 
    if x <> 13 then failwith "Failed"
