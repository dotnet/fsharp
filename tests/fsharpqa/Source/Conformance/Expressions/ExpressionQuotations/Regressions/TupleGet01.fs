// #Regression #Conformance #Quotations #RequiresPowerPack 
// Regression tests for FSHARP1.0:3424
// Make sure that TupleGet works
//<Expects status=success></Expects> 
#light
    open System
    open Microsoft.FSharp
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Linq.QuotationEvaluation

    let e1 = <@@ (1,2) @@>

    let test = try
                let one = Expr.TupleGet(e1, 0).EvalUntyped()
                let two = Expr.TupleGet(e1, 1).EvalUntyped()
                if ((unbox one : int) = 1) && ((unbox two : int) = 2) then true else false 
               with
                | _ -> printfn "ERROR: Unexpected exception in TupleGet"
                       false
                
    let test2 = try
                 let three = Expr.TupleGet(e1, 3)
                 printfn "ERROR: TupleGet did not throw!"
                 false
                with
                 | _ -> true
                 
    (if (test && test2) then 0 else 1) |> exit
