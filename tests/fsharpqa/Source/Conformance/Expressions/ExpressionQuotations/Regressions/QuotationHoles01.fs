// #Conformance #Quotations 
#light

// Sanity check quotation holes

open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

let quotationWithHole x = <@ 1 + %x @>
let q = quotationWithHole (Expr.Cast (Expr.Value(2)))

let testPassed = 
    match q with
    | Call(None, methodInfo, args) ->
        let a = methodInfo.Name = "op_Addition"
        let b =
            match args with
            | [Int32(1); Int32(2)] -> true
            | _ -> false
        (a = true && b = true)
    | _ -> false

if not testPassed then exit 1

exit 0
        
        
