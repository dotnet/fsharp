// #Conformance #TypeInference #ByRef 
open System

let testFunction() =
    let mutable x = 10
    let mutable y = 20
    let spanX = Span<int>(&x)
    let spanY = Span<int>(&y)
    spanX[0] <- spanX[0] + spanY[0]
    x

if testFunction() <> 30 then failwith "Failed"
