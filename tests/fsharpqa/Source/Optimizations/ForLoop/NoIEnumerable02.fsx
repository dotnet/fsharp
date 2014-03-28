// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
// Regression test for FSHARP1.0:6064

module M

let loop2 N =
   for i in 100 .. N do
      printfn "aaa"
