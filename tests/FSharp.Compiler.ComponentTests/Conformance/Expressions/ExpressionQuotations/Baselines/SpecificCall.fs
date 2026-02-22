// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open QuoteUtils

let (|StringContainsQ|_|) x = (|SpecificCall|_|) <@ (fun (s1:string) (s2:string) -> s1.Contains(s2)) @> x

let q = <@ "a".Contains("b") @>
let q' = Expr.Call(Expr.Value("a"), typeof<string>.GetMethod("Contains", [|typeof<string>|]), [Expr.Value("b")])

let r1 = verify q (|StringContainsQ|_|) "Call (Some (Value (\"a\")), Contains, [Value (\"b\")])"
let r2 = verify q' (|StringContainsQ|_|) "Call (Some (Value (\"a\")), Contains, [Value (\"b\")])"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
