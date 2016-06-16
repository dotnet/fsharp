// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

[<Struct>] type t = { Name : string; Age : int } 
let q = <@ let x = { Name = "Bob"; Age = 10; } in x @>
let q' = Expr.NewRecord(typeof<t>, [Expr.Value("Bob"); Expr.Value(10)])

let r1 = verify q (|NewRecord|_|) "Let (x, NewRecord (t, Value (\"Bob\"), Value (10)), x)"
let r2 = verify q' (|NewRecord|_|) "NewRecord (t, Value (\"Bob\"), Value (10))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
