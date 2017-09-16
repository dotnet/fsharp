// #Regression #NoMono #CodeGen #Optimizations #ReqNOMT   
// Regression test for FSHARP1.0:3990
module HashMicroPerfAndCodeGenerationTests = 
    type GenericKey<'a> = GenericKey of 'a * 'a
    let f6() = 
       let mutable x = 1
       for i = 0 to 10000000 do
           hash (GenericKey(1,2)) |> ignore
