// #Conformance #Quotations #Regression
// Dev11:210812 used to give a poor diagnostic here

open Microsoft.FSharp.Quotations

let q1 x = <@@ unbox<string> (%%x : string) @@> // always worked
let q2 x = <@@ unbox<string> (%%x) @@> // bad error in the past

let r = q1 (Expr.Value("hi"))
try
    let r = q2 (Expr.Value("hi"))
    exit 1
with
    | :? System.ArgumentException as e -> 
        let expected = "Type mismatch when splicing expression into quotation literal. The type of the expression tree being inserted doesn't match the type expected by the splicing operation. Expected 'System.String', but received type 'System.Object'. Consider type-annotating with the expected expression type, e.g., (%% x : System.String) or (%x : System.String)."
        if not (e.Message.Contains(expected)) then 
            printfn "%A" (e.Message)
            exit 1

exit 0

