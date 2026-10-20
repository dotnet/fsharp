module Verify

open Test
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.ExprShape

let rec calls expression =
    match expression with
    | Patterns.Call(_, method, args) -> (if method.Name = "guard" then 1 else 0) + List.sumBy calls args
    | ShapeVar _ -> 0
    | ShapeLambda(_, body) -> calls body
    | ShapeCombination(_, args) -> List.sumBy calls args

type Counter() =
    inherit System.Linq.Expressions.ExpressionVisitor()
    let mutable count = 0
    override this.VisitMethodCall(call) =
        if call.Method.Name = "guard" then count <- count + 1
        base.VisitMethodCall(call)
    member _.Count = count

[<EntryPoint>]
let main _ =
    let count =
        match box q with
        | :? Expr as expression -> calls expression
        | :? System.Linq.Expressions.Expression as expression ->
            let visitor = Counter()
            visitor.Visit(expression) |> ignore
            visitor.Count
        | _ -> failwith "Expected quotation or LINQ expression"
    printfn "calls=%d" count
    0
