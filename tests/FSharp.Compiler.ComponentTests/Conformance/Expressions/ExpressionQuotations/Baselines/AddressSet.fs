// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5612
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

[<Struct>]
type S(z : int) =
    [<DefaultValue>] val mutable x : int

//let q = <@ let mutable y = S(1).x
//           y <- 2 @>              
let q' = Expr.AddressSet(Expr.AddressOf(Expr.Var(Var("someVar", typeof<int>))), Expr.Value(1))

//let r1 = verify q (|AddressSet|_|) "Let (y,
//     Let (copyOfStruct, NewObject (Void .ctor(Int32), Value (1)),
//          FieldGet (Some (copyOfStruct), Int32 x)), VarSet (y, Value (2)))"
let r2 = verify q' (|AddressSet|_|) "AddressSet (AddressOf (someVar), Value (1))"
//exit <| if r1 = 0 && r2 = 0 then 0 else 1
exit <| r2
