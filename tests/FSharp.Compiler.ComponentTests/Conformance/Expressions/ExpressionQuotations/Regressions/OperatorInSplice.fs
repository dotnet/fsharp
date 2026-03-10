// #Regression #Conformance #Quotations 
// Regression for FSHARP1.0:5665
// Some operators (subtraction, division) were throwing NotSupportedExceptions at runtime when evaluated in a splice

open Microsoft.FSharp.Quotations
 
// subtraction
let rec expand_power (n : int) x =
    if  n = 0
    then <@ 1 @>
    else 
        <@ %x * %(expand_power (n - 1) x) @>
 
let mk_power n : Expr<int -> int> =
    let v = Var("x", typeof<int>, false)
    Expr.Lambda(v, expand_power n (Expr.Var v |> Expr.Cast)) |> Expr.Cast

ignore <| mk_power 3

// division
let rec expand_power2 (n : int) x =
    if  n = 0
    then <@ 1 @>
    else 
        let y = n - 1 // so we don't stack overflow
        <@ %x * %(expand_power2 (y / 1) x) @>
 
let mk_power2 n : Expr<int -> int> =
    let v = Var("x", typeof<int>, false)
    Expr.Lambda(v, expand_power2 n (Expr.Var v |> Expr.Cast)) |> Expr.Cast

ignore <| mk_power2 3

// if we didn't blow up already then we're good
exit(0)
