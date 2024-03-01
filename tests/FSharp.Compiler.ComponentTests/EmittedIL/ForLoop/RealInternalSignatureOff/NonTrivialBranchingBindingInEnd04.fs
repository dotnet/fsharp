// #Regression #CodeGen #Optimizations #ControlFlow 
// Regression test for FSHARP1.0:5306
// Bad codegen while optimizing for-loops with non-trivial branching and binding in end
//<Expects status="success"></Expects>

let r = Array.create 8 1
let w = Array.create 5 2

for j in [(min r.Length w.Length) - 1 .. -1 .. 0] do
   r.[j] <- r.[j] + w.[j]
(if r.[0] = 3 then 0 else 1) |> exit
