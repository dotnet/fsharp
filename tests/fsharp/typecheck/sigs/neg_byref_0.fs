
module Test
(* original error *)  
open Microsoft.FSharp.Quotations.Typed
let test a = ()
let mutable i = 5 in
test <@ &i @>
