// #Regression #Conformance #Quotations 
// Verify type annotation is required for casting quotes
//<Expects status="error" span="(9,5)" id="FS0030">Value restriction: The value 'tq' has an inferred generic type\n    val tq: Expr<'_a>\nHowever, values cannot have generic type variables like '_a in "let x: '_a"\. You can do one of the following:\n- Define it as a simple data term like a string literal or a union case\n- Add an explicit type annotation\n- Use the value as a non-generic type in later code for type inference,\nor if you still want type-dependent results, you can define 'tq' as a function instead by doing either:\n- Add a unit parameter\n- Write explicit type parameters like "let x<'a>"\.\nThis error is because a let binding without parameters defines a value, not a function\. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results\.$</Expects>
//</Expects>

open Microsoft.FSharp.Quotations

let uq = <@@ let x = 1 in x @@>
let tq = Expr.Cast uq
