// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

type A() =
    member this.X = 1
    
type B() =
    inherit A()
    member this.Y = 2
    
    
let q = <@ B() :> A @>
let q' = Expr.Coerce(Expr.Value(B()), typeof<A>)

let r1 = verify q (|Coerce|_|) "Coerce (NewObject (B), A)"
let r2 = verify q' (|Coerce|_|) "Coerce (Value (Coerce+B), A)"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
