// #Regression #CodeGen #Optimizations #ControlFlow #NoMono #ReqNOMT 
// Regression test for FSHARP1.0:6064

module M

let loop3 a N = 
   for i in a .. N do
      printfn "aaa"

