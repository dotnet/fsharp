// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT 
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    let f7() = 
       let mutable x = 1
       let arr = [| 0uy .. 100uy |]
       for i = 0 to 10000000 do
           hash arr |> ignore
