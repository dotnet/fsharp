// #Regression #Conformance #Quotations 
// Verify type annotation is required for casting quotes
//<Expects status="error" span="(8,5)" id="FS0030">Value restriction: The value 'tq' has an inferred generic type    val tq: Expr<'_a>However, values cannot have generic type variables like '_a in "let x: '_a"\. You can do one of the following:- Define it as a simple data term like an integer literal, a string literal or a union case like "let x = 1"- Add an explicit type annotation like "let x : int"- Use the value as a non-generic type in later code for type inference like "do x"or if you still want type-dependent results, you can define 'tq' as a function instead by doing either:- Add a unit parameter like "let x\(\)"- Write explicit type parameters like "let x<'a>".This error is because a let binding without parameters defines a value, not a function\. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results\.$</Expects>

open Microsoft.FSharp.Quotations

let uq = <@@ let x = 1 in x @@>
let tq = Expr.Cast uq
