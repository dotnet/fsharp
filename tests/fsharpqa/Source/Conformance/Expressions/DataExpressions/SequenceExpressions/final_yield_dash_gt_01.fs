// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:3135
// Usage of "->" in a sequence expression is deprecated, unless
// in [ for pat in expr -> expr ] and other compact seq expr
// Compile with --warnaserror
//<Expects status="success"></Expects>
#light

let s1 = seq { for i in [ 1 .. 2 ] -> 10 }
(if (Seq.head s1) = 10 then 0 else 1) |> exit
