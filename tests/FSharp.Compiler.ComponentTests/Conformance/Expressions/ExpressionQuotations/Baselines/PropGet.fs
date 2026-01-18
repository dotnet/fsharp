// #Conformance #Quotations 
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

let q = <@ "AString".Length @>
let q' = Expr.PropertyGet(Expr.Value("AString"), typeof<string>.GetProperty("Length"))

let r1 = verify q (|PropertyGet|_|) "PropertyGet (Some (Value (\"AString\")), Length, [])"
let r2 = verify q' (|PropertyGet|_|) "PropertyGet (Some (Value (\"AString\")), Length, [])"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
