// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
// Regression test for FSHARP1.0:6064

module M

let loop1 N =
   for i in 1 .. N do
      printfn "aaa"
