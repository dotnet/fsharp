// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    type KeyR = { key1: int; key2 : int }
    let f5c() = 
       let mutable x = 1
       for i = 0 to 10000000 do
           hash { key1 = 1; key2 = 2 } |> ignore

