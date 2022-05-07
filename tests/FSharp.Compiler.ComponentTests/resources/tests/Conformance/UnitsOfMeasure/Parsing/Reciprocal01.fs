// #Regression #Conformance #UnitsOfMeasure 
// Regression test for FSHARP1.0:2748
// Units or measures: we should not require a space between < and \ when using a reciprocal of a unit

// Regression test for FSHARP1.0:4195
// Reciprocals parsed incorrectly for Measure definitions.

#light
open System

[<Measure>] type s              // [<Measure>] type s
[<Measure>] type s' = / s / s   // [<Measure>] type s' = /s ^ 2
let oneHertz = 1.0</s>          // OK
let twoHertz = 2.0f< /s>        // OK

if (1.0</s/s> + 2.0<s'> <> 3.0</s^2>) then raise (new Exception("if (1.0</s/s> + 2.0<s'> <> 3.0</s^2>)"))
printfn "Finished"
