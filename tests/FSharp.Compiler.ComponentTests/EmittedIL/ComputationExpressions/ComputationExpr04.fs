// #NoMono #NoMT #CodeGen #EmittedIL #ComputationExpressions 
module Program
open Library

let res4 = 
    eventually { 
        try 
            let x = (printfn "hello"; "hello".Length)
            failwith "fail"
            return x 
        with _ -> 
            let x = (printfn "hello"; "hello".Length)
            return x
    }
res4 |> Eventually.force
