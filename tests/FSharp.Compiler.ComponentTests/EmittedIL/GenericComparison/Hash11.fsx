// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT 
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    let f8() = 
       let mutable x = 1
       let arr = [| 0 .. 100 |]
       for i = 0 to 10000000 do
           hash arr |> ignore
