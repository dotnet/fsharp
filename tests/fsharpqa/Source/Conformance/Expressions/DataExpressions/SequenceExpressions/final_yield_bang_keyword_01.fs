// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:3135
// Usage of "->>" in a sequence expression is deprecated
// Use "do yield! ..." instead
//<Expects status="success"></Expects>
#light

let s = seq {  for i in [1 .. 2] do yield! seq { yield i+1 } }
(if (Seq.head s) = 2 then 0 else 1) |> exit
