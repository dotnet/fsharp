// #Regression #Conformance #Quotations 
// Verify type annotation is required for casting quotes
//<Expects status="error" span="(9,5)" id="FS0030">Value restriction\. The value 'tq' has been inferred to have generic type.    val tq: Expr<'_a>    .Either define 'tq' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type annotation\.$</Expects>
//</Expects>

open Microsoft.FSharp.Quotations

let uq = <@@ let x = 1 in x @@>
let tq = Expr.Cast uq
