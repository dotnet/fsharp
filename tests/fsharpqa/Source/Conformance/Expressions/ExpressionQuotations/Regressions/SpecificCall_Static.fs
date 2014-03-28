// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5428
// Make sure SpecificCall pattern returns None as the instance of SpecificCall
#light

open Microsoft.FSharp.Quotations.DerivedPatterns

let (|EqualsQ|_|) x = (|SpecificCall|_|) <@ ( = ) @> x

let m inp = match inp with 
            | EqualsQ (Some inst, _, [x1;x2]) -> 1
            | EqualsQ (None, _, [x1;x2]) -> 0
            | _ -> 1
              
let q = <@ 1 = 2 @>
m q |> exit
