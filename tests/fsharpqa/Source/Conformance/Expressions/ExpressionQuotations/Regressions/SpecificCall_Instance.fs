// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5428
// Make sure SpecificCall pattern correctly returns Some for the instance obj of an instance call
#light

open Microsoft.FSharp.Quotations.DerivedPatterns

let (|StringContainsQ|_|) x = (|SpecificCall|_|) <@ (fun (s1:string) (s2:string) -> s1.Contains(s2)) @> x

let m inp = match inp with 
            | StringContainsQ (None, _, [arg]) -> 1
            | StringContainsQ (Some obj, _, [arg]) -> 0
            | _ -> 1
              
let q = <@ "a".Contains("a") @>
m q |> exit
