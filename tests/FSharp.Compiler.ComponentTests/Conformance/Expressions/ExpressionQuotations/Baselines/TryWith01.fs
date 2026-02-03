// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5649
// Title: Reflected 'try..with' has inverted conditional branches

module Test

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

exception E0
exception E1

let q = <@
            try 0 with 
            | E0 -> 1
            | E1 -> 2
        @>

let r1 = verify q (|TryWith|_|) "TryWith (Value (0), matchValue,
         IfThenElse (TypeTest (E0, matchValue), Value (1),
                     IfThenElse (TypeTest (E1, matchValue), Value (1), Value (0))),
         matchValue,
         IfThenElse (TypeTest (E0, matchValue), Value (1),
                     IfThenElse (TypeTest (E1, matchValue), Value (2),
                                 Call (None, Reraise, []))))"

exit <| if r1 = 0 then 0 else 1
