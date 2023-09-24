// #Conformance #TypeInference #ByRef 
open System

type MyType() =
    static member TestMethod() =
        let arr = [| 1; 2; 3 |]
        let span = Span<int>(arr)
        span[0] <- 13
        span
    
    static member Main args =
        if MyType.TestMethod()[0] <> 13 then failwith "Failed" else 0

module Main =
    [<EntryPoint>]
    let main args = MyType.Main args
