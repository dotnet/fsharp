// #Conformance #Quotations 
module QuoteUtils

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Quotations.ExprShape
open System.Collections.Generic

/// Match a quotation with an active pattern. Use of ExprShape means q should represent valid F# code.   
let matchQuote q (|Pat|_|) =
    let mutable result = false 
    let mutable matchedFragment = None
    let rec checkForShape quote (|Pat|_|) = 
        let m = match quote with
                | Pat v -> result <- true
                           matchedFragment <- Some(quote)
                | _ -> ()
        match quote with
        | ShapeVar v -> Expr.Var v
        | ShapeLambda (v,expr) -> Expr.Lambda (v, checkForShape expr (|Pat|_|))
        | ShapeCombination (o, exprs) -> let f x = checkForShape x (|Pat|_|)
                                         let newExprs = List.map f exprs
                                         RebuildShapeCombination(o, newExprs)
    checkForShape q (|Pat|_|) |> ignore
    if result then matchedFragment else None
    
/// Compare a quotation to a string representation of its expected shape
let checkQuote (q : Expr) (expectedShape : string) =
    let x = sprintf "%A" q
    if (x.Replace("\r\n", "\n") = expectedShape.Replace("\r\n", "\n")) then 
        0 
    else
        printfn "Expected:"
        printfn "========="
        printfn "%s" expectedShape
        printfn "Actual:"
        printfn "========="
        printfn "%s" x
        1    
    
/// Verify a quotation matches the given pattern and its string representation is shaped correctly
let verify q (|Pat|_|) expectedShape =
    let qm = matchQuote q (|Pat|_|)
    let s = checkQuote q expectedShape
    match qm, s with
    | Some x, 0 -> 0
    | _ -> 1
