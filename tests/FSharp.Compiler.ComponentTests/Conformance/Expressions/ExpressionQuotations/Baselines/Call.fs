// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ "a".Contains("b") @>
let q' = Expr.Call(typeof<string>.GetMethod("Compare", [|typeof<string>; typeof<string>|]), [Expr.Value("a"); Expr.Value("b")])

let r1 = verify q (|Call|_|) "Call (Some (Value (\"a\")), Contains, [Value (\"b\")])"
let r2 = verify q' (|Call|_|) "Call (None, Compare, [Value (\"a\"), Value (\"b\")])"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
