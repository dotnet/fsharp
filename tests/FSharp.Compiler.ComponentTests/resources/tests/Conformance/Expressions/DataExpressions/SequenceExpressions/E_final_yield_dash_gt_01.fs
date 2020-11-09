// #Regression #Conformance #DataExpressions #Sequences 
// Regression test for FSHARP1.0:3135
// See also FSHARP1.0:4207
// Usage of "->" in a sequence expression is deprecated, unless
// in [ for pat in expr -> expr ] and other compact seq expr
//<Expects id="FS0596" span="(8,16-8,23)" status="error">The use of '->' in sequence and computation expressions is limited to the form 'for pat in expr -> expr'\. Use the syntax 'for \.\.\. in \.\.\. do \.\.\. yield\.\.\.' to generate elements in more complex sequence expressions</Expects>

let s1 = seq { -> 10 }
(if (Seq.head s1) = 10 then 0 else 1) |> exit
